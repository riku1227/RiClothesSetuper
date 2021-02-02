using System;

namespace RiClothes {
    namespace V1 {
        [Serializable]
        public class CustomOperation {
            /*
            * オプションの処理タイプ
            * RiClothes.V1.CustomOption.operation を参照
            */
            public String operation;
            //特定の処理タイプで値を指定するときに使用する
            public String argment = "";
            //対象のオブジェクト
            public String[] target_object_list;
        }
    }
}