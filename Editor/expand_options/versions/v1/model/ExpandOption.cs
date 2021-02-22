using System;

namespace RiClothes {
    namespace V1 {
        [Serializable]
        public class ExpandOption {
            //対応しているRiClothes Setuperのバージョン
            public int version;
            //その服のID (固有のかぶらないものが望ましい)
            public String id;
            //対象のアバターのボーンをしまっているオブジェクトの名前を指定 (使うことあるのかは不明)
            public String avatar_armature = "Armature";
            //対象のアバターに着せた時に不要になるオブジェクトの名前
            public String[] unnecessary_objects;
            /*
            * 服にある独自のボーンをいい感じに処理するよう
            * Setuper側の自動ボーン移動の前に実行される
            */
            public OriginalBone[] original_bones;
            //その服独自の設定
            public CustomOption[] custom_options;
            //テクスチャの差分生成用
            public DifferenceTexture[] difference_textures;
        }
    }
}