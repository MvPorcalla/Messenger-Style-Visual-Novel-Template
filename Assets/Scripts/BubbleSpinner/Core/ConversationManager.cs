// ════════════════════════════════════════════════════════════════════════
// Assets/Scripts/BubbleSpinner/Core/ConversationManager.cs
// ════════════════════════════════════════════════════════════════════════

using System.Collections.Generic;
using UnityEngine;
using BubbleSpinner.Data;

namespace BubbleSpinner.Core
{
    /// <summary>
    /// Manages the lifecycle of conversations, including starting, saving, loading, and ending conversations.
    /// It interacts with DialogueExecutor to run conversations 
    /// and uses IBubbleSpinnerCallbacks to communicate with external systems for saving/loading and event notifications.
    /// This class is designed to be the central hub for all conversation-related logic in BubbleSpinner.
    /// </summary>
    public class ConversationManager : MonoBehaviour
    {
        // ═══════════════════════════════════════════════════════════
        // ░ PRIVATE TYPES
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Holds a paired executor + state for one active conversation.
        /// Replaces the previous separate activeExecutors and activeStates dictionaries,
        /// which could desync if one was updated without the other (Phase 2 fix #8).
        /// State is always cached here on creation, fixing the orphaned-state bug
        /// where GetOrCreateState returned a new state that never landed in activeStates (fix #3).
        /// </summary>
        private class ConversationSession
        {
            public DialogueExecutor executor;
            public ConversationState state;
        }

        // ═══════════════════════════════════════════════════════════
        // ░ CONSTANTS
        // ═══════════════════════════════════════════════════════════

        private const float SAVE_THROTTLE_DELAY = 0.5f;

        // ═══════════════════════════════════════════════════════════
        // ░ DEPENDENCIES (injected via Initialize)
        // ═══════════════════════════════════════════════════════════

        private IBubbleSpinnerCallbacks callbacks;

        // ═══════════════════════════════════════════════════════════
        // ░ STATE
        // ═══════════════════════════════════════════════════════════

        // Single dictionary replaces the previous activeExecutors + activeStates pair
        private Dictionary<string, ConversationSession> activeSessions = new Dictionary<string, ConversationSession>();

        private string currentConversationId;

        // Save throttling
        private float lastSaveTime = -999f;
        private bool hasPendingSave = false;

        // ═══════════════════════════════════════════════════════════
        // ░ PROPERTIES
        // ═══════════════════════════════════════════════════════════

        public DialogueExecutor CurrentExecutor =>
            currentConversationId != null && activeSessions.TryGetValue(currentConversationId, out var s)
                ? s.executor
                : null;

        public string CurrentConversationId => currentConversationId;
        public bool HasActiveConversation => CurrentExecutor != null;

        // ═══════════════════════════════════════════════════════════
        // ░ INITIALIZATION
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Initialize ConversationManager with external callbacks.
        /// </summary>
        public void Initialize(IBubbleSpinnerCallbacks externalCallbacks)
        {
            callbacks = externalCallbacks ?? throw new System.ArgumentNullException(nameof(externalCallbacks));
            Debug.Log("[ConversationManager] Initialized with external callbacks");
        }

