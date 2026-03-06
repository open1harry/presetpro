using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PresetPro.Editor
{
    public sealed class PresetProSettingsWindow : EditorWindow
    {
        private PresetProSettingsAsset _settings;
        private List<PresetProCategoryData> _categories = new List<PresetProCategoryData>();
        private Vector2 _scroll;

        public static void OpenWindowInternal(bool skipIntroCheck)
        {
            if (!skipIntroCheck && PresetProFirstRunState.TryOpenIntroIfNeeded())
            {
                return;
            }

            PresetProSettingsAsset settings = PresetProSettingsProvider.GetOrCreateSettings();
            var window = GetWindow<PresetProSettingsWindow>(false, PresetProLocalization.Choose(settings, "预设 Pro 设置", "Preset Pro Settings"));
            window.minSize = new Vector2(520f, 420f);
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            _settings = PresetProSettingsProvider.GetOrCreateSettings();
            _settings.presetsRoot = PresetProPathUtility.NormalizeAssetFolderPath(_settings.presetsRoot);
            _settings.SanitizeMenuSettings();
            PresetProPathUtility.EnsureAssetFolderExists(_settings.presetsRoot);
            _categories = PresetProDataScanner.GetCategories(_settings);
            PresetProSettingsProvider.SaveSettings(_settings);
            titleContent = new GUIContent(T("预设 Pro 设置", "Preset Pro Settings"));
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                LoadSettings();
            }

            EditorGUILayout.LabelField("Preset Pro", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                T(
                    "文件夹即分类，Prefab 即选项。将用户预设放到 Presets 目录后，点击“生成菜单”即可更新 GameObject 菜单入口。",
                    "Folder is category, prefab is option. Put user prefabs under Presets and click Generate Menu to update the GameObject menu entry."),
                MessageType.Info);
            EditorGUILayout.Space(4f);

            DrawLanguage();
            EditorGUILayout.Space(8f);
            DrawPresetsRoot();
            EditorGUILayout.Space(8f);
            DrawGameObjectMenuRoot();
            EditorGUILayout.Space(8f);
            DrawCategoryAliasMappings();
            EditorGUILayout.Space(8f);
            DrawActions();
        }

        private void DrawLanguage()
        {
            EditorGUILayout.LabelField(T("界面语言", "UI Language"), EditorStyles.boldLabel);
            int current = _settings.uiLanguage == PresetProLanguage.Chinese ? 0 : 1;
            string[] options = { "中文（默认）", "English" };
            EditorGUI.BeginChangeCheck();
            int next = EditorGUILayout.Popup(T("选择语言", "Language"), current, options);
            if (EditorGUI.EndChangeCheck())
            {
                _settings.uiLanguage = next == 0 ? PresetProLanguage.Chinese : PresetProLanguage.English;
                PresetProSettingsProvider.SaveSettings(_settings);
                titleContent = new GUIContent(T("预设 Pro 设置", "Preset Pro Settings"));
                PresetProMenuGenerator.GenerateAndRefresh(false);
            }
        }

        private void DrawPresetsRoot()
        {
            EditorGUILayout.LabelField(T("预设根目录", "Preset Root"), EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField(T("预设文件夹", "Presets Folder"), _settings.presetsRoot);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(T("浏览文件夹...", "Browse Folder..."), GUILayout.Width(150f)))
                {
                    string selected = EditorUtility.OpenFolderPanel(
                        T("选择 Presets 目录", "Choose Presets Folder"),
                        PresetProPathUtility.AssetPathToAbsolutePath(_settings.presetsRoot),
                        string.Empty);

                    if (!string.IsNullOrEmpty(selected))
                    {
                        string assetPath = PresetProPathUtility.AbsolutePathToAssetPath(selected);
                        if (!string.IsNullOrEmpty(assetPath) && PresetProPathUtility.IsAssetPathInsideProject(assetPath))
                        {
                            _settings.presetsRoot = assetPath;
                            PresetProPathUtility.EnsureAssetFolderExists(_settings.presetsRoot);
                            PresetProSettingsProvider.SaveSettings(_settings);
                            _categories = PresetProDataScanner.GetCategories(_settings);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Preset Pro", T("请选择 Assets 目录内的文件夹。", "Pick a folder inside project Assets."), "OK");
                        }
                    }
                }

                if (GUILayout.Button(T("打开文件夹", "Open Folder"), GUILayout.Width(140f)))
                {
                    string absolutePath = PresetProPathUtility.AssetPathToAbsolutePath(_settings.presetsRoot);
                    EditorUtility.RevealInFinder(absolutePath);
                }
            }
        }

        private void DrawGameObjectMenuRoot()
        {
            EditorGUILayout.LabelField(T("GameObject 菜单根节点", "GameObject Menu Root"), EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                T(
                    "用于生成 GameObject/<名称>/... 菜单路径。会自动移除斜杠，留空时回退为 Preset Pro。",
                    "Used for the generated GameObject/<name>/... menu path. Slashes are removed automatically, and an empty value falls back to Preset Pro."),
                MessageType.None);

            EditorGUI.BeginChangeCheck();
            string nextRoot = EditorGUILayout.TextField(T("菜单名称", "Menu Name"), _settings.gameObjectMenuRoot);
            if (EditorGUI.EndChangeCheck())
            {
                _settings.gameObjectMenuRoot = PresetProSettingsAsset.SanitizeGameObjectMenuRoot(nextRoot);
                PresetProSettingsProvider.SaveSettings(_settings);
            }
        }

        private void DrawCategoryAliasMappings()
        {
            EditorGUILayout.LabelField(T("分类别名映射", "Category Alias Mappings"), EditorStyles.boldLabel);
            if (_categories.Count == 0)
            {
                EditorGUILayout.HelpBox(T("当前 Presets 根目录下还没有分类文件夹。", "No category folder found under the presets root."), MessageType.None);
                return;
            }

            EditorGUILayout.HelpBox(
                T("可通过上下按钮调整分类排序，生成菜单时会按此顺序显示。", "Use the up and down buttons to reorder categories. Generated menus will follow this order."),
                MessageType.None);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MinHeight(180f));
            bool changed = false;
            bool reorderRequested = false;
            for (int i = 0; i < _categories.Count; i++)
            {
                PresetProCategoryData category = _categories[i];
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.IntField(i + 1, GUILayout.Width(44f));
                    }

                    EditorGUILayout.LabelField(category.folderName, GUILayout.Width(160f));
                    EditorGUI.BeginChangeCheck();
                    string alias = EditorGUILayout.TextField(_settings.GetAliasValue(category.folderName));
                    if (EditorGUI.EndChangeCheck())
                    {
                        _settings.SetAliasValue(category.folderName, alias);
                        changed = true;
                    }

                    using (new EditorGUI.DisabledScope(i == 0))
                    {
                        if (GUILayout.Button("↑", GUILayout.Width(28f)))
                        {
                            MoveCategory(i, i - 1);
                            reorderRequested = true;
                        }
                    }

                    using (new EditorGUI.DisabledScope(i >= _categories.Count - 1))
                    {
                        if (GUILayout.Button("↓", GUILayout.Width(28f)))
                        {
                            MoveCategory(i, i + 1);
                            reorderRequested = true;
                        }
                    }
                }

                if (reorderRequested)
                {
                    changed = true;
                    break;
                }
            }

            EditorGUILayout.EndScrollView();

            if (changed)
            {
                PresetProSettingsProvider.SaveSettings(_settings);
                _categories = PresetProDataScanner.GetCategories(_settings);
            }
        }

        private void MoveCategory(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || toIndex < 0 || fromIndex >= _categories.Count || toIndex >= _categories.Count || fromIndex == toIndex)
            {
                return;
            }

            PresetProCategoryData fromCategory = _categories[fromIndex];
            PresetProCategoryData toCategory = _categories[toIndex];
            int fromOrder = _settings.GetSortOrder(fromCategory.folderName);
            int toOrder = _settings.GetSortOrder(toCategory.folderName);
            _settings.SetSortOrder(fromCategory.folderName, toOrder);
            _settings.SetSortOrder(toCategory.folderName, fromOrder);
        }

        private void DrawActions()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(T("刷新分类", "Refresh Categories"), GUILayout.Height(28f)))
                {
                    _categories = PresetProDataScanner.GetCategories(_settings);
                }

                if (GUILayout.Button(T("生成菜单", "Generate Menu"), GUILayout.Height(28f)))
                {
                    PresetProMenuGenerator.GenerateAndRefresh(true);
                }
            }
        }

        private string T(string chinese, string english)
        {
            return PresetProLocalization.Choose(_settings, chinese, english);
        }
    }
}
