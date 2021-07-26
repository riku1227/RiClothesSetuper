using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class SetuperMissingRemover {
        static public void Remove(GameObject gameObject) {
            try {
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            } catch(ArgumentException) { }
            #if UNITY_2018
                RemoveMissingScript_2018(gameObject);
            #endif
            #if UNITY_2019_1_OR_NEWER
                RemoveMissingScript_2019(gameObject);
            #endif
        }

        /*
        * Unity 2019でMissing Componentを削除する処理
        */
        static private void RemoveMissingScript_2019(GameObject gameObject) {
            //Unity 2019でMissing Scirptを削除する機能が追加された
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
            //子オブジェクトのMissing Scriptを削除するために再起実行する
            if(gameObject.transform.childCount > 0) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    RemoveMissingScript_2019(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }

        /*
        * Unity 2018でMissing Componentを削除する処理
        */
        static private void RemoveMissingScript_2018(GameObject gameObject) {
            //そのGameObjectについてるコンポーネントを取得
            Component[] components = gameObject.GetComponents<Component>();
            int count = 0;
            for(int i = 0; i < components.Length; i++) {
                Component component = components[i];
                //コンポーネントがnullなら (Missing Scriptはnullになる)
                if(component == null) {
                    //そのオブジェクトをシリアライズ化
                    SerializedObject sObject = new SerializedObject(gameObject);
                    //m_Componentを削除する
                    SerializedProperty property = sObject.FindProperty("m_Component");
                    //２つ以上Missing Scriptがある時消すと次のindexがずれるからcountで補正
                    property.DeleteArrayElementAtIndex(i - count);
                    count++;
                    sObject.ApplyModifiedProperties();
                }
            }

            if(gameObject.transform.childCount > 0) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    RemoveMissingScript_2018(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}