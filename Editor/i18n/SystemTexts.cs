namespace RiClothes {
    public class SystemTexts {
                static public string ENGLISH = $@"
option.input.avatar=Avatar
option.input.cloth=Cloth
option.button.change_cloth=Change of clothes
option.toggle.is_delete_unnecessary_object=Delete Unnecessary Object
option.toggle.show_advanced_option=Show advanced option
option.toggle.delete_is_set_editor_only_tag=(Recommend)Delete is 'Set EditorOnly Tag'
option.toggle.delete_is_set_editor_only_tag.description=Do not delete the object but set the tag to Editor Only
option.toggle.is_remove_missing_script=Remove Missing Script Component
option.toggle.is_remove_missing_script.description=For environments without dynamic bones

difftex.button.generate_all=Generate ALL Diff Texture
difftex.button.generate_tex=Generate: 

cloth_uninstaller.description=Remove all objects and bones added by clothes\nActivate a deactivated object\nUntagged tags are set to EditorOnly\Not recommended to use it on avatars that have been modified after being dressed up.
cloth_uninstaller.un_support_version=Uninstall Cloth can only be used by avatars who have\n dressed in clothes that support Expand Option V2 or higher.
cloth_uninstaller.uninstall_button=Uninstall
";
        static public string JAPANESE = $@"
option.input.avatar=アバター
option.input.cloth=服
option.button.change_cloth=着替える
option.toggle.is_delete_unnecessary_object=不要なオブジェクトを削除する
option.toggle.show_advanced_option=詳細オプションを表示する
option.toggle.delete_is_set_editor_only_tag=(推奨)削除を'EditorOnlyタグに設定'にする
option.toggle.delete_is_set_editor_only_tag.description=オブジェクトを削除せず、タグをEditor Onlyに設定するようにします
option.toggle.is_remove_missing_script=Missing Scriptコンポーネントを削除します
option.toggle.is_remove_missing_script.description=ダイナミックボーンが無い環境用です

difftex.button.generate_all=全ての差分テクスチャを生成
difftex.button.generate_tex=生成: 
cloth_uninstaller.description=服で追加されたオブジェクトとボーンを全て削除\n非アクティブにしたオブジェクトをアクティブ化\nタグがEditorOnlyになってるのを、Untaggedにします\n着せ替えた後に改変したアバターへの使用は非推奨です
cloth_uninstaller.un_support_version=Uninstall ClothはExpand Option V2以上対応の服で\n着替えたアバターのみ使用できます
cloth_uninstaller.uninstall_button=アンインストール
";
    }
}