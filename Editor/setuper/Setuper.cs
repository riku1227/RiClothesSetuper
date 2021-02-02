using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class Setuper: EditorWindow {
        //EditorGUILayoutのScrollView用
        Vector2 scrollPosition = Vector2.zero;
        SetuperExpandOption setuperExpandOption;
        GameObject prevAvatarPrefab = null;
        GameObject prevClothPrefab = null;

        // RiClothes SetuperのGUIを描画
        void OnGUI() {

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            PrefabData.SetAvatar( EditorGUILayout.ObjectField( I18N.Instance().Get("option.input.avatar"), PrefabData.GetAvatar(), typeof(GameObject), true ) as GameObject );
            PrefabData.SetCloth(EditorGUILayout.ObjectField( I18N.Instance().Get("option.input.cloth"), PrefabData.GetCloth(), typeof(GameObject), true ) as GameObject);

            //prevとの比較が必要な処理: 開始
            SetupExpandOption();

            //prevとの比較が必要な処理: 終了
            //prev更新
            prevAvatarPrefab = PrefabData.GetAvatar();
            prevClothPrefab = PrefabData.GetCloth();

            //上から16のマージン
            GUILayout.Space(16);
            if(GUILayout.Button(I18N.Instance().Get("option.button.change_cloth"))) {
                if(PrefabData.GetAvatar() == null || PrefabData.GetCloth() == null) {
                    return;
                }
                //両方とも同じプレハブが入っているときは処理しないように
                if(PrefabData.GetAvatar() == PrefabData.GetCloth()) {
                    return;
                }

                PrefabUtil.UnpackPrefabOnPrefabData();
                //ボーンを移動する前に実行されるオプション
                setuperExpandOption.BeforeMoveArmature();
                SetupArmature(PrefabData.GetAvatar().transform.Find("Armature"), PrefabData.GetCloth().transform.Find("Armature"));
                MoveClothObject();

                //Setuper側の処理が終わったあとに実行
                setuperExpandOption.AfterSetuperProcess();

                //残った服の残骸(プレハブ)を消す
                GameObject.DestroyImmediate (PrefabData.GetCloth());
                //Missing Scriptを削除する
                MissingRemover.Remove(PrefabData.GetAvatar());
                //シリアライズオブジェクトを編集した場合一回実行しないとUnityを閉じるときにクラッシュするのを対策
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }

            EditorGUILayout.EndScrollView();
        }

        private void SetupExpandOption() {
            if(setuperExpandOption == null) {
                setuperExpandOption = new SetuperExpandOption();
            }

            if(setuperExpandOption != null) {
                //prevの服プレハブが現在の服プレハブが違ったらSetuperExpandOptionの服プレハブを更新
                if(prevClothPrefab != PrefabData.GetCloth()) {
                    setuperExpandOption.UpdateClothPrefab();
                }

                setuperExpandOption.OnExpandGUI();
            }
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
            int clothChileCount = PrefabData.GetCloth().transform.childCount;
            for (int i = 0; i < clothChileCount; i++) {
                PrefabData.GetCloth().transform.GetChild (0).parent = PrefabData.GetAvatar().transform;
            }
        }
    }
}