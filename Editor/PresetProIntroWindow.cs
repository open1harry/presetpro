using UnityEditor;
using UnityEngine;

namespace PresetPro.Editor
{
    public sealed class PresetProIntroWindow : EditorWindow
    {
        private PresetProSettingsAsset _settings;

        public static void OpenWindow()
        {
            var window = GetWindow<PresetProIntroWindow>();
            window._settings = PresetProSettingsProvider.GetOrCreateSettings();
            window.titleContent = new GUIContent(window.T("工具介绍", "Introduction"));
            window.minSize = new Vector2(560f, 380f);
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

            string gameObjectMenuRoot = _settings.GetGameObjectMenuRoot();

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(T("欢迎使用 Preset Pro", "Welcome to Preset Pro"), EditorStyles.boldLabel);
            EditorGUILayout.Space(6f);
            EditorGUILayout.HelpBox(
                T(
                    "Preset Pro 主要用于把场景中常用的对象快速沉淀为 Prefab 预设，便于分类整理、复用和备份。\n\n" +
                    "菜单位置：\n" +
                    "- VFXSkill/预设 Pro/设置\n" +
                    "- VFXSkill/预设 Pro/快速添加\n" +
                    "- VFXSkill/预设 Pro/生成菜单\n\n" +
                    "使用方法：\n" +
                    "1. 打开“设置”，确认 Presets 根目录，并按需修改 GameObject 菜单根节点名称。\n" +
                    "2. 在 Presets 目录下按文件夹建立分类，并放入或保存 Prefab。\n" +
                    "3. 在 Hierarchy 选中对象后按 Shift+V，可快速保存为预设。\n" +
                    "4. 点击“生成菜单”，将预设同步到 GameObject/" + gameObjectMenuRoot + "/... 菜单。",
                    "Preset Pro is used to turn commonly reused scene objects into prefab presets for organized reuse and backup.\n\n" +
                    "Menu location:\n" +
                    "- VFXSkill/Preset Pro/Settings\n" +
                    "- VFXSkill/Preset Pro/Quick Add\n" +
                    "- VFXSkill/Preset Pro/Generate Menu\n\n" +
                    "How to use:\n" +
                    "1. Open Settings, confirm the Presets root folder, and adjust the GameObject menu root name if needed.\n" +
                    "2. Create category folders under Presets, then place or save prefabs into them.\n" +
                    "3. Select an object in Hierarchy and press Shift+V to save it as a preset quickly.\n" +
                    "4. Click Generate Menu to sync presets into GameObject/" + gameObjectMenuRoot + "/... ."),
                MessageType.Info);

            EditorGUILayout.Space(10f);
            if (GUILayout.Button(T("打开设置", "Open Settings"), GUILayout.Height(30f)))
            {
                PresetProFirstRunState.MarkIntroShown();
                PresetProSettingsWindow.OpenWindowInternal(true);
                Close();
            }

            EditorGUILayout.Space(8f);
            if (GUILayout.Button(T("关闭", "Close"), GUILayout.Height(26f)))
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
