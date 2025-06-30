using UnityEngine;
using UnityEditor;
using nadena.dev.modular_avatar.core;

namespace AvatarTools.Editor
{
    [CreateAssetMenu(fileName = "AvatarValidatorSettings", menuName = "Avatar Tools/Validator Settings")]
    public class ValidatorSettings : ScriptableObject
    {
        [Header("Display Options")]
        public bool ShowHierarchyIcons = true;
        public bool ShowValidIcons = false;
        public bool RecursiveValidation = true;

        [Header("Icon Configuration")]
        public Texture2D ErrorIcon;
        public Texture2D WarningIcon;
        public Texture2D ValidIcon;

        [Header("Validation Rules")]
        public bool ValidateAnchorOverride = true;
        public bool ValidateBoundsOverride = true;
        public bool RequireNonDefaultBounds = false;
        public bool ValidatePrefabs = true;


        private void OnValidate()
        {
            if (ErrorIcon == null)
            {
                ErrorIcon = EditorGUIUtility.IconContent("console.erroricon").image as Texture2D;
            }
            if (WarningIcon == null)
            {
                WarningIcon = EditorGUIUtility.IconContent("console.warnicon").image as Texture2D;
            }
            if (ValidIcon == null)
            {
                ValidIcon = EditorGUIUtility.IconContent("Collab").image as Texture2D;
            }
        }
    }
}