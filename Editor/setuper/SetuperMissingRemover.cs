using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class SetuperMissingRemover {
        static public void Remove(GameObject gameObject) {
            try {
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            } catch(ArgumentException) { }
            RemoveMissingScript(gameObject);
        }

        static private void RemoveMissingScript(GameObject gameObject) {
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
                    RemoveMissingScript(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}