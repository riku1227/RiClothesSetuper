using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class Setuper: EditorWindow {
        //EditorGUILayoutのScrollView用
        Vector2 scrollPosition = Vector2.zero;
        I18N i18N = null;

        GameObject avatarPrefab = null;
        GameObject clothPrefab = null;

        // RiClothes SetuperのGUIを描画
        void OnGUI() {
            if(i18N == null) {
                i18N = new I18N();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            avatarPrefab = EditorGUILayout.ObjectField( i18N.Get("option.input.avatar"), avatarPrefab, typeof(GameObject), true ) as GameObject;
            clothPrefab = EditorGUILayout.ObjectField( i18N.Get("option.input.cloth"), clothPrefab, typeof(GameObject), true ) as GameObject;

            //上から16のマージン
            GUILayout.Space(16);
            if(GUILayout.Button(i18N.Get("option.button.change_cloth"))) {
                //両方とも同じプレハブが入っているときは処理しないように
                if(avatarPrefab == clothPrefab) {
                    return;
                }
                UnpackPrefab();
                SetupArmature(avatarPrefab.transform.Find("Armature"), clothPrefab.transform.Find("Armature"));
                MoveClothObject();

                //残った服の残骸(プレハブ)を消す
                GameObject.DestroyImmediate (clothPrefab);
                //Missing Scriptを削除する
                MissingRemover.Remove(avatarPrefab);
                //シリアライズオブジェクトを編集した場合一回実行しないとUnityを閉じるときにクラッシュするのを対策
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }

            EditorGUILayout.EndScrollView();
        }

        //プレハブをアンパックする
        private void UnpackPrefab() {
            if(AssetDatabase.GetAssetPath(avatarPrefab) != "") {
                avatarPrefab =  PrefabUtility.InstantiatePrefab(avatarPrefab) as GameObject;
            }
            if(AssetDatabase.GetAssetPath(clothPrefab) != "") {
                avatarPrefab = PrefabUtility.InstantiatePrefab(clothPrefab) as GameObject;
            }

            try {
                PrefabUtility.UnpackPrefabInstance(avatarPrefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            } catch(ArgumentException) {}

            try {
                PrefabUtility.UnpackPrefabInstance(clothPrefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            } catch(ArgumentException) {}
        }

        //服のボーンをアバターの同名ボーンの下に移動させる
        private void SetupArmature(Transform avatarBone, Transform clothBone) {
            if(avatarBone == null || clothBone == null) {
                return;
            }

            if(clothBone.childCount > 0) {
                for(int i = 0; i < clothBone.childCount; i++) {
                    Transform clothChildBone = clothBone.GetChild(i);
                    Transform avatarChildBone = avatarBone.Find(clothChildBone.name);

                    if(avatarChildBone != null) {
                        SetupArmature(avatarChildBone, clothChildBone);
                    }
                }
            } else {
                Transform parentClothBone = clothBone.parent;
                clothBone.SetParent(avatarBone);

                if(parentClothBone != null) {
                    SetupArmature(avatarBone.parent, parentClothBone);
                }
            }
        }
        //服のオブジェクトをアバター側に移動させる
        private void MoveClothObject() {
            int clothChileCount = clothPrefab.transform.childCount;
            for (int i = 0; i < clothChileCount; i++) {
                clothPrefab.transform.GetChild (0).parent = avatarPrefab.transform;
            }
        }
    }
}