using System;

namespace RiClothes {
    namespace V2 {
        [Serializable]
        public class DifferenceParts {
            //パーツの名前
            public string name;
            //パーツの説明
            public string description;
            //デフォルトで選択される場所
            public int select = 0;
            //差分リスト
            public PartsTexture[] texture_list;
        }
    }
}