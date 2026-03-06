using System;
using System.Collections.Generic;
using UnityEngine;

namespace PresetPro.Editor
{
    public enum PresetProLanguage
    {
        Chinese = 0,
        English = 1
    }

    [Serializable]
    public sealed class PresetProCategoryAlias
    {
        public string folderName = string.Empty;
        public string displayName = string.Empty;
        public int sortOrder;
    }

    public sealed class PresetProSettingsAsset : ScriptableObject
    {
        public const string DefaultGameObjectMenuRoot = "Preset Pro";

        public string presetsRoot = PresetProPathUtility.DefaultPresetsRoot;
        public PresetProLanguage uiLanguage = PresetProLanguage.Chinese;
        public string gameObjectMenuRoot = DefaultGameObjectMenuRoot;
        public List<PresetProCategoryAlias> categoryAliases = new List<PresetProCategoryAlias>();

        public string GetDisplayName(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return folderName;
            }

            for (int i = 0; i < categoryAliases.Count; i++)
            {
                PresetProCategoryAlias alias = categoryAliases[i];
                if (alias != null && alias.folderName == folderName && !string.IsNullOrWhiteSpace(alias.displayName))
                {
                    return alias.displayName.Trim();
                }
            }

            return folderName;
        }

        public string GetAliasValue(string folderName)
        {
            PresetProCategoryAlias alias = GetCategoryAlias(folderName);
            return alias != null ? alias.displayName ?? string.Empty : string.Empty;
        }

        public int GetSortOrder(string folderName)
        {
            PresetProCategoryAlias alias = GetCategoryAlias(folderName);
            return alias != null ? alias.sortOrder : int.MaxValue;
        }

        public void SetAliasValue(string folderName, string displayName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return;
            }

            PresetProCategoryAlias alias = GetOrCreateCategoryAlias(folderName);
            alias.displayName = displayName ?? string.Empty;
        }

        public void SetSortOrder(string folderName, int sortOrder)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return;
            }

            PresetProCategoryAlias alias = GetOrCreateCategoryAlias(folderName);
            alias.sortOrder = sortOrder;
        }

        public void EnsureCategoryAliasOrders(IReadOnlyList<PresetProCategoryData> categories)
        {
            if (categories == null)
            {
                return;
            }

            for (int i = 0; i < categories.Count; i++)
            {
                PresetProCategoryData category = categories[i];
                if (category == null || string.IsNullOrEmpty(category.folderName))
                {
                    continue;
                }

                PresetProCategoryAlias alias = GetOrCreateCategoryAlias(category.folderName);
                if (alias.sortOrder == 0)
                {
                    alias.sortOrder = i + 1;
                }
            }
        }

        public string GetGameObjectMenuRoot()
        {
            return SanitizeGameObjectMenuRoot(gameObjectMenuRoot);
        }

        public void SanitizeMenuSettings()
        {
            gameObjectMenuRoot = SanitizeGameObjectMenuRoot(gameObjectMenuRoot);
        }

        private PresetProCategoryAlias GetCategoryAlias(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return null;
            }

            for (int i = 0; i < categoryAliases.Count; i++)
            {
                PresetProCategoryAlias alias = categoryAliases[i];
                if (alias != null && alias.folderName == folderName)
                {
                    return alias;
                }
            }

            return null;
        }

        private PresetProCategoryAlias GetOrCreateCategoryAlias(string folderName)
        {
            PresetProCategoryAlias alias = GetCategoryAlias(folderName);
            if (alias != null)
            {
                return alias;
            }

            alias = new PresetProCategoryAlias
            {
                folderName = folderName,
                displayName = string.Empty,
                sortOrder = categoryAliases.Count + 1
            };
            categoryAliases.Add(alias);
            return alias;
        }

        public static string SanitizeGameObjectMenuRoot(string value)
        {
            string sanitized = (value ?? string.Empty).Trim();
            sanitized = sanitized.Replace("/", " ").Replace("\\", " ");
            return string.IsNullOrWhiteSpace(sanitized) ? DefaultGameObjectMenuRoot : sanitized;
        }
    }
}
