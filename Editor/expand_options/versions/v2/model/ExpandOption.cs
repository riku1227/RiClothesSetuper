using System;

namespace RiClothes {
    namespace V2 {
        [Serializable]
        public class ExpandOption {
            //対応しているRiClothes Setuperのバージョン
            public int version;
            //服のID (固有のかぶらないものが望ましい)
            public string id;
            //対象のアバターのボーンをしまっているオブジェクトの名前を指定 (使うことあるのかは不明)
            public string avatar_bone_parent = "Armature";
            /*
            * Setuper側で処理されないようにする
            * 自動処理されたくないオブジェクトの名前を指定
            * '/'でフルパスを指定することができる
            * '**'を先頭につけるとオブジェクトの中にそれが含まれていたらになる
            * '***'を先頭につけるとフルパスの中にそれが含まれていたらになる
            */
            public string[] exclude_object_list;
            //Setuper側がボーンを移動させる前に実行
            public CustomOption[] before_move_bone;
            //Setuper側がボーンを移動させた後に実行
            public CustomOption[] after_move_bone;
            //Setuper側の処理が終わったあとに実行される
            public CustomOption[] custom_options;
        }
    }
}