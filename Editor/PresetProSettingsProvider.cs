using UnityEditor;
using UnityEngine;

namespace PresetPro.Editor
{
    public static class PresetProSettingsProvider
    {
        public static PresetProSettingsAsset GetOrCreateSettings()
        {
            PresetProSettingsAsset settings = AssetDatabase.LoadAssetAtPath<PresetProSettingsAsset>(PresetProPathUtility.SettingsAssetPath);
            if (settings != null)
            {
                settings.presetsRoot = PresetProPathUtility.NormalizeAssetFolderPath(settings.presetsRoot);
                settings.SanitizeMenuSettings();
                return settings;
            }

            PresetProPathUtility.EnsureAssetFolderExists("Assets/PresetPro");
            settings = ScriptableObject.CreateInstance<PresetProSettingsAsset>();
            settings.presetsRoot = PresetProPathUtility.DefaultPresetsRoot;
            settings.SanitizeMenuSettings();
            AssetDatabase.CreateAsset(settings, PresetProPathUtility.SettingsAssetPath);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }

        public static void SaveSettings(PresetProSettingsAsset settings)
        {
            if (settings == null)
            {
                return;
            }

            settings.presetsRoot = PresetProPathUtility.NormalizeAssetFolderPath(settings.presetsRoot);
            settings.SanitizeMenuSettings();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }
}
