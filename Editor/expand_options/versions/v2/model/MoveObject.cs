using System;

namespace RiClothes {
    namespace V2 {
        [Serializable]
        public class MoveObject {
            /*
            * 移動させるオブジェクト
            * 服側を参照
            */
            public String from;
            /*
            * 移動させる場所
            * アバター側を参照
            */
            public String to;
        }
    }
}