using System.Collections.Generic;

namespace AvatarTools.Editor
{
    internal static class Translations_EN
    {
        public static Dictionary<string, string> GetTranslations()
        {
            return new Dictionary<string, string>
            {
                // Window and Menu Items
                ["menu.fix_mesh_settings"] = "Fix Mesh Settings",
                ["menu.validate_all"] = "Validate All Mesh Settings",
                ["menu.fix_all_missing"] = "Fix All Missing Mesh Settings",
                ["window.title"] = "Avatar Mesh Settings",
                
                // Validation Messages
                ["validation.missing_component"] = "Missing MAMeshSettings component",
                ["validation.invalid_anchor"] = "Invalid Anchor Override configuration (must be Set with valid target)",
                ["validation.invalid_bounds"] = "Invalid Bounds configuration (must be Set with valid target)",
                ["validation.invalid_config"] = "Invalid MAMeshSettings configuration",
                ["validation.child_issues"] = "Child avatar(s) have configuration issues",
                ["validation.properly_configured"] = "Avatar properly configured",
                
                // Dialog Messages
                ["dialog.no_avatars"] = "No Avatars Selected",
                ["dialog.no_avatars_message"] = "Please select one or more GameObjects with VRCAvatarDescriptor components.",
                ["dialog.no_issues"] = "No Issues Found",
                ["dialog.all_configured"] = "All avatars in the scene already have MAMeshSettings.",
                ["dialog.fix_missing"] = "Fix Missing Mesh Settings",
                ["dialog.fix_missing_message"] = "Found {0} avatar(s) without MAMeshSettings.\n\nWould you like to add MAMeshSettings to all of them?",
                ["dialog.fix_all"] = "Fix All",
                ["dialog.cancel"] = "Cancel",
                ["dialog.show_details"] = "Show Details",
                ["dialog.complete"] = "Complete",
                ["dialog.complete_message"] = "Added MAMeshSettings to {0} avatar(s).",
                ["dialog.reset_settings"] = "Reset Settings",
                ["dialog.reset_confirm"] = "Are you sure you want to reset all settings to their default values?",
                ["dialog.reset"] = "Reset",
                
                // Settings
                ["settings.title"] = "Avatar Mesh Settings Validator Configuration",
                ["settings.display_options"] = "Display Options",
                ["settings.icon_configuration"] = "Icon Configuration",
                ["settings.show_hierarchy_icons"] = "Show Hierarchy Icons",
                ["settings.show_hierarchy_icons_tooltip"] = "Display validation icons in the Unity Hierarchy window",
                ["settings.show_valid_icons"] = "Show Valid Icons",
                ["settings.show_valid_icons_tooltip"] = "Show green checkmarks for properly configured avatars",
                ["settings.recursive_validation"] = "Recursive Validation",
                ["settings.recursive_validation_tooltip"] = "Check child GameObjects for avatar issues",
                ["settings.validation_rules"] = "Validation Rules",
                ["settings.validate_anchor"] = "Validate Anchor Override",
                ["settings.validate_anchor_tooltip"] = "Check that Anchor Override is set to 'Set' with a valid target",
                ["settings.validate_bounds"] = "Validate Bounds Override",
                ["settings.validate_bounds_tooltip"] = "Check that Bounds Override is set to 'Set' with a valid target",
                ["settings.require_non_default_bounds"] = "Require Non-Default Bounds",
                ["settings.require_non_default_bounds_tooltip"] = "Fail validation if bounds are at default values",
                ["settings.validate_prefabs"] = "Validate Prefabs",
                ["settings.validate_prefabs_tooltip"] = "Include prefabs in project-wide validation",
                ["settings.actions"] = "Actions",
                ["settings.open_validator"] = "Open Validator Window",
                ["settings.validate_all_avatars"] = "Validate All Avatars",
                ["settings.reset_to_defaults"] = "Reset to Defaults",
                
                // Window UI
                ["window.refresh"] = "Refresh",
                ["window.fix_all"] = "Fix All",
                ["window.auto_refresh"] = "Auto Refresh",
                ["window.settings"] = "Settings",
                ["window.scene_avatars"] = "Scene Avatars ({0})",
                ["window.project_avatars"] = "Project Avatars ({0})",
                ["window.no_avatars_found"] = "No avatars found",
                ["window.fix"] = "Fix",
                ["window.status_need_settings"] = "{0} avatar(s) need MAMeshSettings",
                ["window.status_all_configured"] = "All avatars properly configured",
                ["window.total_avatars"] = "Total: {0} avatars",
                
                // Log Messages
                ["log.added_settings"] = "Added MAMeshSettings to {0}",
                ["log.updated_settings"] = "Updated MAMeshSettings on {0}",
                ["log.added_with_hips"] = "Added MAMeshSettings to {0} with Hips as override target",
                ["log.updated_with_fix"] = "Updated MAMeshSettings on {0} - fixed override configuration",
                ["log.hips_not_found"] = "Could not find Hips bone in {0}. Please set Anchor Override manually.",
                ["log.bounds_not_found"] = "Could not find Hips bone in {0}. Please set Bounds Root Bone manually.",
                
                // Tooltips
                ["tooltip.error_icon"] = "{0}: {1}",
                ["tooltip.warning_icon"] = "{0}: Child avatar(s) have configuration issues",
                ["tooltip.valid_icon"] = "{0}: Avatar properly configured"
            };
        }
    }
}