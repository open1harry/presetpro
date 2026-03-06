using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PresetPro.Editor
{
    public static class PresetProPathUtility
    {
        public const string DefaultPresetsRoot = "Assets/PresetPro/Presets";
        public const string SettingsAssetPath = "Assets/PresetPro/PresetProSettings.asset";
        public const string GeneratedMenuDirectory = "Assets/PresetPro/Editor/Generated";
        public const string GeneratedMenuScriptPath = GeneratedMenuDirectory + "/PresetProGeneratedMenu.cs";
        public const string GeneratedEditorMenuScriptPath = GeneratedMenuDirectory + "/PresetProGeneratedEditorMenu.cs";

        public static string NormalizeAssetFolderPath(string path)
        {
            string normalized = (path ?? string.Empty).Trim().Replace('\\', '/');
            if (string.IsNullOrEmpty(normalized))
            {
                return DefaultPresetsRoot;
            }

            while (normalized.EndsWith("/", StringComparison.Ordinal))
            {
                normalized = normalized.Substring(0, normalized.Length - 1);
            }

            return normalized;
        }

        public static string GetProjectRootAbsolutePath()
        {
            string assetsPath = Application.dataPath.Replace('\\', '/');
            int lastSlash = assetsPath.LastIndexOf('/');
            return lastSlash > 0 ? assetsPath.Substring(0, lastSlash) : assetsPath;
        }

        public static bool IsAssetPathInsideProject(string assetPath)
        {
            return NormalizeAssetFolderPath(assetPath).StartsWith("Assets", StringComparison.Ordinal);
        }

        public static string AssetPathToAbsolutePath(string assetPath)
        {
            string normalized = NormalizeAssetFolderPath(assetPath);
            return Path.Combine(GetProjectRootAbsolutePath(), normalized).Replace('\\', '/');
        }

        public static string AbsolutePathToAssetPath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                return string.Empty;
            }

            string projectRoot = GetProjectRootAbsolutePath().Replace('\\', '/');
            string normalized = absolutePath.Replace('\\', '/');

            if (!normalized.StartsWith(projectRoot, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            string relative = normalized.Substring(projectRoot.Length).TrimStart('/');
            return NormalizeAssetFolderPath(relative);
        }

        public static void EnsureAssetFolderExists(string folderPath)
        {
            string normalized = NormalizeAssetFolderPath(folderPath);
            if (!IsAssetPathInsideProject(normalized))
            {
                throw new InvalidOperationException("Preset Pro path must be under Assets.");
            }

            if (AssetDatabase.IsValidFolder(normalized))
            {
                return;
            }

            string[] parts = normalized.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }
    }
}
