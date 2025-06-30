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
                        tooltip = $"{obj.name}: Missing MAMeshSettings component";
                    }
                    else if (result.Issues.Contains(ValidationIssue.InvalidAnchorOverride))
                    {
                        tooltip = $"{obj.name}: Invalid Anchor Override or Bounds configuration (must be Set with valid target)";
                    }
                    else if (result.Issues.Contains(ValidationIssue.InvalidConfiguration))
                    {
                        tooltip = $"{obj.name}: Invalid MAMeshSettings configuration";
                    }
                    break;
                case IconType.Warning:
                    icon = AvatarMeshSettingsValidator.Settings.WarningIcon;
                    tooltip = $"{obj.name}: Child avatar(s) have configuration issues";
                    break;
                case IconType.Valid:
                    icon = AvatarMeshSettingsValidator.Settings.ValidIcon;
                    tooltip = $"{obj.name}: Avatar properly configured";
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