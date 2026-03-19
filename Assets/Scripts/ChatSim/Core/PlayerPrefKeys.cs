// ════════════════════════════════════════════════════════════════════════
// Assets/Scripts/ChatSim/Core/PlayerPrefKeys.cs
// ════════════════════════════════════════════════════════════════════════

namespace ChatSim.Core
{
    /// <summary>
    /// Shared PlayerPrefs key constants and default values.
    /// Always reference these instead of raw strings.
    /// </summary>
    public static class PlayerPrefKeys
    {
        public const string FastMode    = "ChatFastMode";
        public const string TextSize    = "ChatTextSize";

        public const float DefaultTextSize = 48f;
        public const int   DefaultFastMode = 0;
    }
}