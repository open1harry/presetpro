namespace PresetPro.Editor
{
    public static class PresetProLocalization
    {
        public static bool UseChinese(PresetProSettingsAsset settings)
        {
            return settings == null || settings.uiLanguage == PresetProLanguage.Chinese;
        }

        public static string Choose(PresetProSettingsAsset settings, string chinese, string english)
        {
            return UseChinese(settings) ? chinese : english;
        }
    }
}
