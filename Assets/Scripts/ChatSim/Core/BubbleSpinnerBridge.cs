// ════════════════════════════════════════════════════════════════════════
// Assets/Scripts/ChatSim/Core/BubbleSpinnerBridge.cs
// ════════════════════════════════════════════════════════════════════════

using UnityEngine;
using BubbleSpinner.Core;
using BubbleSpinner.Data;
using ChatSim.Data;

namespace ChatSim.Core
{
    /// <summary>
    /// Bridge class that implements IBubbleSpinnerCallbacks to connect BubbleSpinner's conversation system
    /// with ChatSim's game-specific logic.
    /// Handles saving/loading conversation state using ChatSim's SaveManager and triggers
    /// ChatSim events when conversations start, end, or when CGs are unlocked.
    /// </summary>
    public class BubbleSpinnerBridge : IBubbleSpinnerCallbacks
    {
        private ConversationManager _conversationManager;

        // ── PHASE 2 FIX (Bridge disk-read caching) ──────────────────────────
        // Previously, every SaveConversationState and LoadConversationState call
        // did a full disk read via GameBootstrap.Save.LoadGame() before mutating
        // and writing back. On mobile, this caused a disk read + write per message
        // batch — visible as hitches during active conversation.
        //
        // Fix: cache the SaveData in memory after the first load. All subsequent
        // reads and writes operate on the cached object. The cache is:
        //   - Populated lazily on first access via GetSaveData()
        //   - Invalidated (nulled) in Cleanup() on bridge teardown
        //   - Invalidated in OnCharacterStoryReset() so the next access re-reads
        //     the post-reset state from disk cleanly
        // ────────────────────────────────────────────────────────────────────
        private SaveData _cachedSaveData;

        public BubbleSpinnerBridge(ConversationManager conversationManager)
        {
            _conversationManager = conversationManager;
            GameEvents.OnCharacterStoryReset += OnCharacterStoryReset;
        }

        // ═══════════════════════════════════════════════════════════
        // ░ CACHE ACCESS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Returns the cached SaveData, loading from disk on first access.
        /// All save/load operations go through this instead of calling LoadGame() directly.
        /// </summary>
        private SaveData GetSaveData()
        {
            if (_cachedSaveData == null)
            {
                _cachedSaveData = GameBootstrap.Save.LoadGame() ?? GameBootstrap.Save.CreateNewSave();
                Debug.Log("[BubbleSpinnerBridge] SaveData loaded into cache");
            }

            return _cachedSaveData;
        }

        // ═══════════════════════════════════════════════════════════
        // ░ SAVE/LOAD CALLBACKS
        // ═══════════════════════════════════════════════════════════

        public bool SaveConversationState(ConversationState state)
        {
            if (state == null)
            {
                Debug.LogError("[BubbleSpinnerBridge] Cannot save null state");
                return false;
            }

            var saveData = GetSaveData();

            // Find and replace existing state, or add new one
            var index = saveData.conversationStates.FindIndex(c => c.conversationId == state.conversationId);

            if (index >= 0)
            {
                saveData.conversationStates[index] = state;
            }
            else
            {
                saveData.conversationStates.Add(state);
            }

            // Write to disk — cache already reflects the mutation above
            bool success = GameBootstrap.Save.SaveGame(saveData);

            if (success)
            {
                Debug.Log($"[BubbleSpinnerBridge] ✓ Saved conversation: {state.conversationId}");
            }
            else
            {
                Debug.LogError($"[BubbleSpinnerBridge] ✗ Failed to save: {state.conversationId}");
            }

            return success;
        }

        public ConversationState LoadConversationState(string conversationId)
        {
            var saveData = GetSaveData();

            var state = saveData.conversationStates.Find(c => c.conversationId == conversationId);

            if (state != null)
            {
                Debug.Log($"[BubbleSpinnerBridge] ✓ Loaded conversation: {conversationId}");
            }
            else
            {
                Debug.Log($"[BubbleSpinnerBridge] No saved state for: {conversationId}");
            }

            return state;
        }

        public void DeleteConversationState(string conversationId)
        {
            var saveData = GetSaveData();

            int removed = saveData.conversationStates.RemoveAll(c => c.conversationId == conversationId);

            if (removed > 0)
            {
                GameBootstrap.Save.SaveGame(saveData);
                Debug.Log($"[BubbleSpinnerBridge] ✓ Deleted conversation: {conversationId}");
            }
            else
            {
                Debug.LogWarning($"[BubbleSpinnerBridge] No state found to delete: {conversationId}");
            }
        }

        // ═══════════════════════════════════════════════════════════
        // ░ EVENT CALLBACKS (bridge to GameEvents)
        // ═══════════════════════════════════════════════════════════

        public void OnConversationStarted(string conversationId)
        {
            GameEvents.TriggerConversationStarted(conversationId);
        }

        public void OnCGUnlocked(string cgAddressableKey)
        {
            GameEvents.TriggerCGUnlocked(cgAddressableKey);
        }

        public void OnConversationEnded(string conversationId)
        {
            Debug.Log($"[BubbleSpinnerBridge] Conversation ended: {conversationId}");
        }

        public void OnChapterChanged(string conversationId, int chapterIndex, string chapterName)
        {
            Debug.Log($"[BubbleSpinnerBridge] Chapter changed: {conversationId} - {chapterName}");
        }

        private void OnCharacterStoryReset(string conversationId)
        {
            Debug.Log($"[BubbleSpinnerBridge] Story reset — invalidating cache and evicting: {conversationId}");

            // Invalidate cache so the next access re-reads the post-reset state from disk
            _cachedSaveData = null;

            _conversationManager?.EvictConversationCache(conversationId);
        }

        public void Cleanup()
        {
            GameEvents.OnCharacterStoryReset -= OnCharacterStoryReset;
            _cachedSaveData = null;
            _conversationManager = null;
        }
    }
}