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
            
            EditorGUILayout.LabelField("Avatar Mesh Settings Validator Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Display Options
            EditorGUILayout.LabelField("Display Options", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ShowHierarchyIcons"), 
                new GUIContent("Show Hierarchy Icons", "Display validation icons in the Unity Hierarchy window"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ShowValidIcons"), 
                new GUIContent("Show Valid Icons", "Show green checkmarks for properly configured avatars"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("RecursiveValidation"), 
                new GUIContent("Recursive Validation", "Check child GameObjects for avatar issues"));
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Icon Configuration
            EditorGUILayout.LabelField("Icon Configuration", EditorStyles.boldLabel);
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
            EditorGUILayout.LabelField("Validation Rules", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ValidateAnchorOverride"), 
                new GUIContent("Validate Anchor Override", "Check that Anchor Override is set to 'Set' with a valid target"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ValidateBoundsOverride"), 
                new GUIContent("Validate Bounds Override", "Check that Bounds Override is set to 'Set' with a valid target"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("RequireNonDefaultBounds"), 
                new GUIContent("Require Non-Default Bounds", "Fail validation if bounds are at default values"));
            
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("ValidatePrefabs"), 
                new GUIContent("Validate Prefabs", "Include prefabs in project-wide validation"));
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Actions
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Open Validator Window", GUILayout.Height(30)))
            {
                EditorWindow.GetWindow<AvatarMeshSettingsWindow>("Avatar Mesh Settings");
            }
            
            if (GUILayout.Button("Validate All Avatars", GUILayout.Height(30)))
            {
                var window = EditorWindow.GetWindow<AvatarMeshSettingsWindow>("Avatar Mesh Settings");
                window.Show();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Reset to Defaults"))
            {
                if (EditorUtility.DisplayDialog("Reset Settings", 
                    "Are you sure you want to reset all settings to their default values?", 
                    "Reset", "Cancel"))
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