using System;

namespace RiClothes {
    namespace V2 {
        [Serializable]
        public class CustomOperation {
            /*
            * オプションの処理タイプ
            * V2
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
            *   | argumentでmaterialのパスを指定する
            *   | パスの始まり(ベース)パスはそのExpandOptionのjsonファイルがあるフォルダ
            *   | '../'と書くことで相対的に書ける
            * MOVE_OBJECT
            *   | 指定したボーンを指定した場所に入れる
            *   | move_objectで指定する
            * NONE
            *   | 何もしない
            *
            * V3
            * SET_BLEND_SHAPE
            *   | 指定したオブジェクトのブレンドシェイプ (シェイプキー) をセットする
            *   | argumentでブレンドシェイプの名前とセットする値を指定する
            *   | 書き方
            *   | example_shape:100
            */
            public string operation_type;
            //特定の処理タイプで値を指定するときに使用する
            public string argument;
            //対象のオブジェクトがアバターのオブジェクトかどうか
            public bool is_avatar_object = false;
            //MOVE_BONE以外での対象オブジェクト
            public String[] target_object_list;
            //処理タイプがMOVE_BONEのとき使用
            public MoveObject[] move_object_list;
        }
    }
}