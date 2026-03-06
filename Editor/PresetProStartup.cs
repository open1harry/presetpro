using UnityEditor;

namespace PresetPro.Editor
{
    [InitializeOnLoad]
    public static class PresetProStartup
    {
        static PresetProStartup()
        {
            EditorApplication.delayCall += TryOpenIntroOnStartup;
        }

        private static void TryOpenIntroOnStartup()
        {
            EditorApplication.delayCall -= TryOpenIntroOnStartup;
            PresetProFirstRunState.TryOpenIntroIfNeeded();
        }
    }
}
