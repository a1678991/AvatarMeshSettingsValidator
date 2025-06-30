using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AvatarTools.Editor
{
    public class AvatarMeshSettingsProvider : SettingsProvider
    {
        private SerializedObject _serializedSettings;
        private ValidatorSettings _settings;
        
        private const string SETTINGS_PATH = "Project/Avatar Tools/Mesh Settings Validator";
        
        private AvatarMeshSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
            : base(path, scope)
        {
            keywords = new HashSet<string>(new[] 
            { 
                "Avatar", "Mesh", "Settings", "Validator", "MAMeshSettings", 
                "ModularAvatar", "VRChat", "Hierarchy", "Icons"
            });
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new AvatarMeshSettingsProvider(SETTINGS_PATH, SettingsScope.Project);
            
            // Automatically create keywords from the settings class
            return provider;
        }
        
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _settings = AvatarMeshSettingsValidator.Settings;
            _serializedSettings = new SerializedObject(_settings);
        }
        
        public override void OnGUI(string searchContext)
        {
            _serializedSettings.Update();
            
            EditorGUILayout.LabelField(Localizer.Get("settings.title"), EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Language Selection
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Language / 言語 / 语言");
            var currentLang = Localizer.GetCurrentLanguage();
            var availableLangs = Localizer.GetAvailableLanguages();
            var langIndex = System.Array.IndexOf(availableLangs, currentLang);
            var displayNames = new string[availableLangs.Length];
            for (int i = 0; i < availableLangs.Length; i++)
            {
                displayNames[i] = Localizer.GetLanguageDisplayName(availableLangs[i]);
            }
            var newLangIndex = EditorGUILayout.Popup(langIndex, displayNames);
            if (newLangIndex != langIndex)
            {
                Localizer.SetLanguage(availableLangs[newLangIndex]);
                EditorApplication.RepaintHierarchyWindow();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            // Display Options
            EditorGUILayout.LabelField(Localizer.Get("settings.display_options"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ShowHierarchyIcons"), 
                Localizer.GetContent("settings.show_hierarchy_icons", "settings.show_hierarchy_icons_tooltip"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ShowValidIcons"), 
                Localizer.GetContent("settings.show_valid_icons", "settings.show_valid_icons_tooltip"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("RecursiveValidation"), 
                Localizer.GetContent("settings.recursive_validation", "settings.recursive_validation_tooltip"));
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Icon Configuration
            EditorGUILayout.LabelField(Localizer.Get("settings.icon_configuration"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ErrorIcon"), 
                new GUIContent("Error Icon", "Icon shown for avatars with missing or invalid configuration"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("WarningIcon"), 
                new GUIContent("Warning Icon", "Icon shown for parent objects with affected children"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ValidIcon"), 
                new GUIContent("Valid Icon", "Icon shown for properly configured avatars"));
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Validation Rules
            EditorGUILayout.LabelField(Localizer.Get("settings.validation_rules"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ValidateAnchorOverride"), 
                Localizer.GetContent("settings.validate_anchor", "settings.validate_anchor_tooltip"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ValidateBoundsOverride"), 
                Localizer.GetContent("settings.validate_bounds", "settings.validate_bounds_tooltip"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("RequireNonDefaultBounds"), 
                Localizer.GetContent("settings.require_non_default_bounds", "settings.require_non_default_bounds_tooltip"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ValidatePrefabs"), 
                Localizer.GetContent("settings.validate_prefabs", "settings.validate_prefabs_tooltip"));
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Actions
            EditorGUILayout.LabelField(Localizer.Get("settings.actions"), EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button(Localizer.Get("settings.open_validator"), GUILayout.Height(30)))
            {
                EditorWindow.GetWindow<AvatarMeshSettingsWindow>("Avatar Mesh Settings");
            }
            
            if (GUILayout.Button(Localizer.Get("settings.validate_all_avatars"), GUILayout.Height(30)))
            {
                var window = EditorWindow.GetWindow<AvatarMeshSettingsWindow>(Localizer.Get("window.title"));
                window.Show();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button(Localizer.Get("settings.reset_to_defaults")))
            {
                if (EditorUtility.DisplayDialog(Localizer.Get("dialog.reset_settings"), 
                    Localizer.Get("dialog.reset_confirm"), 
                    Localizer.Get("dialog.reset"), Localizer.Get("dialog.cancel")))
                {
                    Undo.RecordObject(_settings, "Reset Avatar Validator Settings");
                    
                    _settings.ShowHierarchyIcons = true;
                    _settings.ShowValidIcons = false;
                    _settings.RecursiveValidation = true;
                    _settings.ValidateAnchorOverride = true;
                    _settings.ValidateBoundsOverride = true;
                    _settings.RequireNonDefaultBounds = false;
                    _settings.ValidatePrefabs = true;
                    
                    EditorUtility.SetDirty(_settings);
                    _serializedSettings.Update();
                }
            }
            
            if (_serializedSettings.hasModifiedProperties)
            {
                _serializedSettings.ApplyModifiedProperties();
                EditorUtility.SetDirty(_settings);
                
                // Refresh hierarchy to update icons
                EditorApplication.RepaintHierarchyWindow();
            }
        }
    }
}