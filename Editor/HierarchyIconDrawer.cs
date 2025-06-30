using UnityEditor;
using UnityEngine;

namespace AvatarTools.Editor
{
    [InitializeOnLoad]
    internal static class HierarchyIconDrawer
    {
        private enum IconType
        {
            None,
            Error,
            Warning,
            Valid
        }

        static HierarchyIconDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            if (!AvatarMeshSettingsValidator.Settings.ShowHierarchyIcons)
                return;

            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            var iconType = GetIconType(obj);
            if (iconType != IconType.None)
            {
                DrawIcon(selectionRect, iconType, obj);
            }
        }

        private static IconType GetIconType(GameObject obj)
        {
            var result = AvatarValidationCache.Instance.GetOrValidate(obj);

            if (!result.IsValid)
            {
                // Show error icon for any validation issues
                if (result.Issues.Contains(ValidationIssue.MissingMAMeshSettings) ||
                    result.Issues.Contains(ValidationIssue.InvalidAnchorOverride) ||
                    result.Issues.Contains(ValidationIssue.InvalidConfiguration))
                {
                    return IconType.Error;
                }
            }
            else if (result.HasChildIssues)
            {
                return IconType.Warning;
            }
            else if (AvatarMeshSettingsValidator.Settings.ShowValidIcons)
            {
#if VRC_SDK_VRCSDK3
                var descriptor = obj.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
                if (descriptor != null)
                    return IconType.Valid;
#endif
            }

            return IconType.None;
        }

        private static void DrawIcon(Rect selectionRect, IconType type, GameObject obj)
        {
            var iconRect = new Rect(selectionRect.xMax - 20, selectionRect.y, 16, 16);
            
            Texture2D icon = null;
            string tooltip = "";

            switch (type)
            {
                case IconType.Error:
                    icon = AvatarMeshSettingsValidator.Settings.ErrorIcon;
                    var result = AvatarValidationCache.Instance.GetOrValidate(obj);
                    if (result.Issues.Contains(ValidationIssue.MissingMAMeshSettings))
                    {
                        tooltip = string.Format(Localizer.Get("tooltip.error_icon"), obj.name, Localizer.Get("validation.missing_component"));
                    }
                    else if (result.Issues.Contains(ValidationIssue.InvalidAnchorOverride))
                    {
                        tooltip = string.Format(Localizer.Get("tooltip.error_icon"), obj.name, Localizer.Get("validation.invalid_anchor"));
                    }
                    else if (result.Issues.Contains(ValidationIssue.InvalidConfiguration))
                    {
                        tooltip = string.Format(Localizer.Get("tooltip.error_icon"), obj.name, Localizer.Get("validation.invalid_config"));
                    }
                    break;
                case IconType.Warning:
                    icon = AvatarMeshSettingsValidator.Settings.WarningIcon;
                    tooltip = string.Format(Localizer.Get("tooltip.warning_icon"), obj.name);
                    break;
                case IconType.Valid:
                    icon = AvatarMeshSettingsValidator.Settings.ValidIcon;
                    tooltip = string.Format(Localizer.Get("tooltip.valid_icon"), obj.name);
                    break;
            }

            if (icon != null)
            {
                var content = new GUIContent(icon, tooltip);
                GUI.Label(iconRect, content);
                
                if (Event.current.type == EventType.MouseDown && iconRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 0)
                    {
                        Selection.activeGameObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                    Event.current.Use();
                }
            }
        }
    }
}