        // ═══════════════════════════════════════════════════════════
        // ░ PUBLIC API - CONVERSATION CONTROL
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Starts or resumes a conversation.
        /// Returns the DialogueExecutor for UI to subscribe to.
        /// </summary>
        public DialogueExecutor StartConversation(ConversationAsset asset)
        {
            if (callbacks == null)
            {
                Debug.LogError("[ConversationManager] Not initialized! Call Initialize() first.");
                return null;
            }

            if (asset == null)
            {
                Debug.LogError("[ConversationManager] Cannot start null conversation");
                return null;
            }

            string convId = asset.ConversationId;
            Debug.Log($"[ConversationManager] Starting conversation: {asset.characterName} (ID: {convId})");

            if (!activeSessions.ContainsKey(convId))
            {
                // ── PHASE 2 FIX (#3 + #8) ───────────────────────────────────────
                // Previously: GetOrCreateState() returned a new ConversationState
                // but only sometimes stored it in activeStates, depending on call
                // site. activeExecutors and activeStates were separate dictionaries
                // that could drift out of sync.
                //
                // Now: state is loaded/created here, immediately paired with its
                // executor in a ConversationSession, and stored in one dictionary.
                // There is no path where state or executor exist without the other.
                // ────────────────────────────────────────────────────────────────

                ConversationState state = LoadOrCreateState(convId, asset.characterName);

                var executor = new DialogueExecutor();
                executor.Initialize(asset, state, callbacks);
                SubscribeToExecutorEvents(executor);

                activeSessions[convId] = new ConversationSession
                {
                    executor = executor,
                    state = state
                };

                Debug.Log($"[ConversationManager] Created new session for {convId}");

                // Save after executor initialized state properly
                ForceSaveGame();
                Debug.Log($"[ConversationManager] ✓ Initial save complete: Node='{state.currentNodeName}'");
            }
            else
            {
                Debug.Log($"[ConversationManager] Reusing existing session for {convId}");
            }

            currentConversationId = convId;
            callbacks.OnConversationStarted(convId);

            return activeSessions[convId].executor;
        }

        /// <summary>
        /// Saves the current conversation state (throttled).
        /// </summary>
        public void SaveCurrentConversation()
        {
            if (string.IsNullOrEmpty(currentConversationId) || CurrentExecutor == null)
            {
                Debug.LogWarning("[ConversationManager] No active conversation to save");
                return;
            }

            SaveConversationState(throttle: true);
        }

        /// <summary>
        /// Forces an immediate save (bypasses throttle).
        /// Use for critical save points (quit, pause, chapter end).
        /// </summary>
        public void ForceSaveCurrentConversation()
        {
            if (string.IsNullOrEmpty(currentConversationId) || CurrentExecutor == null)
            {
                Debug.LogWarning("[ConversationManager] No active conversation to force save");
                return;
            }

            SaveConversationState(throttle: false);
        }

        /// <summary>
        /// Ends the current conversation and saves state.
        /// </summary>
        public void EndCurrentConversation()
        {
            if (CurrentExecutor == null)
            {
                Debug.LogWarning("[ConversationManager] No active conversation to end");
                return;
            }

            Debug.Log($"[ConversationManager] Ending conversation: {currentConversationId}");

            ForceSaveCurrentConversation();

            if (activeSessions.TryGetValue(currentConversationId, out var session))
            {
                UnsubscribeFromExecutorEvents(session.executor);
            }

            callbacks?.OnConversationEnded(currentConversationId);

            currentConversationId = null;

            Debug.Log("[ConversationManager] Conversation ended");
        }

        /// <summary>
        /// Resets a conversation to its initial state (clears save data).
        /// </summary>
        public void ResetConversation(string conversationId)
        {
            Debug.Log($"[ConversationManager] Resetting conversation: {conversationId}");

            if (activeSessions.TryGetValue(conversationId, out var session))
            {
                UnsubscribeFromExecutorEvents(session.executor);
                activeSessions.Remove(conversationId);
            }

            if (currentConversationId == conversationId)
            {
                currentConversationId = null;
            }

            callbacks?.DeleteConversationState(conversationId);

            Debug.Log($"[ConversationManager] Conversation reset complete: {conversationId}");
        }

        /// <summary>
        /// Evicts in-memory session cache WITHOUT touching the save file.
        /// Called by BubbleSpinnerBridge after SaveManager.ResetCharacterStory()
        /// has already wiped the progress data on disk.
        /// </summary>
        public void EvictConversationCache(string conversationId)
        {
            Debug.Log($"[ConversationManager] Evicting cache for: {conversationId}");

            if (activeSessions.TryGetValue(conversationId, out var session))
            {
                UnsubscribeFromExecutorEvents(session.executor);
                activeSessions.Remove(conversationId);
            }

            if (currentConversationId == conversationId)
            {
                currentConversationId = null;
            }

            Debug.Log($"[ConversationManager] ✓ Cache evicted for: {conversationId}");
        }

