using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif

namespace AvatarTools.Editor
{
    public class AvatarMeshSettingsWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private List<AvatarInfo> _sceneAvatars = new List<AvatarInfo>();
        private List<AvatarInfo> _projectAvatars = new List<AvatarInfo>();
        private bool _showSceneAvatars = true;
        private bool _showProjectAvatars = false;
        private bool _autoRefresh = true;
        private double _lastRefreshTime;
        private const double REFRESH_INTERVAL = 1.0;

        private class AvatarInfo
        {
            public GameObject GameObject;
            public bool IsValid;
            public string Path;
            public ValidationResult ValidationResult;
        }

        private void OnEnable()
        {
            RefreshAvatarList();
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            AvatarMeshSettingsValidator.OnAvatarValidationChanged += OnAvatarValidationChanged;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            AvatarMeshSettingsValidator.OnAvatarValidationChanged -= OnAvatarValidationChanged;
        }

        private void OnHierarchyChanged()
        {
            if (_autoRefresh)
            {
                RefreshAvatarList();
            }
        }

        private void OnAvatarValidationChanged(GameObject avatar)
        {
            RefreshAvatarList();
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawAvatarList();
            DrawStatusBar();

            // Auto-refresh periodically
            if (_autoRefresh && EditorApplication.timeSinceStartup - _lastRefreshTime > REFRESH_INTERVAL)
            {
                RefreshAvatarList();
                Repaint();
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                RefreshAvatarList();
            }

            if (GUILayout.Button("Fix All", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                FixAllInvalid();
            }

            GUILayout.FlexibleSpace();

            _autoRefresh = GUILayout.Toggle(_autoRefresh, "Auto Refresh", EditorStyles.toolbarButton, GUILayout.Width(80));

            if (GUILayout.Button("Settings", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                Selection.activeObject = AvatarMeshSettingsValidator.Settings;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAvatarList()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Scene Avatars
            _showSceneAvatars = EditorGUILayout.BeginFoldoutHeaderGroup(_showSceneAvatars, 
                $"Scene Avatars ({_sceneAvatars.Count})");
            
            if (_showSceneAvatars)
            {
                EditorGUI.indentLevel++;
                DrawAvatarSection(_sceneAvatars, true);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);

            // Project Avatars
            _showProjectAvatars = EditorGUILayout.BeginFoldoutHeaderGroup(_showProjectAvatars, 
                $"Project Avatars ({_projectAvatars.Count})");
            
            if (_showProjectAvatars)
            {
                EditorGUI.indentLevel++;
                DrawAvatarSection(_projectAvatars, false);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.EndScrollView();
        }

        private void DrawAvatarSection(List<AvatarInfo> avatars, bool isSceneAvatar)
        {
            foreach (var avatar in avatars)
            {
                EditorGUILayout.BeginHorizontal();

                // Status icon
                var iconRect = GUILayoutUtility.GetRect(20, 20, GUILayout.Width(20));
                if (!avatar.IsValid)
                {
                    string tooltip = "Configuration issues: ";
                    if (avatar.ValidationResult.Issues.Contains(ValidationIssue.MissingMAMeshSettings))
                        tooltip = "Missing MAMeshSettings";
                    else if (avatar.ValidationResult.Issues.Contains(ValidationIssue.InvalidAnchorOverride))
                        tooltip = "Invalid Anchor Override configuration";
                    else if (avatar.ValidationResult.Issues.Contains(ValidationIssue.InvalidConfiguration))
                        tooltip = "Invalid configuration";
                        
                    GUI.Label(iconRect, new GUIContent(AvatarMeshSettingsValidator.Settings.ErrorIcon, tooltip));
                }
                else
                {
                    GUI.Label(iconRect, new GUIContent(AvatarMeshSettingsValidator.Settings.ValidIcon, 
                        "Properly configured"));
                }

                // Avatar name
                var labelStyle = avatar.IsValid ? EditorStyles.label : new GUIStyle(EditorStyles.label) 
                { 
                    normal = { textColor = new Color(1f, 0.3f, 0.3f) }
                };
                
                if (GUILayout.Button(avatar.GameObject.name, labelStyle))
                {
                    Selection.activeGameObject = avatar.GameObject;
                    if (isSceneAvatar)
                    {
                        EditorGUIUtility.PingObject(avatar.GameObject);
                    }
                    else
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(avatar.Path));
                    }
                }

                GUILayout.FlexibleSpace();

                // Action buttons
                GUI.enabled = !avatar.IsValid;
                if (GUILayout.Button("Fix", GUILayout.Width(40)))
                {
                    AvatarMeshSettingsValidator.AddMeshSettings(avatar.GameObject);
                    RefreshAvatarList();
                }
                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();

                // Show child issues if any
                if (avatar.ValidationResult != null && avatar.ValidationResult.HasChildIssues)
                {
                    EditorGUI.indentLevel++;
                    foreach (var child in avatar.ValidationResult.AffectedChildren)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        EditorGUILayout.LabelField($"↳ {child.name}", 
                            new GUIStyle(EditorStyles.label) { normal = { textColor = Color.yellow } });
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
            }

            if (avatars.Count == 0)
            {
                EditorGUILayout.LabelField("No avatars found", EditorStyles.centeredGreyMiniLabel);
            }
        }

        private void DrawStatusBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            var invalidCount = _sceneAvatars.Count(a => !a.IsValid) + _projectAvatars.Count(a => !a.IsValid);
            var totalCount = _sceneAvatars.Count + _projectAvatars.Count;

            var statusText = invalidCount > 0 
                ? $"{invalidCount} avatar(s) need MAMeshSettings" 
                : "All avatars properly configured";

            var statusStyle = invalidCount > 0 
                ? new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(1f, 0.5f, 0f) } }
                : EditorStyles.miniLabel;

            EditorGUILayout.LabelField(statusText, statusStyle);

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField($"Total: {totalCount} avatars", EditorStyles.miniLabel);

            EditorGUILayout.EndHorizontal();
        }

