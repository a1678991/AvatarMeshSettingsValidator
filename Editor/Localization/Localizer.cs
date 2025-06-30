using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AvatarTools.Editor
{
    internal static class Localizer
    {
        private static Dictionary<string, Dictionary<string, string>> _translations;
        private static string _currentLanguage;
        
        static Localizer()
        {
            InitializeTranslations();
            DetectLanguage();
        }
        
        private static void InitializeTranslations()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>();
            
            // Load all translation files
            LoadTranslations("en", Translations_EN.GetTranslations());
            LoadTranslations("ja", Translations_JA.GetTranslations());
            LoadTranslations("zh-Hans", Translations_ZH_CN.GetTranslations());
        }
        
        private static void LoadTranslations(string languageCode, Dictionary<string, string> translations)
        {
            _translations[languageCode] = translations;
        }
        
        private static void DetectLanguage()
        {
            // Get Unity's current language setting
            var systemLanguage = Application.systemLanguage;
            
            switch (systemLanguage)
            {
                case SystemLanguage.Japanese:
                    _currentLanguage = "ja";
                    break;
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                    _currentLanguage = "zh-Hans";
                    break;
                case SystemLanguage.ChineseTraditional:
                    // Fall back to simplified Chinese for now
                    _currentLanguage = "zh-Hans";
                    break;
                default:
                    _currentLanguage = "en";
                    break;
            }
            
            // Allow override via EditorPrefs
            var savedLanguage = EditorPrefs.GetString("AvatarMeshSettingsValidator.Language", "");
            if (!string.IsNullOrEmpty(savedLanguage) && _translations.ContainsKey(savedLanguage))
            {
                _currentLanguage = savedLanguage;
            }
        }
        
        public static string Get(string key)
        {
            if (_translations.TryGetValue(_currentLanguage, out var languageDict))
            {
                if (languageDict.TryGetValue(key, out var translation))
                {
                    return translation;
                }
            }
            
            // Fall back to English
            if (_currentLanguage != "en" && _translations.TryGetValue("en", out var englishDict))
            {
                if (englishDict.TryGetValue(key, out var translation))
                {
                    return translation;
                }
            }
            
            // Return key if no translation found
            return key;
        }
        
        public static GUIContent GetContent(string key, string tooltip = null)
        {
            var text = Get(key);
            var tooltipText = string.IsNullOrEmpty(tooltip) ? "" : Get(tooltip);
            return new GUIContent(text, tooltipText);
        }
        
        public static string GetCurrentLanguage()
        {
            return _currentLanguage;
        }
        
        public static void SetLanguage(string languageCode)
        {
            if (_translations.ContainsKey(languageCode))
            {
                _currentLanguage = languageCode;
                EditorPrefs.SetString("AvatarMeshSettingsValidator.Language", languageCode);
            }
        }
        
        public static string[] GetAvailableLanguages()
        {
            var languages = new string[_translations.Count];
            _translations.Keys.CopyTo(languages, 0);
            return languages;
        }
        
        public static string GetLanguageDisplayName(string languageCode)
        {
            switch (languageCode)
            {
                case "en":
                    return "English";
                case "ja":
                    return "日本語";
                case "zh-Hans":
                    return "简体中文";
                default:
                    return languageCode;
            }
        }
    }
}