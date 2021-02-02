using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class PrefabUtil {

        public static void UnpackPrefabOnPrefabData() {
            //Assetから直接D&Dだったらインスタンス化
            if(AssetDatabase.GetAssetPath(PrefabData.GetAvatar()) != "") {
                PrefabData.SetAvatar(PrefabUtility.InstantiatePrefab(PrefabData.GetAvatar()) as GameObject);
            }
            //Assetから直接D&Dだったらインスタンス化
            if(AssetDatabase.GetAssetPath(PrefabData.GetCloth()) != "") {
                PrefabData.SetCloth(PrefabUtility.InstantiatePrefab(PrefabData.GetCloth()) as GameObject);
            }

            UnpackPrefab(PrefabData.GetAvatar());
            UnpackPrefab(PrefabData.GetCloth());
        }
        public static void UnpackPrefab(GameObject prefab) {
            try {
                PrefabUtility.UnpackPrefabInstance(prefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            } catch(ArgumentException) {}
        }
    }
}