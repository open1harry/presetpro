using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PresetPro.Editor
{
    public sealed class PresetProQuickAddWindow : EditorWindow
    {
        private PresetProSettingsAsset _settings;
        private List<PresetProCategoryData> _categories = new List<PresetProCategoryData>();
        private GameObject _sourceObject;
        private int _categoryIndex;
        private string _newCategoryName = string.Empty;
        private string _presetName = string.Empty;

        public static void OpenForCurrentSelection()
        {
            GameObject selected = Selection.activeGameObject;
            PresetProSettingsAsset settings = PresetProSettingsProvider.GetOrCreateSettings();
            if (selected == null || EditorUtility.IsPersistent(selected))
            {
                EditorUtility.DisplayDialog(
                    "Preset Pro",
                    PresetProLocalization.Choose(settings, "\u8bf7\u5148\u5728 Hierarchy \u4e2d\u9009\u4e2d\u4e00\u4e2a\u573a\u666f\u5bf9\u8c61\u3002", "Select a scene object in Hierarchy first."),
                    "OK");
                return;
            }

            var window = CreateInstance<PresetProQuickAddWindow>();
            window._sourceObject = selected;
            window._presetName = selected.name;
            window.Initialize();
            window.titleContent = new GUIContent(window.T("\u5feb\u901f\u6dfb\u52a0\u9884\u8bbe", "Preset Pro Quick Add"));
            window.minSize = new Vector2(420f, 220f);
            window.maxSize = new Vector2(680f, 280f);
            window.position = GetCenteredRect(500f, 250f);
            window.ShowUtility();
            window.Focus();
        }

        private void Initialize()
        {
            _settings = PresetProSettingsProvider.GetOrCreateSettings();
            _settings.presetsRoot = PresetProPathUtility.NormalizeAssetFolderPath(_settings.presetsRoot);
            PresetProPathUtility.EnsureAssetFolderExists(_settings.presetsRoot);
            _categories = PresetProDataScanner.GetCategories(_settings);
            _categoryIndex = 0;
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                Initialize();
            }

            EditorGUILayout.LabelField(T("\u5feb\u901f\u6dfb\u52a0\u9884\u8bbe", "Quick Add Preset"), EditorStyles.boldLabel);
            EditorGUILayout.Space(4f);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField(T("\u6765\u6e90\u5bf9\u8c61", "Source"), _sourceObject, typeof(GameObject), true);
            }

            string[] options = BuildCategoryOptions();
            _categoryIndex = Mathf.Clamp(_categoryIndex, 0, Mathf.Max(0, options.Length - 1));
            _categoryIndex = EditorGUILayout.Popup(T("\u5206\u7c7b", "Category"), _categoryIndex, options);

            if (IsCreateNewCategorySelected())
            {
                _newCategoryName = EditorGUILayout.TextField(T("\u65b0\u5206\u7c7b\u540d\u79f0", "New Category"), _newCategoryName);
            }

            _presetName = EditorGUILayout.TextField(T("\u9884\u8bbe\u540d\u79f0", "Preset Name"), _presetName);

            EditorGUILayout.Space(12f);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(T("\u53d6\u6d88", "Cancel"), GUILayout.Width(110f), GUILayout.Height(28f)))
                {
                    Close();
                }

                using (new EditorGUI.DisabledScope(!CanSave()))
                {
                    if (GUILayout.Button(T("\u4fdd\u5b58", "Save"), GUILayout.Width(110f), GUILayout.Height(28f)))
                    {
                        SavePreset();
                    }
                }
            }
        }

        private string[] BuildCategoryOptions()
        {
            var options = new List<string>(_categories.Count + 1);
            for (int i = 0; i < _categories.Count; i++)
            {
                options.Add(_categories[i].folderName);
            }

            options.Add(T("\u3010\u65b0\u5efa\u5206\u7c7b...\u3011", "[Create New Category...]"));
            return options.ToArray();
        }

        private bool IsCreateNewCategorySelected()
        {
            return _categoryIndex >= _categories.Count;
        }

        private bool CanSave()
        {
            if (_sourceObject == null || EditorUtility.IsPersistent(_sourceObject))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(_presetName))
            {
                return false;
            }

            if (IsCreateNewCategorySelected() && string.IsNullOrWhiteSpace(_newCategoryName))
            {
                return false;
            }

            return true;
        }

        private void SavePreset()
        {
            string categoryName = ResolveCategoryName();
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                EditorUtility.DisplayDialog("Preset Pro", T("\u5206\u7c7b\u540d\u4e0d\u80fd\u4e3a\u7a7a\u3002", "Category name cannot be empty."), "OK");
                return;
            }

            string sanitizedCategory = PresetProNameUtility.SanitizeFolderName(categoryName, "NewCategory");
            string sanitizedPrefabName = PresetProNameUtility.SanitizeFileName(_presetName, _sourceObject != null ? _sourceObject.name : "Preset");
            string rootPath = PresetProPathUtility.NormalizeAssetFolderPath(_settings.presetsRoot);
            string categoryPath = rootPath + "/" + sanitizedCategory;
            PresetProPathUtility.EnsureAssetFolderExists(categoryPath);

            string prefabPath = categoryPath + "/" + sanitizedPrefabName + ".prefab";
            UnityEngine.Object existing = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabPath);
            if (existing != null)
            {
                string message = T(
                    "\u5f53\u524d\u5206\u7c7b\u5df2\u5b58\u5728\u540c\u540d\u9884\u8bbe\uff1a" + sanitizedPrefabName + ".prefab\n" +
                    "\u5206\u7c7b\uff1a" + sanitizedCategory,
                    "A preset already exists: " + sanitizedPrefabName + ".prefab\nCategory: " + sanitizedCategory);

                int choice = EditorUtility.DisplayDialogComplex(
                    "Preset Pro",
                    message,
                    T("\u8986\u76d6", "Overwrite"),
                    T("\u53d6\u6d88", "Cancel"),
                    T("\u91cd\u547d\u540d", "Rename"));

                if (choice == 1)
                {
                    return;
                }

                if (choice == 2)
                {
                    prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
                }
            }

            bool success;
            PrefabUtility.SaveAsPrefabAssetAndConnect(_sourceObject, prefabPath, InteractionMode.UserAction, out success);
            if (!success)
            {
                EditorUtility.DisplayDialog("Preset Pro", T("\u4fdd\u5b58\u9884\u8bbe\u5931\u8d25\uff0c\u8bf7\u67e5\u770b Console\u3002", "Failed to save prefab. Check Console."), "OK");
                return;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            PresetProMenuGenerator.GenerateAndRefresh(false);
            Close();
            Debug.Log(T("\u9884\u8bbe Pro: \u9884\u8bbe\u4fdd\u5b58\u6210\u529f -> ", "Preset Pro: saved preset -> ") + prefabPath);
        }

        private string ResolveCategoryName()
        {
            if (IsCreateNewCategorySelected())
            {
                return _newCategoryName.Trim();
            }

            if (_categories.Count == 0 || _categoryIndex < 0 || _categoryIndex >= _categories.Count)
            {
                return string.Empty;
            }

            return _categories[_categoryIndex].folderName;
        }

        private string T(string chinese, string english)
        {
            return PresetProLocalization.Choose(_settings, chinese, english);
        }

        private static Rect GetCenteredRect(float width, float height)
        {
            Rect main = GetMainWindowRect();
            float x = main.x + (main.width - width) * 0.5f;
            float y = main.y + (main.height - height) * 0.5f;
            return new Rect(x, y, width, height);
        }

        private static Rect GetMainWindowRect()
        {
            Type containerType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ContainerWindow");
            if (containerType == null)
            {
                return new Rect(80f, 80f, 1200f, 800f);
            }

            PropertyInfo positionProperty = containerType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo showModeField = containerType.GetField("m_ShowMode", BindingFlags.NonPublic | BindingFlags.Instance);
            if (positionProperty == null || showModeField == null)
            {
                return new Rect(80f, 80f, 1200f, 800f);
            }

            UnityEngine.Object[] windows = Resources.FindObjectsOfTypeAll(containerType);
            for (int i = 0; i < windows.Length; i++)
            {
                object window = windows[i];
                int showMode = (int)showModeField.GetValue(window);
                if (showMode == 4)
                {
                    return (Rect)positionProperty.GetValue(window, null);
                }
            }

            return new Rect(80f, 80f, 1200f, 800f);
        }
    }
}
