using System;
using UnityEngine;

namespace RiClothes {
    public class GameObjectUtil {
        /*
        * そのオブジェクトのフルパスを取得する
        * プレハブ(一番上の親)は除外される
        */
        public static string GetFullPath(Transform transform) {
            string result = transform.name;
            Transform objectParent = transform.parent;
            //プレハブ(一番上の親)を除外するためにそのオブジェクトに親要素があるかないかで判定
            while(objectParent.parent) {
                result = objectParent.name + "/" + result;
                objectParent = objectParent.parent;
            }
            return result;
        }
        //GameObjectでも呼べるように
        public static string GetFullPath(GameObject gameObject) {
            return GetFullPath(gameObject.transform);
        }

        /*
        * GameObjectのタグを 'Editor Only' にして無効化する
        */
        public static void SetEditorOnly(GameObject gameObject) {
            gameObject.tag = "EditorOnly";
            gameObject.SetActive(false);
        }
    }
}