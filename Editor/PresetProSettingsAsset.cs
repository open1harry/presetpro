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
    }

    public sealed class PresetProSettingsAsset : ScriptableObject
    {
        public string presetsRoot = PresetProPathUtility.DefaultPresetsRoot;
        public PresetProLanguage uiLanguage = PresetProLanguage.Chinese;
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
            if (string.IsNullOrEmpty(folderName))
            {
                return string.Empty;
            }

            for (int i = 0; i < categoryAliases.Count; i++)
            {
                PresetProCategoryAlias alias = categoryAliases[i];
                if (alias != null && alias.folderName == folderName)
                {
                    return alias.displayName ?? string.Empty;
                }
            }

            return string.Empty;
        }

        public void SetAliasValue(string folderName, string displayName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return;
            }

            for (int i = 0; i < categoryAliases.Count; i++)
            {
                PresetProCategoryAlias alias = categoryAliases[i];
                if (alias != null && alias.folderName == folderName)
                {
                    alias.displayName = displayName ?? string.Empty;
                    return;
                }
            }

            categoryAliases.Add(new PresetProCategoryAlias
            {
                folderName = folderName,
                displayName = displayName ?? string.Empty
            });
        }
    }
}
