namespace PresetPro.Editor
{
    public static class PresetProLocalization
    {
        public static bool UseChinese()
        {
            return UseChinese(PresetProSettingsProvider.GetOrCreateSettings());
        }

        public static bool UseChinese(PresetProSettingsAsset settings)
        {
            return settings == null || settings.uiLanguage == PresetProLanguage.Chinese;
        }

        public static bool UseEnglish()
        {
            return !UseChinese();
        }

        public static bool UseEnglish(PresetProSettingsAsset settings)
        {
            return !UseChinese(settings);
        }

        public static string Choose(PresetProSettingsAsset settings, string chinese, string english)
        {
            return UseChinese(settings) ? chinese : english;
        }
    }
}
