using System;
using System.Text;

namespace PresetPro.Editor
{
    public static class PresetProNameUtility
    {
        public static string SanitizeMethodName(string raw, string fallback)
        {
            string candidate = string.IsNullOrWhiteSpace(raw) ? fallback : raw;
            var builder = new StringBuilder(candidate.Length);
            for (int i = 0; i < candidate.Length; i++)
            {
                char c = candidate[i];
                bool isValid = char.IsLetterOrDigit(c) || c == '_';
                builder.Append(isValid ? c : '_');
            }

            string result = builder.ToString().Trim('_');
            if (string.IsNullOrEmpty(result))
            {
                result = fallback;
            }

            if (!char.IsLetter(result[0]) && result[0] != '_')
            {
                result = "_" + result;
            }

            return result;
        }

        public static string SanitizeFileName(string raw, string fallback)
        {
            string candidate = string.IsNullOrWhiteSpace(raw) ? fallback : raw.Trim();
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(candidate.Length);
            for (int i = 0; i < candidate.Length; i++)
            {
                char c = candidate[i];
                bool invalid = Array.IndexOf(invalidChars, c) >= 0;
                builder.Append(invalid ? '_' : c);
            }

            string value = builder.ToString().Trim();
            return string.IsNullOrEmpty(value) ? fallback : value;
        }

        public static string SanitizeFolderName(string raw, string fallback)
        {
            return SanitizeFileName(raw, fallback).Replace("/", "_").Replace("\\", "_");
        }

        public static string SanitizeMenuLabel(string raw, string fallback)
        {
            string value = string.IsNullOrWhiteSpace(raw) ? fallback : raw.Trim();
            value = value.Replace("/", "_");
            return string.IsNullOrEmpty(value) ? fallback : value;
        }

        public static string EscapeCSharpString(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
