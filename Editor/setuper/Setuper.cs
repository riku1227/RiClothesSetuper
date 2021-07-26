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

        //詳細オプションの表示
        private bool showAdvancedOption = false;
        private bool isRemoveMissingScript = true;

        void OnDestroy() {
            PrefabData.ClearPrefabData();
            I18N.Instance().ResetText();
        }

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

            OnAdvancedOptionGUI();

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
                //ボーンを移動した後に実行されるオプション
                setuperExpandOption.AfterMoveBone();

                MoveClothObject();

                //Setuper側の処理が終わったあとに実行
                setuperExpandOption.AfterSetuperProcess();

                //残った服の残骸(プレハブ)を消す
                GameObject.DestroyImmediate (PrefabData.GetCloth());

                //Missing Scriptを削除するオプションが有効のときMissing Scriptを削除する
                if(isRemoveMissingScript) {
                    SetuperMissingRemover.Remove(PrefabData.GetAvatar());
                }
                #if UNITY_2018
                    //シリアライズオブジェクトを編集した場合一回実行しないとUnityを閉じるときにクラッシュするのを対策
                    EditorApplication.ExecuteMenuItem("Edit/Play");
                #endif
                Close();
            }

            EditorGUILayout.EndScrollView();
        }

        void OnAdvancedOptionGUI() {
            if(PrefabData.GetCloth() == null) {
                return;
            }

            GUILayout.Space(4);
            showAdvancedOption = GUILayout.Toggle(showAdvancedOption, I18N.Instance().Get("option.toggle.show_advanced_option"));
            GUILayout.Space(2);

            if(!showAdvancedOption) {
                return;
            }

            isRemoveMissingScript = GUILayout.Toggle(isRemoveMissingScript, I18N.Instance().Get("option.toggle.is_remove_missing_script"));
            GUIUtil.RenderLabel(I18N.Instance().Get("option.toggle.is_remove_missing_script.description"));
            GUILayout.Space(2);

            setuperExpandOption.OnExpandAdvancedOptionGUI();
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

        /*
        * 服のボーンをアバターの同名ボーンの下に移動させる
        * avatarBone: アバター側のボーン
        * clothBone: 服側のボーン
        * dosableChild: trueのとき、ボーンの子要素を無視する (再起実行されない)
        */
        private void SetupArmature(Transform avatarBone, Transform clothBone, bool disableChild = false) {
            if(avatarBone == null || clothBone == null) {
                return;
            }

            if(clothBone.childCount > 0 && !disableChild) {
                for(int i = 0; i < clothBone.childCount; i++) {
                    Transform clothChildBone = clothBone.GetChild(i);
                    Transform avatarChildBone = avatarBone.Find(clothChildBone.name);

                    //ボーンに子ボーンがあればそれも移動させる
                    if(avatarChildBone != null) {
                        SetupArmature(avatarChildBone, clothChildBone);
                    }
                }
            } else {
                Transform parentClothBone = clothBone.parent;
                bool setDisableChild = false;

                if(!setuperExpandOption.CheckExcludeObject(clothBone)) {
                    //ボーンにIDを追加, ExpandOptionが無い場合は何もしない
                    clothBone.name = setuperExpandOption.AppendID(clothBone.name);
                    //ボーンの親をアバター側の同名ボーンにすることでアバターの同名ボーンに入れる
                    clothBone.SetParent(avatarBone);
                } else {
                    setDisableChild = true;
                }

                //親ボーンがあればそのボーンも移動させる
                if(parentClothBone != null) {
                    SetupArmature(avatarBone.parent, parentClothBone, setDisableChild);
                }
            }
        }
        //服のオブジェクトをアバター側に移動させる
        private void MoveClothObject() {
            int clothChileCount = PrefabData.GetCloth().transform.childCount;
            //除外するとindexがずれるからそれを補正する用
            int skipCount = 0;
            for (int i = 0; i < clothChileCount; i++) {
                Transform clothObject = PrefabData.GetCloth().transform.GetChild(skipCount);
                //除外オブジェクトの場合は補正用にカウントを進める
                if(setuperExpandOption.CheckExcludeObject(clothObject)) {
                    skipCount++;
                } else {
                    //V2はオブジェクトにもIDをつける
                    if(setuperExpandOption.GetExpandOptionVersion() == 2) {
                        clothObject.name = setuperExpandOption.AppendID(clothObject.name);
                    }
                    clothObject.SetParent(PrefabData.GetAvatar().transform);
                }
            }
        }
    }
}