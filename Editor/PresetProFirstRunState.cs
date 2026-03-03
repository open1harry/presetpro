using UnityEditor;
using UnityEngine;

namespace PresetPro.Editor
{
    public static class PresetProFirstRunState
    {
        private const string IntroShownKeyPrefix = "PresetPro.IntroShown.";

        private static string IntroShownKey
        {
            get
            {
                string projectPath = Application.dataPath.Replace('\\', '/');
                return IntroShownKeyPrefix + projectPath;
            }
        }

        public static bool IsIntroShown()
        {
            return EditorPrefs.GetBool(IntroShownKey, false);
        }

        public static void MarkIntroShown()
        {
            EditorPrefs.SetBool(IntroShownKey, true);
        }

        public static bool TryOpenIntroIfNeeded()
        {
            if (IsIntroShown())
            {
                return false;
            }

            MarkIntroShown();
            PresetProIntroWindow.OpenWindow();
            return true;
        }
    }
}
