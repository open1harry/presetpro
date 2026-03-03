using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace PresetPro.Editor
{
    public sealed class PresetProCategoryData
    {
        public string folderName;
        public string folderPath;
        public string displayName;
        public List<PresetProPrefabData> prefabs = new List<PresetProPrefabData>();
    }

    public sealed class PresetProPrefabData
    {
        public string name;
        public string assetPath;
    }

    public static class PresetProDataScanner
    {
        public static List<PresetProCategoryData> GetCategories(PresetProSettingsAsset settings)
        {
            string root = PresetProPathUtility.NormalizeAssetFolderPath(settings != null ? settings.presetsRoot : PresetProPathUtility.DefaultPresetsRoot);
            if (!AssetDatabase.IsValidFolder(root))
            {
                return new List<PresetProCategoryData>();
            }

            string[] subfolders = AssetDatabase.GetSubFolders(root);
            var categories = new List<PresetProCategoryData>(subfolders.Length);
            for (int i = 0; i < subfolders.Length; i++)
            {
                string folderPath = subfolders[i];
                string folderName = Path.GetFileName(folderPath.Replace('\\', '/'));
                var category = new PresetProCategoryData
                {
                    folderName = folderName,
                    folderPath = folderPath,
                    displayName = settings != null ? settings.GetDisplayName(folderName) : folderName
                };

                string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
                for (int p = 0; p < prefabGuids.Length; p++)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[p]);
                    string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                    category.prefabs.Add(new PresetProPrefabData
                    {
                        name = prefabName,
                        assetPath = prefabPath
                    });
                }

                category.prefabs.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
                categories.Add(category);
            }

            categories.Sort((a, b) => string.CompareOrdinal(a.folderName, b.folderName));
            return categories;
        }
    }
}
