using System;

namespace RiClothes {
    namespace V1 {
        [Serializable]
        public class CustomOption {
            /*
            * カスタムオプションのタイプ
            * V1
            * toggle
            *   | チェックボックスのオンかオフかだけのオプション
            * grid
            *   | グリッド選択複数選択肢を出す場合
            */
            public String option_type;
            /*
            * カスタムオプションの処理タイプ
            * V1
            * DELETE_OBJECT
            *   | 指定したオブジェクトを削除する
            * NOT_DELETE_OBJECT
            *   | 指定したオブジェクトを削除しない
            * ENABLE_OBJECT
            *   | 指定したオブジェクトを有効化する
            * DISABLE_OBJECT
            *   | 指定したオブジェクトを無効化する
            * SET_MATERIAL
            *   | 指定したオブジェクトのマテリアルを設定する
            *   | argmentでmaterialのパスを指定する
            * NONE
            *   | 何もしない
            */
            public String operation;
            //特定の処理タイプで値を指定するときに使用する
            public String argment = "";
            //オプションの名前
            public String name;
            //オプションの対象オブジェクト
            public String[] target_object_list;
            /*
            * オプションがデフォルトでチェックされているかどうか
            * オプションタイプがtoggleの時に使用される
            */
            public Boolean is_check = false;
            // 適用ボタンを表示するか
            public Boolean display_apply_button = false;
            //特定オブジェクトが存在するときに適用ボタンを表示するようにする
            public String[] apply_filter;
            /*
            * 何番目のグリッドがデフォルトで選択されるか
            * オプションタイプがgridの時に使用される
            */
            public int select = 0;
            //そのオプションを非表示にする (見えないだけで処理はされる)
            public Boolean is_not_visible_option = false;
            /*
            * グリッドの中身を入れる
            * オプションタイプがgridの時に使用される
            */
            public CustomGridOption[] grid_list;
            //一つのオプションで複数の処理をやりたいときに使用する
            public CustomOperation[] multi_operation;
        }
    }
}