        private void RefreshAvatarList()
        {
            _lastRefreshTime = EditorApplication.timeSinceStartup;

            // Get scene avatars
            _sceneAvatars.Clear();
            var sceneAvatars = AvatarMeshSettingsValidator.FindAllAvatarsInScene();
            foreach (var avatar in sceneAvatars)
            {
                var result = AvatarMeshSettingsValidator.GetValidationResult(avatar);
                _sceneAvatars.Add(new AvatarInfo
                {
                    GameObject = avatar,
                    IsValid = AvatarMeshSettingsValidator.ValidateAvatar(avatar),
                    Path = GetGameObjectPath(avatar),
                    ValidationResult = result
                });
            }
            _sceneAvatars = _sceneAvatars.OrderBy(a => a.GameObject.name).ToList();

            // Get project avatars
            if (AvatarMeshSettingsValidator.Settings.ValidatePrefabs)
            {
                _projectAvatars.Clear();
                var projectAvatars = AvatarMeshSettingsValidator.FindAllAvatarsInProject();
                foreach (var avatar in projectAvatars)
                {
                    var result = AvatarMeshSettingsValidator.GetValidationResult(avatar);
                    _projectAvatars.Add(new AvatarInfo
                    {
                        GameObject = avatar,
                        IsValid = AvatarMeshSettingsValidator.ValidateAvatar(avatar),
                        Path = AssetDatabase.GetAssetPath(avatar),
                        ValidationResult = result
                    });
                }
                _projectAvatars = _projectAvatars.OrderBy(a => a.Path).ToList();
            }
        }

        private void FixAllInvalid()
        {
            var invalidAvatars = new List<GameObject>();
            invalidAvatars.AddRange(_sceneAvatars.Where(a => !a.IsValid).Select(a => a.GameObject));
            invalidAvatars.AddRange(_projectAvatars.Where(a => !a.IsValid).Select(a => a.GameObject));

            if (invalidAvatars.Count == 0)
            {
                EditorUtility.DisplayDialog("No Issues", "All avatars already have MAMeshSettings.", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Fix All Invalid Avatars",
                $"Add MAMeshSettings to {invalidAvatars.Count} avatar(s)?", "Fix All", "Cancel"))
            {
                AvatarMeshSettingsValidator.AddMeshSettingsToMultiple(invalidAvatars.ToArray());
                RefreshAvatarList();
            }
        }

        private string GetGameObjectPath(GameObject obj)
        {
            var path = obj.name;
            var parent = obj.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }
    }
}