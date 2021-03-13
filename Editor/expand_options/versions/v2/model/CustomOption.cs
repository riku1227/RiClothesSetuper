using System;

namespace RiClothes {
    namespace V2 {
        [Serializable]
        public class CustomOption {
            //オプションの名前
            public string name;
            //オプションの説明
            public string description;
            //そのオプションを表示にする (非表示の場合でも処理はされる)
            public Boolean visible_option = true;
            /*
            * カスタムオプションのタイプ
            * V2
            * toggle
            *   | チェックボックスのオンかオフかだけのオプション
            * grid
            *   | グリッド選択複数選択肢を出す場合
            */
            public string option_type;
            /*
            * オプションがデフォルトでチェックされているかどうか
            * オプションタイプがtoggleの時に使用される
            */
            public Boolean is_check = false;
            /*
            * 何番目のグリッドがデフォルトで選択されるか
            * オプションタイプがgridの時に使用される
            */
            public int select = 0;
            /*
            * グリッドの中身を入れる
            * オプションタイプがgridの時に使用される
            */
            public CustomGridOption[] grid_list;
            // 適用ボタンを表示するか
            public Boolean display_apply_button = false;
            //apply_filterのオブジェクトがアバターのオブジェクトかどうか
            public Boolean apply_filter_is_avatar_object = false;
            //特定オブジェクトが存在するときに適用ボタンを表示するようにする
            public String[] apply_filter;
            //オプションの処理内容
            public CustomOperation[] operation_list;
        }
    }
}