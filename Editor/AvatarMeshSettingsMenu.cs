using System.Linq;
using UnityEditor;
using UnityEngine;
using nadena.dev.modular_avatar.core;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif

namespace AvatarTools.Editor
{
    public static class AvatarMeshSettingsMenu
    {
        private const int MENU_PRIORITY = 49; // Place it near other Modular Avatar items

        [MenuItem("GameObject/Modular Avatar/Fix Mesh Settings", false, MENU_PRIORITY)]
        private static void AddMeshSettingsMenu()
        {
            var selected = GetSelectedAvatars();

            if (selected.Length == 0)
            {
                EditorUtility.DisplayDialog("No Avatars Selected", 
                    "Please select one or more GameObjects with VRCAvatarDescriptor components.", "OK");
                return;
            }

            AvatarMeshSettingsValidator.AddMeshSettingsToMultiple(selected);
        }

        [MenuItem("GameObject/Modular Avatar/Fix Mesh Settings", true)]
        private static bool AddMeshSettingsMenuValidate()
        {
#if VRC_SDK_VRCSDK3
            return Selection.gameObjects.Any(go => 
            {
                var descriptor = go.GetComponent<VRCAvatarDescriptor>();
                if (descriptor == null) return false;
                
                // Show menu item if avatar is invalid (missing or misconfigured)
                return !AvatarMeshSettingsValidator.ValidateAvatar(go);
            });
#else
            return false;
#endif
        }

        [MenuItem("Tools/Avatar Tools/Validate All Mesh Settings", false, 100)]
        private static void ValidateAllMeshSettings()
        {
            var window = EditorWindow.GetWindow<AvatarMeshSettingsWindow>("Avatar Mesh Settings");
            window.Show();
        }

        [MenuItem("Tools/Avatar Tools/Fix All Missing Mesh Settings", false, 101)]
        private static void FixAllMissingMeshSettings()
        {
            var avatars = AvatarMeshSettingsValidator.FindAllAvatarsInScene();
            var missingSettings = avatars.Where(a => !AvatarMeshSettingsValidator.ValidateAvatar(a)).ToArray();

            if (missingSettings.Length == 0)
            {
                EditorUtility.DisplayDialog("No Issues Found", 
                    "All avatars in the scene already have MAMeshSettings.", "OK");
                return;
            }

            var result = EditorUtility.DisplayDialogComplex("Fix Missing Mesh Settings",
                $"Found {missingSettings.Length} avatar(s) without MAMeshSettings.\n\nWould you like to add MAMeshSettings to all of them?",
                "Fix All", "Cancel", "Show Details");

            if (result == 0) // Fix All
            {
                AvatarMeshSettingsValidator.AddMeshSettingsToMultiple(missingSettings);
                EditorUtility.DisplayDialog("Complete", 
                    $"Added MAMeshSettings to {missingSettings.Length} avatar(s).", "OK");
            }
            else if (result == 2) // Show Details
            {
                var window = EditorWindow.GetWindow<AvatarMeshSettingsWindow>("Avatar Mesh Settings");
                window.Show();
            }
        }

        [MenuItem("CONTEXT/VRCAvatarDescriptor/Fix MAMeshSettings", false)]
        private static void AddMeshSettingsContext(MenuCommand command)
        {
#if VRC_SDK_VRCSDK3
            var descriptor = command.context as VRCAvatarDescriptor;
            if (descriptor != null)
            {
                AvatarMeshSettingsValidator.AddMeshSettings(descriptor.gameObject);
            }
#endif
        }

        [MenuItem("CONTEXT/VRCAvatarDescriptor/Fix MAMeshSettings", true)]
        private static bool AddMeshSettingsContextValidate(MenuCommand command)
        {
#if VRC_SDK_VRCSDK3
            var descriptor = command.context as VRCAvatarDescriptor;
            if (descriptor != null)
            {
                // Show menu item if avatar is invalid (missing or misconfigured)
                return !AvatarMeshSettingsValidator.ValidateAvatar(descriptor.gameObject);
            }
#endif
            return false;
        }

        private static GameObject[] GetSelectedAvatars()
        {
#if VRC_SDK_VRCSDK3
            return Selection.gameObjects
                .Where(go => go.GetComponent<VRCAvatarDescriptor>() != null)
                .ToArray();
#else
            return new GameObject[0];
#endif
        }
    }
}