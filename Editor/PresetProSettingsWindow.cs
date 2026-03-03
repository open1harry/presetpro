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

        [MenuItem("Tools/Preset Pro/Settings", false, 1200)]
        public static void OpenWindow()
        {
            OpenWindowInternal(false);
        }

        internal static void OpenWindowInternal(bool skipIntroCheck)
        {
            if (!skipIntroCheck && PresetProFirstRunState.TryOpenIntroIfNeeded())
            {
                return;
            }

            PresetProSettingsAsset settings = PresetProSettingsProvider.GetOrCreateSettings();
            var window = GetWindow<PresetProSettingsWindow>(false, PresetProLocalization.Choose(settings, "\u9884\u8bbe Pro \u8bbe\u7f6e", "Preset Pro Settings"));
            window.minSize = new Vector2(520f, 380f);
            window.Show();
        }

        [MenuItem("Tools/Preset Pro/Quick Add", false, 1201)]
        public static void OpenQuickAdd()
        {
            PresetProQuickAddWindow.OpenForCurrentSelection();
        }

        [MenuItem("Tools/Preset Pro/Generate Refresh Menu", false, 1202)]
        public static void GenerateMenu()
        {
            PresetProMenuGenerator.GenerateAndRefresh(true);
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            _settings = PresetProSettingsProvider.GetOrCreateSettings();
            _settings.presetsRoot = PresetProPathUtility.NormalizeAssetFolderPath(_settings.presetsRoot);
            PresetProPathUtility.EnsureAssetFolderExists(_settings.presetsRoot);
            _categories = PresetProDataScanner.GetCategories(_settings);
            PresetProSettingsProvider.SaveSettings(_settings);
            titleContent = new GUIContent(T("\u9884\u8bbe Pro \u8bbe\u7f6e", "Preset Pro Settings"));
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
                    "\u6587\u4ef6\u5939\u5373\u5206\u7c7b\uff0cPrefab \u5373\u9009\u9879\u3002\u5c06\u7528\u6237\u9884\u8bbe\u653e\u5230 Presets \u76ee\u5f55\u540e\uff0c\u70b9\u51fb\"\u751f\u6210/\u5237\u65b0\u83dc\u5355\"\u5373\u53ef\u66f4\u65b0\u53f3\u952e\u83dc\u5355\u3002",
                    "Folder is category, prefab is option. Put user prefabs under Presets and click Generate/Refresh Menu."),
                MessageType.Info);
            EditorGUILayout.Space(4f);

            DrawLanguage();
            EditorGUILayout.Space(8f);
            DrawPresetsRoot();
            EditorGUILayout.Space(8f);
            DrawCategoryAliasMappings();
            EditorGUILayout.Space(8f);
            DrawActions();
        }

        private void DrawLanguage()
        {
            EditorGUILayout.LabelField(T("\u754c\u9762\u8bed\u8a00", "UI Language"), EditorStyles.boldLabel);
            int current = _settings.uiLanguage == PresetProLanguage.Chinese ? 0 : 1;
            string[] options = { "\u4e2d\u6587\uff08\u9ed8\u8ba4\uff09", "English" };
            EditorGUI.BeginChangeCheck();
            int next = EditorGUILayout.Popup(T("\u9009\u62e9\u8bed\u8a00", "Language"), current, options);
            if (EditorGUI.EndChangeCheck())
            {
                _settings.uiLanguage = next == 0 ? PresetProLanguage.Chinese : PresetProLanguage.English;
                PresetProSettingsProvider.SaveSettings(_settings);
                titleContent = new GUIContent(T("\u9884\u8bbe Pro \u8bbe\u7f6e", "Preset Pro Settings"));
            }
        }

        private void DrawPresetsRoot()
        {
            EditorGUILayout.LabelField(T("\u9884\u8bbe\u6839\u76ee\u5f55", "Preset Root"), EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            string root = EditorGUILayout.TextField(T("\u9884\u8bbe\u6587\u4ef6\u5939", "Presets Folder"), _settings.presetsRoot);
            if (EditorGUI.EndChangeCheck())
            {
                _settings.presetsRoot = PresetProPathUtility.NormalizeAssetFolderPath(root);
                if (!PresetProPathUtility.IsAssetPathInsideProject(_settings.presetsRoot))
                {
                    EditorUtility.DisplayDialog("Preset Pro", T("\u8def\u5f84\u5fc5\u987b\u4f4d\u4e8e Assets \u76ee\u5f55\u4e0b\u3002", "Path must stay under Assets."), "OK");
                    _settings.presetsRoot = PresetProPathUtility.DefaultPresetsRoot;
                }

                PresetProPathUtility.EnsureAssetFolderExists(_settings.presetsRoot);
                PresetProSettingsProvider.SaveSettings(_settings);
                _categories = PresetProDataScanner.GetCategories(_settings);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(T("\u6d4f\u89c8\u6587\u4ef6\u5939...", "Browse Folder..."), GUILayout.Width(150f)))
                {
                    string selected = EditorUtility.OpenFolderPanel(
                        T("\u9009\u62e9 Presets \u76ee\u5f55", "Choose Presets Folder"),
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
                            EditorUtility.DisplayDialog("Preset Pro", T("\u8bf7\u9009\u62e9 Assets \u76ee\u5f55\u5185\u7684\u6587\u4ef6\u5939\u3002", "Pick a folder inside project Assets."), "OK");
                        }
                    }
                }

                if (GUILayout.Button(T("\u6253\u5f00\u6587\u4ef6\u5939", "Open Folder"), GUILayout.Width(140f)))
                {
                    string absolutePath = PresetProPathUtility.AssetPathToAbsolutePath(_settings.presetsRoot);
                    EditorUtility.RevealInFinder(absolutePath);
                }
            }
        }

        private void DrawCategoryAliasMappings()
        {
            EditorGUILayout.LabelField(T("\u5206\u7c7b\u522b\u540d\u6620\u5c04", "Category Alias Mappings"), EditorStyles.boldLabel);
            if (_categories.Count == 0)
            {
                EditorGUILayout.HelpBox(T("\u5f53\u524d Presets \u6839\u76ee\u5f55\u4e0b\u8fd8\u6ca1\u6709\u5206\u7c7b\u6587\u4ef6\u5939\u3002", "No category folder found under the presets root."), MessageType.None);
                return;
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MinHeight(140f));
            bool changed = false;
            for (int i = 0; i < _categories.Count; i++)
            {
                PresetProCategoryData category = _categories[i];
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(category.folderName, GUILayout.Width(180f));
                    EditorGUI.BeginChangeCheck();
                    string alias = EditorGUILayout.TextField(_settings.GetAliasValue(category.folderName));
                    if (EditorGUI.EndChangeCheck())
                    {
                        _settings.SetAliasValue(category.folderName, alias);
                        changed = true;
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            if (changed)
            {
                PresetProSettingsProvider.SaveSettings(_settings);
                _categories = PresetProDataScanner.GetCategories(_settings);
            }
        }

        private void DrawActions()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(T("\u5237\u65b0\u5206\u7c7b", "Refresh Categories"), GUILayout.Height(28f)))
                {
                    _categories = PresetProDataScanner.GetCategories(_settings);
                }

                if (GUILayout.Button(T("\u751f\u6210/\u5237\u65b0\u83dc\u5355", "Generate/Refresh Menu"), GUILayout.Height(28f)))
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
