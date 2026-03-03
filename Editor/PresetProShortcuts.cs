using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace PresetPro.Editor
{
    public static class PresetProShortcuts
    {
        [Shortcut("Preset Pro/\u5feb\u901f\u6dfb\u52a0\u9884\u8bbe", KeyCode.V, ShortcutModifiers.Shift)]
        private static void QuickAddPresetShortcut()
        {
            if (Selection.activeGameObject == null || EditorUtility.IsPersistent(Selection.activeGameObject))
            {
                PresetProSettingsAsset settings = PresetProSettingsProvider.GetOrCreateSettings();
                Debug.LogWarning(PresetProLocalization.Choose(
                    settings,
                    "\u9884\u8bbe Pro: \u8bf7\u5148\u5728 Hierarchy \u4e2d\u9009\u62e9\u4e00\u4e2a\u573a\u666f\u5bf9\u8c61\u3002",
                    "Preset Pro: select a scene GameObject in Hierarchy first."));
                return;
            }

            PresetProQuickAddWindow.OpenForCurrentSelection();
        }
    }
}
