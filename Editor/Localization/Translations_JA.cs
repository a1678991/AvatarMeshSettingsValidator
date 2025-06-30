using System.Collections.Generic;

namespace AvatarTools.Editor
{
    internal static class Translations_JA
    {
        public static Dictionary<string, string> GetTranslations()
        {
            return new Dictionary<string, string>
            {
                // Window and Menu Items
                ["menu.fix_mesh_settings"] = "メッシュ設定を修正",
                ["menu.validate_all"] = "すべてのメッシュ設定を検証",
                ["menu.fix_all_missing"] = "不足しているメッシュ設定をすべて修正",
                ["window.title"] = "アバターメッシュ設定",
                
                // Validation Messages
                ["validation.missing_component"] = "MAMeshSettingsコンポーネントがありません",
                ["validation.invalid_anchor"] = "無効なアンカーオーバーライド設定（有効なターゲットで「Set」である必要があります）",
                ["validation.invalid_bounds"] = "無効なBounds設定（有効なターゲットで「Set」である必要があります）",
                ["validation.invalid_config"] = "無効なMAMeshSettings設定",
                ["validation.child_issues"] = "子アバターに設定の問題があります",
                ["validation.properly_configured"] = "アバターが正しく設定されています",
                
                // Dialog Messages
                ["dialog.no_avatars"] = "アバターが選択されていません",
                ["dialog.no_avatars_message"] = "VRCAvatarDescriptorコンポーネントを持つGameObjectを1つ以上選択してください。",
                ["dialog.no_issues"] = "問題が見つかりませんでした",
                ["dialog.all_configured"] = "シーン内のすべてのアバターには既にMAMeshSettingsがあります。",
                ["dialog.fix_missing"] = "不足しているメッシュ設定を修正",
                ["dialog.fix_missing_message"] = "MAMeshSettingsがない{0}個のアバターが見つかりました。\n\nすべてにMAMeshSettingsを追加しますか？",
                ["dialog.fix_all"] = "すべて修正",
                ["dialog.cancel"] = "キャンセル",
                ["dialog.show_details"] = "詳細を表示",
                ["dialog.complete"] = "完了",
                ["dialog.complete_message"] = "{0}個のアバターにMAMeshSettingsを追加しました。",
                ["dialog.reset_settings"] = "設定をリセット",
                ["dialog.reset_confirm"] = "すべての設定をデフォルト値にリセットしてもよろしいですか？",
                ["dialog.reset"] = "リセット",
                
                // Settings
                ["settings.title"] = "アバターメッシュ設定バリデーターの設定",
                ["settings.display_options"] = "表示オプション",
                ["settings.icon_configuration"] = "アイコン設定",
                ["settings.show_hierarchy_icons"] = "ヒエラルキーアイコンを表示",
                ["settings.show_hierarchy_icons_tooltip"] = "Unityヒエラルキーウィンドウに検証アイコンを表示",
                ["settings.show_valid_icons"] = "有効なアイコンを表示",
                ["settings.show_valid_icons_tooltip"] = "正しく設定されたアバターに緑のチェックマークを表示",
                ["settings.recursive_validation"] = "再帰的検証",
                ["settings.recursive_validation_tooltip"] = "子GameObjectのアバターの問題をチェック",
                ["settings.validation_rules"] = "検証ルール",
                ["settings.validate_anchor"] = "アンカーオーバーライドを検証",
                ["settings.validate_anchor_tooltip"] = "アンカーオーバーライドが有効なターゲットで「Set」に設定されているかチェック",
                ["settings.validate_bounds"] = "Boundsオーバーライドを検証",
                ["settings.validate_bounds_tooltip"] = "Boundsオーバーライドが有効なターゲットで「Set」に設定されているかチェック",
                ["settings.require_non_default_bounds"] = "デフォルト以外のBoundsを要求",
                ["settings.require_non_default_bounds_tooltip"] = "Boundsがデフォルト値の場合、検証に失敗",
                ["settings.validate_prefabs"] = "プレハブを検証",
                ["settings.validate_prefabs_tooltip"] = "プロジェクト全体の検証にプレハブを含める",
                ["settings.actions"] = "アクション",
                ["settings.open_validator"] = "バリデーターウィンドウを開く",
                ["settings.validate_all_avatars"] = "すべてのアバターを検証",
                ["settings.reset_to_defaults"] = "デフォルトにリセット",
                
                // Window UI
                ["window.refresh"] = "更新",
                ["window.fix_all"] = "すべて修正",
                ["window.auto_refresh"] = "自動更新",
                ["window.settings"] = "設定",
                ["window.scene_avatars"] = "シーンのアバター ({0})",
                ["window.project_avatars"] = "プロジェクトのアバター ({0})",
                ["window.no_avatars_found"] = "アバターが見つかりません",
                ["window.fix"] = "修正",
                ["window.status_need_settings"] = "{0}個のアバターにMAMeshSettingsが必要です",
                ["window.status_all_configured"] = "すべてのアバターが正しく設定されています",
                ["window.total_avatars"] = "合計: {0}個のアバター",
                
                // Log Messages
                ["log.added_settings"] = "{0}にMAMeshSettingsを追加しました",
                ["log.updated_settings"] = "{0}のMAMeshSettingsを更新しました",
                ["log.added_with_hips"] = "{0}にMAMeshSettingsを追加しました（Hipsをオーバーライドターゲットとして設定）",
                ["log.updated_with_fix"] = "{0}のMAMeshSettingsを更新しました - オーバーライド設定を修正",
                ["log.hips_not_found"] = "{0}でHipsボーンが見つかりませんでした。アンカーオーバーライドを手動で設定してください。",
                ["log.bounds_not_found"] = "{0}でHipsボーンが見つかりませんでした。Boundsルートボーンを手動で設定してください。",
                
                // Tooltips
                ["tooltip.error_icon"] = "{0}: {1}",
                ["tooltip.warning_icon"] = "{0}: 子アバターに設定の問題があります",
                ["tooltip.valid_icon"] = "{0}: アバターが正しく設定されています"
            };
        }
    }
}