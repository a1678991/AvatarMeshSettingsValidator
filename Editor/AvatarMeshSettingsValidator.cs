using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using nadena.dev.modular_avatar.core;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif

namespace AvatarTools.Editor
{
    [InitializeOnLoad]
    public static class AvatarMeshSettingsValidator
    {
        // ModularAvatar's default bounds: center (0,0,0) and size (2,2,2)
        private static readonly Bounds MA_DEFAULT_BOUNDS = new Bounds(Vector3.zero, Vector3.one * 2);
        
        public static event Action<GameObject> OnAvatarValidationChanged;
        public static event Action<ValidationResult[]> OnBatchValidationComplete;

        private static ValidatorSettings _settings;
        public static ValidatorSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = LoadOrCreateSettings();
                }
                return _settings;
            }
        }

        static AvatarMeshSettingsValidator()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            AvatarValidationCache.Instance.InvalidateAll();
        }

        public static bool ValidateAvatar(GameObject avatar)
        {
            if (avatar == null) return true;

#if VRC_SDK_VRCSDK3
            var descriptor = avatar.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null) return true;

            var meshSettings = avatar.GetComponent<ModularAvatarMeshSettings>();
            if (meshSettings == null) return false;

            // Check Anchor Override configuration if enabled
            if (Settings.ValidateAnchorOverride)
            {
                if (meshSettings.InheritProbeAnchor != ModularAvatarMeshSettings.InheritMode.Set || 
                    meshSettings.ProbeAnchor == null || 
                    meshSettings.ProbeAnchor.Get(meshSettings) == null)
                {
                    return false;
                }
            }
            
            // Check Bounds Override configuration if enabled
            if (Settings.ValidateBoundsOverride)
            {
                if (meshSettings.InheritBounds != ModularAvatarMeshSettings.InheritMode.Set || 
                    meshSettings.RootBone == null || 
                    meshSettings.RootBone.Get(meshSettings) == null)
                {
                    return false;
                }
            }

            if (Settings.RequireNonDefaultBounds)
            {
                // Check if bounds are at ModularAvatar's default values
                if (meshSettings.Bounds.center == MA_DEFAULT_BOUNDS.center && 
                    meshSettings.Bounds.size == MA_DEFAULT_BOUNDS.size)
                    return false;
            }

            return true;
#else
            return true;
