using System;

namespace RiClothes {
    namespace V2 {
        [Serializable]
        public class CustomGridOption {
            //オプションの名前
            public string name;
            //オプションの処理内容
            public CustomOperation[] operation;
        }
    }
}