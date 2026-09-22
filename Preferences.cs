using System;
using MelonLoader;
using MelonLoader.Preferences;
using Manager;
using UnityEngine;

namespace LongPressExit
{
    internal static class ModPreferences
    {
        private const string CategoryName = "LongPressExit";
        private static MelonPreferences_Category _category;
        private static MelonPreferences_Entry<string> _longPressButtonEntry;
        private static MelonPreferences_Entry<string> _trackSkipKeyEntry;
        private static MelonPreferences_Entry<string> _trackSkipAlternateKeyEntry;

        public static InputManager.ButtonSetting LongPressButton { get; private set; }
        public static KeyCode TrackSkipKey { get; private set; }
        public static KeyCode TrackSkipAlternateKey { get; private set; }

        public static void Initialize()
        {
            _category = MelonPreferences.CreateCategory(CategoryName, "Long Press Exit");
            _longPressButtonEntry = _category.CreateEntry(
                "LongPressButton",
                "Select",
                "Long-press button",
                "InputManager.ButtonSetting name. Default: Select.",
                false,
                false,
                null);
            _trackSkipKeyEntry = _category.CreateEntry(
                "TrackSkipKey",
                "Space",
                "TrackSkip key",
                "UnityEngine.KeyCode name. Default: Space.",
                false,
                false,
                null);
            _trackSkipAlternateKeyEntry = _category.CreateEntry(
                "TrackSkipAlternateKey",
                "KeypadPlus",
                "TrackSkip alternate key",
                "UnityEngine.KeyCode name. Default: KeypadPlus.",
                false,
                false,
                null);

            LongPressButton = ParseButton(_longPressButtonEntry.Value, InputManager.ButtonSetting.Select);
            TrackSkipKey = ParseKeyCode(_trackSkipKeyEntry.Value, KeyCode.Space);
            TrackSkipAlternateKey = ParseKeyCode(_trackSkipAlternateKeyEntry.Value, KeyCode.KeypadPlus);

            MelonPreferences.Save();
            MelonLogger.Msg("[LongPressExit] Preferences loaded: LongPressButton="
                + LongPressButton + ", TrackSkipKey=" + TrackSkipKey
                + ", TrackSkipAlternateKey=" + TrackSkipAlternateKey);
        }

        public static bool IsLongPressButtonHeld()
        {
            return InputManager.GetButtonPush(0, LongPressButton)
                || InputManager.GetButtonPush(1, LongPressButton);
        }

        public static bool IsTrackSkipKeyDown()
        {
            return DebugInput.GetKeyDown(TrackSkipKey)
                || DebugInput.GetKeyDown(TrackSkipAlternateKey)
                || Input.GetKeyDown(TrackSkipKey)
                || Input.GetKeyDown(TrackSkipAlternateKey);
        }

        private static InputManager.ButtonSetting ParseButton(string value, InputManager.ButtonSetting fallback)
        {
            InputManager.ButtonSetting parsed;
            if (Enum.TryParse(value, true, out parsed) && parsed != InputManager.ButtonSetting.End)
            {
                return parsed;
            }

            MelonLogger.Warning("[LongPressExit] Invalid LongPressButton '" + value + "'; using " + fallback + ".");
            return fallback;
        }

        private static KeyCode ParseKeyCode(string value, KeyCode fallback)
        {
            KeyCode parsed;
            if (Enum.TryParse(value, true, out parsed) && parsed != KeyCode.None)
            {
                return parsed;
            }

            MelonLogger.Warning("[LongPressExit] Invalid key '" + value + "'; using " + fallback + ".");
            return fallback;
        }
    }
}