#endif
        }

        public static ValidationResult GetValidationResult(GameObject gameObject)
        {
            if (gameObject == null)
                return new ValidationResult { IsValid = true };

            var result = new ValidationResult
            {
                RootObject = gameObject,
                Issues = new List<ValidationIssue>()
            };

#if VRC_SDK_VRCSDK3
            var descriptor = gameObject.GetComponent<VRCAvatarDescriptor>();
            if (descriptor != null)
            {
                var meshSettings = gameObject.GetComponent<ModularAvatarMeshSettings>();
                if (meshSettings == null)
                {
                    result.Issues.Add(ValidationIssue.MissingMAMeshSettings);
                }
                else
                {
                    // Check Anchor Override configuration if enabled
                    if (Settings.ValidateAnchorOverride)
                    {
                        if (meshSettings.InheritProbeAnchor != ModularAvatarMeshSettings.InheritMode.Set || 
                            meshSettings.ProbeAnchor == null || 
                            meshSettings.ProbeAnchor.Get(meshSettings) == null)
                        {
                            result.Issues.Add(ValidationIssue.InvalidAnchorOverride);
                        }
                    }
                    
                    // Check Bounds Override configuration if enabled
                    if (Settings.ValidateBoundsOverride)
                    {
                        if (meshSettings.InheritBounds != ModularAvatarMeshSettings.InheritMode.Set || 
                            meshSettings.RootBone == null || 
                            meshSettings.RootBone.Get(meshSettings) == null)
                        {
                            result.Issues.Add(ValidationIssue.InvalidAnchorOverride);
                        }
                    }
                    
                    // Check bounds if required
                    if (Settings.RequireNonDefaultBounds)
                    {
                        if (meshSettings.Bounds.center == MA_DEFAULT_BOUNDS.center && 
                            meshSettings.Bounds.size == MA_DEFAULT_BOUNDS.size)
                        {
                            result.Issues.Add(ValidationIssue.InvalidConfiguration);
                        }
                    }
                }
            }

            if (Settings.RecursiveValidation)
            {
                var childAvatars = gameObject.GetComponentsInChildren<VRCAvatarDescriptor>(true)
                    .Where(d => d.gameObject != gameObject)
                    .Select(d => d.gameObject)
                    .ToList();

                foreach (var child in childAvatars)
                {
                    if (!ValidateAvatar(child))
                    {
                        result.AffectedChildren.Add(child);
                        if (!result.Issues.Contains(ValidationIssue.MissingInChild))
                        {
                            result.Issues.Add(ValidationIssue.MissingInChild);
                        }
                    }
                }
            }
#endif

            result.IsValid = result.Issues.Count == 0;
            return result;
        }

        public static void AddMeshSettings(GameObject avatar)
        {
            if (avatar == null) return;

#if VRC_SDK_VRCSDK3
            var descriptor = avatar.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null) return;

            var meshSettings = avatar.GetComponent<ModularAvatarMeshSettings>();
            bool wasNewComponent = false;
            
            if (meshSettings == null)
            {
                Undo.RecordObject(avatar, "Add MAMeshSettings");
                meshSettings = avatar.AddComponent<ModularAvatarMeshSettings>();
                wasNewComponent = true;
            }
            else
            {
                Undo.RecordObject(meshSettings, "Configure MAMeshSettings");
            }

            // Set modes based on validation settings
            // Note: ModularAvatar defaults to InheritMode.Inherit for both fields
            // We only change to "Set" mode when validation is enabled
            if (Settings.ValidateAnchorOverride)
            {
                meshSettings.InheritProbeAnchor = ModularAvatarMeshSettings.InheritMode.Set;
            }
            
            if (Settings.ValidateBoundsOverride)
            {
                meshSettings.InheritBounds = ModularAvatarMeshSettings.InheritMode.Set;
            }
            
            // Find Hips bone for both Anchor and Bounds
            var hips = FindHipsBone(avatar);
            
            // Only set ProbeAnchor if validation is enabled and not already configured
            if (Settings.ValidateAnchorOverride && 
                (meshSettings.ProbeAnchor == null || meshSettings.ProbeAnchor.Get(meshSettings) == null))
            {
                if (hips != null)
                {
                    if (meshSettings.ProbeAnchor == null)
                        meshSettings.ProbeAnchor = new AvatarObjectReference();
                    meshSettings.ProbeAnchor.Set(hips);
                }
                else
                {
                    Debug.LogWarning(string.Format(Localizer.Get("log.hips_not_found"), avatar.name));
                }
            }
            
            // Only set RootBone if validation is enabled and not already configured
            if (Settings.ValidateBoundsOverride && 
                (meshSettings.RootBone == null || meshSettings.RootBone.Get(meshSettings) == null))
            {
                if (hips != null)
                {
                    if (meshSettings.RootBone == null)
                        meshSettings.RootBone = new AvatarObjectReference();
                    meshSettings.RootBone.Set(hips);
                }
                else
                {
                    Debug.LogWarning(string.Format(Localizer.Get("log.bounds_not_found"), avatar.name));
                }
            }
            
            EditorUtility.SetDirty(meshSettings);
            OnAvatarValidationChanged?.Invoke(avatar);
            
            if (wasNewComponent)
            {
                if (Settings.ValidateAnchorOverride || Settings.ValidateBoundsOverride)
                {
                    Debug.Log(string.Format(Localizer.Get("log.added_with_hips"), avatar.name));
                }
                else
                {
                    Debug.Log(string.Format(Localizer.Get("log.added_settings"), avatar.name));
                }
            }
            else
            {
                if (Settings.ValidateAnchorOverride || Settings.ValidateBoundsOverride)
                {
                    Debug.Log(string.Format(Localizer.Get("log.updated_with_fix"), avatar.name));
                }
                else
                {
                    Debug.Log(string.Format(Localizer.Get("log.updated_settings"), avatar.name));
                }
            }