        // ═══════════════════════════════════════════════════════════
        // ░ CG GALLERY API
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Gets all unlocked CGs for a specific conversation.
        /// </summary>
        public List<string> GetUnlockedCGs(string conversationId)
        {
            if (activeSessions.TryGetValue(conversationId, out var session))
                return session.state?.unlockedCGs ?? new List<string>();

            return new List<string>();
        }

        /// <summary>
        /// Gets all unlocked CGs across all conversations.
        /// </summary>
        public List<string> GetAllUnlockedCGs()
        {
            var allCGs = new List<string>();

            foreach (var kvp in activeSessions)
            {
                if (kvp.Value.state?.unlockedCGs != null)
                    allCGs.AddRange(kvp.Value.state.unlockedCGs);
            }

            return allCGs;
        }

        /// <summary>
        /// Checks if a specific CG is unlocked in a conversation.
        /// </summary>
        public bool IsCGUnlocked(string conversationId, string cgPath)
        {
            if (activeSessions.TryGetValue(conversationId, out var session))
                return session.state?.unlockedCGs?.Contains(cgPath) ?? false;

            return false;
        }

        // ═══════════════════════════════════════════════════════════
        // ░ EVENT SUBSCRIPTIONS (for auto-save)
        // ═══════════════════════════════════════════════════════════

        private void SubscribeToExecutorEvents(DialogueExecutor executor)
        {
            executor.OnMessagesReady += OnExecutorMessagesReady;
            executor.OnChoicesReady += OnExecutorChoicesReady;
            executor.OnPauseReached += OnExecutorPauseReached;
            executor.OnConversationEnd += OnExecutorConversationEnd;
            executor.OnChapterChange += OnExecutorChapterChange;

            Debug.Log("[ConversationManager] Subscribed to executor events for auto-save");
        }

        private void UnsubscribeFromExecutorEvents(DialogueExecutor executor)
        {
            if (executor == null) return;

            executor.OnMessagesReady -= OnExecutorMessagesReady;
            executor.OnChoicesReady -= OnExecutorChoicesReady;
            executor.OnPauseReached -= OnExecutorPauseReached;
            executor.OnConversationEnd -= OnExecutorConversationEnd;
            executor.OnChapterChange -= OnExecutorChapterChange;

            Debug.Log("[ConversationManager] Unsubscribed from executor events");
        }

        // ═══════════════════════════════════════════════════════════
        // ░ EXECUTOR EVENT HANDLERS (auto-save triggers)
        // ═══════════════════════════════════════════════════════════

        private void OnExecutorMessagesReady(List<MessageData> messages)
        {
            SaveCurrentConversation();
        }

        private void OnExecutorChoicesReady(List<ChoiceData> choices)
        {
            SaveCurrentConversation();
        }

        private void OnExecutorPauseReached()
        {
            SaveCurrentConversation();
        }

        private void OnExecutorConversationEnd()
        {
            ForceSaveCurrentConversation();
        }

        private void OnExecutorChapterChange(string chapterName)
        {
            Debug.Log($"[ConversationManager] Chapter changed: {chapterName}");
            ForceSaveCurrentConversation();
        }

        // ═══════════════════════════════════════════════════════════
        // ░ SAVE/LOAD LOGIC
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Loads existing ConversationState from save, or creates a fresh one.
        /// Result is always immediately stored in activeSessions by the caller —
        /// it never floats unowned as it did with the old GetOrCreateState pattern.
        /// </summary>
        private ConversationState LoadOrCreateState(string conversationId, string characterName)
        {
            var existingState = callbacks?.LoadConversationState(conversationId);

            if (existingState != null)
            {
                Debug.Log($"[ConversationManager] Loaded existing state: {conversationId} " +
                         $"(Chapter: {existingState.currentChapterIndex}, Node: '{existingState.currentNodeName}')");
                return existingState;
            }

            var newState = new ConversationState(conversationId)
            {
                characterName = characterName
            };

            Debug.Log($"[ConversationManager] Created new state: {conversationId}");
            return newState;
        }

