using System;

namespace RiClothes {
    namespace V1 {
        [Serializable]
        public class CustomGridOption {
            //オプションの名前
            public String name;
            /*
            * オプションの処理タイプ
            * RiClothes.V1.CustomOption.operation を参照
            */
            public String operation;
            //特定の処理タイプで値を指定するときに使用する
            public String argment = "";
            //対象のオブジェクト
            public String[] target_object_list;
            //一つのオプションで複数の処理をやりたいときに使用する
            public CustomOperation[] multi_operation;
        }
    }
}