#endif
        }

        private static GameObject FindHipsBone(GameObject avatar)
        {
            if (avatar == null) return null;

            // Common names for hips bones
            string[] hipsNames = { "Hips", "hips", "Hip", "hip", "Pelvis", "pelvis", "Root", "root" };
            
            // First try to find in the avatar's animator
            var animator = avatar.GetComponent<Animator>();
            if (animator != null && animator.isHuman)
            {
                var hipsBone = animator.GetBoneTransform(HumanBodyBones.Hips);
                if (hipsBone != null)
                    return hipsBone.gameObject;
            }
            
            // Fall back to searching by name
            foreach (var hipsName in hipsNames)
            {
                var transforms = avatar.GetComponentsInChildren<Transform>(true);
                foreach (var t in transforms)
                {
                    if (t.name.Equals(hipsName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return t.gameObject;
                    }
                }
            }
            
            // Try to find in Armature
            var armature = avatar.transform.Find("Armature");
            if (armature != null)
            {
                foreach (var hipsName in hipsNames)
                {
                    var hips = armature.Find(hipsName);
                    if (hips != null)
                        return hips.gameObject;
                }
            }
            
            return null;
        }

        public static void AddMeshSettingsToMultiple(GameObject[] avatars)
        {
            if (avatars == null || avatars.Length == 0) return;

            Undo.SetCurrentGroupName($"Add MAMeshSettings to {avatars.Length} avatars");
            var group = Undo.GetCurrentGroup();

            foreach (var avatar in avatars)
            {
                AddMeshSettings(avatar);
            }

            Undo.CollapseUndoOperations(group);

            var results = avatars.Select(a => GetValidationResult(a)).ToArray();
            OnBatchValidationComplete?.Invoke(results);
        }

        public static GameObject[] FindAllAvatarsInScene()
        {
#if VRC_SDK_VRCSDK3
            return UnityEngine.Object.FindObjectsOfType<VRCAvatarDescriptor>()
                .Select(d => d.gameObject)
                .ToArray();
#else
            return new GameObject[0];
#endif
        }

        public static GameObject[] FindAllAvatarsInProject()
        {
            var avatars = new List<GameObject>();

#if VRC_SDK_VRCSDK3
            var guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.GetComponent<VRCAvatarDescriptor>() != null)
                {
                    avatars.Add(prefab);
                }
            }
#endif

            return avatars.ToArray();
        }

        private static ValidatorSettings LoadOrCreateSettings()
        {
            var settingsPath = "Assets/Tools/Editor/AvatarMeshSettingsValidator/ValidatorSettings.asset";
            var settings = AssetDatabase.LoadAssetAtPath<ValidatorSettings>(settingsPath);

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<ValidatorSettings>();
                
                var directory = System.IO.Path.GetDirectoryName(settingsPath);
                if (!AssetDatabase.IsValidFolder(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
                
                AssetDatabase.CreateAsset(settings, settingsPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<ValidationIssue> Issues { get; set; } = new List<ValidationIssue>();
        public GameObject RootObject { get; set; }
        public List<GameObject> AffectedChildren { get; set; } = new List<GameObject>();

        public bool HasChildIssues => AffectedChildren.Count > 0;
    }

    public enum ValidationIssue
    {
        None = 0,
        MissingMAMeshSettings = 1,
        InvalidConfiguration = 2,
        MissingInChild = 4,
        InvalidAnchorOverride = 8
    }
}