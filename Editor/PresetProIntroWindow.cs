using UnityEditor;
using UnityEngine;

namespace PresetPro.Editor
{
    public sealed class PresetProIntroWindow : EditorWindow
    {
        private PresetProSettingsAsset _settings;

        [MenuItem("Tools/Preset Pro/Introduction", false, 1199)]
        public static void OpenWindow()
        {
            var window = GetWindow<PresetProIntroWindow>();
            window._settings = PresetProSettingsProvider.GetOrCreateSettings();
            window.titleContent = new GUIContent(window.T("\u5de5\u5177\u4ecb\u7ecd", "Introduction"));
            window.minSize = new Vector2(560f, 360f);
            window.Show();
        }

        private void OnEnable()
        {
            _settings = PresetProSettingsProvider.GetOrCreateSettings();
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                _settings = PresetProSettingsProvider.GetOrCreateSettings();
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(T("\u6b22\u8fce\u4f7f\u7528 Preset Pro", "Welcome to Preset Pro"), EditorStyles.boldLabel);
            EditorGUILayout.Space(6f);
            EditorGUILayout.HelpBox(
                T(
                    "\u8fd9\u662f\u4e00\u4e2a\u57fa\u4e8e Unity \u539f\u751f\u5de5\u4f5c\u6d41\u7684\u5feb\u901f\u9884\u8bbe\u5de5\u5177\u3002\n\n" +
                    "\u6838\u5fc3\u7406\u5ff5\uff1a\u6587\u4ef6\u5939\u5373\u5206\u7c7b\uff0cPrefab \u5373\u9009\u9879\u3002\n" +
                    "1. \u5728 Presets \u76ee\u5f55\u4e0b\u6309\u6587\u4ef6\u5939\u7ba1\u7406\u5206\u7c7b\n" +
                    "2. \u7528 Shift+V \u628a\u573a\u666f\u5bf9\u8c61\u5feb\u901f\u4fdd\u5b58\u4e3a\u9884\u8bbe\n" +
                    "3. \u70b9\u51fb\u201c\u751f\u6210/\u5237\u65b0\u83dc\u5355\u201d\u540e\uff0c\u9884\u8bbe\u4f1a\u51fa\u73b0\u5728 GameObject/Preset Pro",
                    "This tool extends Unity native workflow for fast hierarchy presets.\n\n" +
                    "Core idea: folder is category, prefab is option.\n" +
                    "1. Organize categories with folders under Presets\n" +
                    "2. Use Shift+V to save selected scene object as preset\n" +
                    "3. Click Generate/Refresh Menu to rebuild GameObject/Preset Pro entries"),
                MessageType.Info);

            EditorGUILayout.Space(10f);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(T("\u6253\u5f00\u8bbe\u7f6e", "Open Settings"), GUILayout.Height(30f)))
                {
                    PresetProFirstRunState.MarkIntroShown();
                    PresetProSettingsWindow.OpenWindowInternal(true);
                    Close();
                }

                if (GUILayout.Button(T("\u6253\u5f00\u5feb\u6377\u6dfb\u52a0", "Open Quick Add"), GUILayout.Height(30f)))
                {
                    PresetProFirstRunState.MarkIntroShown();
                    PresetProQuickAddWindow.OpenForCurrentSelection();
                }
            }

            EditorGUILayout.Space(8f);
            if (GUILayout.Button(T("\u5173\u95ed", "Close"), GUILayout.Height(26f)))
            {
                PresetProFirstRunState.MarkIntroShown();
                Close();
            }
        }

        private string T(string chinese, string english)
        {
            return PresetProLocalization.Choose(_settings, chinese, english);
        }
    }
}
