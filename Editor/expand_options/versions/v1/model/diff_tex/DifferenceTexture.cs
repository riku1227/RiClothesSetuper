using System;

namespace RiClothes {
    namespace V1 {
        [Serializable]
        public class DifferenceTexture {
            //テクスチャの名前
            public String texture_name;
            //テクスチャファイルの出力パス
            public String output_path;
            //テクスチャのその中のパーツリスト
            public DifferenceParts[] parts_list;
        }
    }
}