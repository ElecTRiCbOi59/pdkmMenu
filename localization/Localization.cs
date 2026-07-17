using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;




namespace pdkmMenu
{
    internal static class Localization
    {
        private static Dictionary<LKey, string> _activeTranslations = new();
        private static List<string> _availableLanguages = new();
        private static string _currentLanguage = "en";
        private static bool _initialized;

        public static string CurrentLanguage => _currentLanguage;

        public static void Initialize()
        {
            if (Plugin.MenuSettings == null) return;
            LoadLanguage(Plugin.MenuSettings.Language.Value);
        }

        /// <summary>
        /// High-performance lookup. Enum keys (ints) are extremely fast in OnGUI.
        /// </summary>
        public static string T(LKey key)
        {
            if (!_initialized) Initialize();
            return _activeTranslations.TryGetValue(key, out var val) ? val : key.ToString();
        }

        private static void LoadLanguage(string language)
        {
            _activeTranslations.Clear();
            _currentLanguage = language.ToLowerInvariant();

            try
            {
                var asm = typeof(Plugin).Assembly;
                string resourcePath = "pdkmMenu.localization.translations.csv";

                using (Stream stream = asm.GetManifestResourceStream(resourcePath))
                {
                    if (stream == null) return;

                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string header = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(header)) return;

                        // AUTO-DETECT SEPARATOR: Check for semicolon first, fallback to comma
                        char separator = header.Contains(';') ? ';' : ',';

                        var columns = header.Split(separator).Select(s => s.Trim().ToLowerInvariant()).ToList();
                        _availableLanguages = columns.Skip(1).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                        int langIdx = columns.IndexOf(_currentLanguage);
                        if (langIdx == -1) langIdx = columns.IndexOf("en");
                        if (langIdx == -1 && columns.Count > 1) langIdx = 1;

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            var values = line.Split(separator);

                            // SAFETY CHECK: Skip rows that don't have enough data
                            if (values.Length <= langIdx || langIdx < 0) continue;

                            string rawKey = values[0].Trim();
                            if (Enum.TryParse(rawKey, out LKey key))
                            {
                                _activeTranslations[key] = values[langIdx].Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Localization Error: {ex}");
            }
            _initialized = true;
        }

        public static IReadOnlyList<string> GetAvailableLanguages() => _availableLanguages;

        public static void CycleLanguage()
        {
            var langs = GetAvailableLanguages();
            if (langs == null || langs.Count == 0)
            {
                Plugin.Logger.LogWarning("Cannot cycle language: No languages found in CSV header!");
                return;
            }
            Plugin.Logger.LogInfo($"Cycling from: {_currentLanguage}. Options: {string.Join(", ", langs)}");

            int idx = -1;
            for (int i = 0; i < langs.Count; i++)
            {
                if (langs[i].Equals(_currentLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    idx = i;
                    break;
                }
            }

            int next = (idx + 1) % langs.Count;
            Plugin.Logger.LogInfo($"Switching to: {langs[next]}");
            SetLanguage(langs[next]);
        }

        public static void SetLanguage(string language)
        {
            if (Plugin.MenuSettings != null) Plugin.MenuSettings.Language.Value = language;
            LoadLanguage(language);
        }
    }
}