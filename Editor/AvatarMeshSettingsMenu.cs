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
                EditorUtility.DisplayDialog(Localizer.Get("dialog.no_avatars"), 
                    Localizer.Get("dialog.no_avatars_message"), "OK");
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
            var window = EditorWindow.GetWindow<AvatarMeshSettingsWindow>(Localizer.Get("window.title"));
            window.Show();
        }

        [MenuItem("Tools/Avatar Tools/Fix All Missing Mesh Settings", false, 101)]
        private static void FixAllMissingMeshSettings()
        {
            var avatars = AvatarMeshSettingsValidator.FindAllAvatarsInScene();
            var missingSettings = avatars.Where(a => !AvatarMeshSettingsValidator.ValidateAvatar(a)).ToArray();

            if (missingSettings.Length == 0)
            {
                EditorUtility.DisplayDialog(Localizer.Get("dialog.no_issues"), 
                    Localizer.Get("dialog.all_configured"), "OK");
                return;
            }

            var result = EditorUtility.DisplayDialogComplex(Localizer.Get("dialog.fix_missing"),
                string.Format(Localizer.Get("dialog.fix_missing_message"), missingSettings.Length),
                Localizer.Get("dialog.fix_all"), Localizer.Get("dialog.cancel"), Localizer.Get("dialog.show_details"));

            if (result == 0) // Fix All
            {
                AvatarMeshSettingsValidator.AddMeshSettingsToMultiple(missingSettings);
                EditorUtility.DisplayDialog(Localizer.Get("dialog.complete"), 
                    string.Format(Localizer.Get("dialog.complete_message"), missingSettings.Length), "OK");
            }
            else if (result == 2) // Show Details
            {
                var window = EditorWindow.GetWindow<AvatarMeshSettingsWindow>(Localizer.Get("window.title"));
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