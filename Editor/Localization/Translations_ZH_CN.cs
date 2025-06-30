using System.Collections.Generic;

namespace AvatarTools.Editor
{
    internal static class Translations_ZH_CN
    {
        public static Dictionary<string, string> GetTranslations()
        {
            return new Dictionary<string, string>
            {
                // Window and Menu Items
                ["menu.fix_mesh_settings"] = "修复网格设置",
                ["menu.validate_all"] = "验证所有网格设置",
                ["menu.fix_all_missing"] = "修复所有缺失的网格设置",
                ["window.title"] = "虚拟形象网格设置",
                
                // Validation Messages
                ["validation.missing_component"] = "缺少 MAMeshSettings 组件",
                ["validation.invalid_anchor"] = "无效的锚点覆盖配置（必须设置为"Set"并具有有效目标）",
                ["validation.invalid_bounds"] = "无效的边界配置（必须设置为"Set"并具有有效目标）",
                ["validation.invalid_config"] = "无效的 MAMeshSettings 配置",
                ["validation.child_issues"] = "子虚拟形象存在配置问题",
                ["validation.properly_configured"] = "虚拟形象配置正确",
                
                // Dialog Messages
                ["dialog.no_avatars"] = "未选择虚拟形象",
                ["dialog.no_avatars_message"] = "请选择一个或多个带有 VRCAvatarDescriptor 组件的游戏对象。",
                ["dialog.no_issues"] = "未发现问题",
                ["dialog.all_configured"] = "场景中的所有虚拟形象都已具有 MAMeshSettings。",
                ["dialog.fix_missing"] = "修复缺失的网格设置",
                ["dialog.fix_missing_message"] = "发现 {0} 个没有 MAMeshSettings 的虚拟形象。\n\n是否要为所有虚拟形象添加 MAMeshSettings？",
                ["dialog.fix_all"] = "全部修复",
                ["dialog.cancel"] = "取消",
                ["dialog.show_details"] = "显示详情",
                ["dialog.complete"] = "完成",
                ["dialog.complete_message"] = "已为 {0} 个虚拟形象添加 MAMeshSettings。",
                ["dialog.reset_settings"] = "重置设置",
                ["dialog.reset_confirm"] = "确定要将所有设置重置为默认值吗？",
                ["dialog.reset"] = "重置",
                
                // Settings
                ["settings.title"] = "虚拟形象网格设置验证器配置",
                ["settings.display_options"] = "显示选项",
                ["settings.icon_configuration"] = "图标配置",
                ["settings.show_hierarchy_icons"] = "显示层级图标",
                ["settings.show_hierarchy_icons_tooltip"] = "在 Unity 层级窗口中显示验证图标",
                ["settings.show_valid_icons"] = "显示有效图标",
                ["settings.show_valid_icons_tooltip"] = "为正确配置的虚拟形象显示绿色勾号",
                ["settings.recursive_validation"] = "递归验证",
                ["settings.recursive_validation_tooltip"] = "检查子游戏对象的虚拟形象问题",
                ["settings.validation_rules"] = "验证规则",
                ["settings.validate_anchor"] = "验证锚点覆盖",
                ["settings.validate_anchor_tooltip"] = "检查锚点覆盖是否设置为"Set"并具有有效目标",
                ["settings.validate_bounds"] = "验证边界覆盖",
                ["settings.validate_bounds_tooltip"] = "检查边界覆盖是否设置为"Set"并具有有效目标",
                ["settings.require_non_default_bounds"] = "要求非默认边界",
                ["settings.require_non_default_bounds_tooltip"] = "如果边界为默认值，则验证失败",
                ["settings.validate_prefabs"] = "验证预制体",
                ["settings.validate_prefabs_tooltip"] = "在项目范围验证中包含预制体",
                ["settings.actions"] = "操作",
                ["settings.open_validator"] = "打开验证器窗口",
                ["settings.validate_all_avatars"] = "验证所有虚拟形象",
                ["settings.reset_to_defaults"] = "重置为默认值",
                
                // Window UI
                ["window.refresh"] = "刷新",
                ["window.fix_all"] = "全部修复",
                ["window.auto_refresh"] = "自动刷新",
                ["window.settings"] = "设置",
                ["window.scene_avatars"] = "场景虚拟形象 ({0})",
                ["window.project_avatars"] = "项目虚拟形象 ({0})",
                ["window.no_avatars_found"] = "未找到虚拟形象",
                ["window.fix"] = "修复",
                ["window.status_need_settings"] = "{0} 个虚拟形象需要 MAMeshSettings",
                ["window.status_all_configured"] = "所有虚拟形象配置正确",
                ["window.total_avatars"] = "总计：{0} 个虚拟形象",
                
                // Log Messages
                ["log.added_settings"] = "已为 {0} 添加 MAMeshSettings",
                ["log.updated_settings"] = "已更新 {0} 的 MAMeshSettings",
                ["log.added_with_hips"] = "已为 {0} 添加 MAMeshSettings（将 Hips 设置为覆盖目标）",
                ["log.updated_with_fix"] = "已更新 {0} 的 MAMeshSettings - 修复了覆盖配置",
                ["log.hips_not_found"] = "在 {0} 中找不到 Hips 骨骼。请手动设置锚点覆盖。",
                ["log.bounds_not_found"] = "在 {0} 中找不到 Hips 骨骼。请手动设置边界根骨骼。",
                
                // Tooltips
                ["tooltip.error_icon"] = "{0}：{1}",
                ["tooltip.warning_icon"] = "{0}：子虚拟形象存在配置问题",
                ["tooltip.valid_icon"] = "{0}：虚拟形象配置正确"
            };
        }
    }
}