using System;

namespace RiClothes {
    namespace V1 {
        [Serializable]
        public class DifferenceParts {
            //パーツの名前
            public String parts_name;
            //デフォルトで選択される場所
            public int select = 0;
            //差分リスト
            public PartsTexture[] texture_list;
        }
    }
}