        /// <summary>
        /// Save conversation state with optional throttling.
        /// </summary>
        private void SaveConversationState(bool throttle)
        {
            if (throttle)
            {
                float timeSinceLastSave = Time.realtimeSinceStartup - lastSaveTime;

                if (timeSinceLastSave < SAVE_THROTTLE_DELAY)
                {
                    hasPendingSave = true;
                    return;
                }
            }

            ForceSaveGame();
        }

        /// <summary>
        /// Performs the actual save operation.
        /// </summary>
        private void ForceSaveGame()
        {
            if (string.IsNullOrEmpty(currentConversationId) ||
                !activeSessions.TryGetValue(currentConversationId, out var session))
                return;

            bool success = callbacks?.SaveConversationState(session.state) ?? false;

            if (success)
            {
                lastSaveTime = Time.realtimeSinceStartup;
                hasPendingSave = false;

                Debug.Log($"[ConversationManager] ✓ Saved: {currentConversationId} " +
                         $"(Node: '{session.state.currentNodeName}', Chapter: {session.state.currentChapterIndex})");
            }
            else
            {
                Debug.LogError($"[ConversationManager] ✗ Save failed for: {currentConversationId}");
            }
        }

        // ═══════════════════════════════════════════════════════════
        // ░ LIFECYCLE
        // ═══════════════════════════════════════════════════════════

        private void Update()
        {
            if (hasPendingSave)
            {
                float timeSinceLastSave = Time.realtimeSinceStartup - lastSaveTime;

                if (timeSinceLastSave >= SAVE_THROTTLE_DELAY)
                {
                    if (!string.IsNullOrEmpty(currentConversationId))
                    {
                        SaveConversationState(throttle: false);
                    }
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && CurrentExecutor != null && !string.IsNullOrEmpty(currentConversationId))
            {
                Debug.Log("[ConversationManager] App paused - force saving conversation");
                ForceSaveCurrentConversation();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && CurrentExecutor != null && !string.IsNullOrEmpty(currentConversationId))
            {
                Debug.Log("[ConversationManager] App lost focus - force saving conversation");
                ForceSaveCurrentConversation();
            }
        }

        private void OnApplicationQuit()
        {
            if (CurrentExecutor != null && !string.IsNullOrEmpty(currentConversationId))
            {
                Debug.Log("[ConversationManager] App quitting - force saving conversation");
                ForceSaveCurrentConversation();
            }
        }

        // ═══════════════════════════════════════════════════════════
        // ░ EDITOR TOOLS
        // ═══════════════════════════════════════════════════════════

#if UNITY_EDITOR
        [ContextMenu("Debug/Print Active Conversations")]
        private void DebugPrintActiveConversations()
        {
            Debug.Log("╔═══════════════ ACTIVE CONVERSATIONS ═══════════════╗");
            Debug.Log($"║ Current: {currentConversationId ?? "None"}");
            Debug.Log($"║ Active Sessions: {activeSessions.Count}");

            foreach (var kvp in activeSessions)
            {
                var state = kvp.Value.state;
                Debug.Log($"║   {kvp.Key}: Chapter {state.currentChapterIndex}, " +
                         $"Node '{state.currentNodeName}', Messages: {state.messageHistory.Count}");
            }

            Debug.Log("╚════════════════════════════════════════════════════╝");
        }

        [ContextMenu("Debug/Force Save Now")]
        private void DebugForceSave()
        {
            if (CurrentExecutor != null)
            {
                ForceSaveCurrentConversation();
                Debug.Log("✓ Force saved current conversation");
            }
            else
            {
                Debug.LogWarning("No active conversation to save");
            }
        }
#endif
    }
}