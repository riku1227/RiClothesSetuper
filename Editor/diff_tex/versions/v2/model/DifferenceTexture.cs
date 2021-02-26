using System;

/*
* パスを書くときのベースパスはすべて指定していた場合はそのフォルダ指定していなかった場合はDiffTexGenのjsonファイルがあるフォルダになる
*/

namespace RiClothes {
    namespace V2 {
        [Serializable]
        public class DifferenceTexture {
            //テクスチャの名前
            public string name;
            //そのテクスチャの説明
            public string description;
            /*
            * テクスチャの出力パス
            */
            public string output_path;
            //テクスチャのその中のパーツリスト
            public DifferenceParts[] parts_list;
        }
    }
}