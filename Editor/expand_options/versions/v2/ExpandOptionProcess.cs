using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    namespace V2 {
        public class ExpandOptionProcess {
            private ExpandOption expandOption;
            private string basePath;

            //削除コマンドを 'EditorOnly' にタグをセットにする
            private bool isDeleteIsEditorOnlyTag = true;

            public ExpandOptionProcess(ExpandOption _expandOption, string _basePath) {
                expandOption = _expandOption;
                basePath = _basePath;
            }

            public void OnGUI() {
                //8のマージン
                GUILayout.Space(8);

                OnExpandGUI();
            }

            private void OnExpandGUI() {
                //BeforeMoveBoneのGUI描画
                if(expandOption.before_move_bone != null) {
                    for(int i = 0; i < expandOption.before_move_bone.Length; i++) {
                        OnCustomOptionGUI(expandOption.before_move_bone[i]);
                    }
                }
                //AfterBoneのGUI描画
                if(expandOption.after_move_bone != null) {
                    for(int i = 0; i < expandOption.after_move_bone.Length; i++) {
                        OnCustomOptionGUI(expandOption.after_move_bone[i]);
                    }
                }
                //CustomOptionsのGUI描画
                if(expandOption.custom_options != null) {
                    for(int i = 0; i < expandOption.custom_options.Length; i++) {
                        OnCustomOptionGUI(expandOption.custom_options[i]);
                    }
                }
            }

            //詳細オプションの描画
            public void OnAdvancedOptionGUI() {
                isDeleteIsEditorOnlyTag = GUILayout.Toggle(isDeleteIsEditorOnlyTag, I18N.Instance().Get("option.toggle.delete_is_set_editor_only_tag"));
                GUIUtil.RenderLabel(I18N.Instance().Get("option.toggle.delete_is_set_editor_only_tag.description"));
            }

            //引数のCustomOptionをGUIで描画する
            private void OnCustomOptionGUI(CustomOption customOption) {
                if(!customOption.visible_option) {
                    return;
                }

                switch(customOption.option_type.ToLower()) {
                    case "toggle":
                        customOption.is_check = GUILayout.Toggle(customOption.is_check, " " + I18N.Instance().Get(customOption.name));
                        if(customOption.description != null && customOption.description != "") {
                            GUIUtil.RenderLabel(I18N.Instance().Get(customOption.description));
                        }
                    break;

                    case "grid":
                        GUILayout.Label(I18N.Instance().Get(customOption.name));
                        if(customOption.description != null && customOption.description != null) {
                            GUIUtil.RenderLabel(I18N.Instance().Get(customOption.description));
                        }

                        //GUI用の文字列リストを作る
                        List<string> selectGridList = new List<string>();
                        //CustomOptionのgrid_listから名前を取り出し文字列リストに追加
                        for(int selectGridCount = 0; selectGridCount < customOption.grid_list.Length; selectGridCount++) {
                            selectGridList.Add(I18N.Instance().Get(customOption.grid_list[selectGridCount].name));
                        }

                        //GUI表示。処理するときにはselect(index)を使ってCustomOptionのgrid_listから取り出す
                        customOption.select = GUILayout.SelectionGrid(customOption.select, selectGridList.ToArray(), 3);
                        GUILayout.Space(1);
                    break;
                }

                //適用ボタン表示のフィルター処理
                bool filterPassed = false;

                if(customOption.apply_filter != null && PrefabData.GetAvatar() != null) {
                    for(int filterCount = 0; filterCount < customOption.apply_filter.Length; filterCount++ ) {
                        Transform filterObject = PrefabData.GetAvatar().transform.Find(customOption.apply_filter[filterCount]);
                        if(filterObject != null) {
                            filterPassed = true;
                        }
                    }
                }

                if(filterPassed) {
                    GUILayout.Space(4);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button(I18N.Instance().Get("option.button.apply") + ": " + I18N.Instance().Get(customOption.name), new GUILayoutOption[] {
                        GUILayout.MinWidth(200)
                    })) {
                        ProcessCustomOption(customOption);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(3);
            }

            /*
            * ボーンを移動する前に実行される
            */
            public void BeforeMoveBone() {
                if(expandOption.before_move_bone == null) {
                    return;
                }

                for(int i = 0; i < expandOption.before_move_bone.Length; i++) {
                    ProcessCustomOption(expandOption.before_move_bone[i]);
                }
            }

            /*
            * ボーンが移動した後に実行される
            */
            public void AfterMoveBone() {
                if(expandOption.after_move_bone == null) {
                    return;
                }

                for(int i = 0; i < expandOption.after_move_bone.Length; i++) {
                    ProcessCustomOption(expandOption.after_move_bone[i]);
                }
            }

            /*
            * Setuper側の処理が終わったあとに実行される
            */
            public void AfterSetuperProcess() {
                if(expandOption.custom_options == null) {
                    return;
                }

                for(int i = 0; i < expandOption.custom_options.Length; i++) {
                    ProcessCustomOption(expandOption.custom_options[i]);
                }
            }

            private void ProcessCustomOption(CustomOption customOption) {
                switch(customOption.option_type.ToLower()) {
                    case "toggle":
                        if(customOption.operation_list == null) {
                            return;
                        }
                        for(int i = 0; i < customOption.operation_list.Length; i++) {
                            ProcessCustomOperation(customOption.operation_list[i], customOption.is_check);
                        }
                    break;

                    case "grid":
                        if(customOption.grid_list == null) {
                            return;
                        }

                        CustomGridOption customGridOption = customOption.grid_list[customOption.select];
                        for(int i = 0; i < customGridOption.operation_list.Length; i++) {
                            ProcessCustomOperation(customGridOption.operation_list[i], true);
                        }
                    break;
                }
            }

            private void ProcessCustomOperation(CustomOperation customOperation, bool isCheck) {
                switch(customOperation.operation_type.ToUpper()) {
                    case "DELETE_OBJECT":
                        if(!isCheck) {
                            return;
                        }

                        for(int i = 0; i < customOperation.target_object_list.Length; i++) {
                            string targetObjectName = customOperation.target_object_list[i];
                            if(!customOperation.is_avatar_object) {
                                targetObjectName = AppendID(targetObjectName);
                            }

                            Transform target = PrefabData.GetAvatar().transform.Find(targetObjectName);
                            if(target == null) {
                                return;
                            }
                            if(isDeleteIsEditorOnlyTag) {
                                GameObjectUtil.SetEditorOnly(target.gameObject);
                            } else {
                                GameObject.DestroyImmediate(target.gameObject);
                            }
                        }
                    break;

                    case "NOT_DELETE_OBJECT":
                        if(isCheck) {
                            return;
                        }

                        for(int i = 0; i < customOperation.target_object_list.Length; i++) {
                            string targetObjectName = customOperation.target_object_list[i];
                            if(!customOperation.is_avatar_object) {
                                targetObjectName = AppendID(targetObjectName);
                            }

                            Transform target = PrefabData.GetAvatar().transform.Find(targetObjectName);
                            if(target == null) {
                                return;
                            }
                            if(isDeleteIsEditorOnlyTag) {
                                GameObjectUtil.SetEditorOnly(target.gameObject);
                            } else {
                                GameObject.DestroyImmediate(target.gameObject);
                            }
                        }
                    break;

                    case "ENABLE_OBJECT":
                        if(!isCheck) {
                            return;
                        }

                        for(int i = 0; i < customOperation.target_object_list.Length; i++) {
                            string targetObjectName = customOperation.target_object_list[i];
                            if(!customOperation.is_avatar_object) {
                                targetObjectName = AppendID(targetObjectName);
                            }

                            Transform target = PrefabData.GetAvatar().transform.Find(targetObjectName);
                            if(target != null) {
                                target.gameObject.SetActive(true);
                            }
                        }
                    break;

                    case "DISABLE_OBJECT":
                        if(!isCheck) {
                            return;
                        }

                        for(int i = 0; i < customOperation.target_object_list.Length; i++) {
                            string targetObjectName = customOperation.target_object_list[i];
                            if(!customOperation.is_avatar_object) {
                                targetObjectName = AppendID(targetObjectName);
                            }

                            Transform target = PrefabData.GetAvatar().transform.Find(targetObjectName);
                            if(target != null) {
                                target.gameObject.SetActive(false);
                            }
                        }
                    break;

                    case "SET_MATERIAL":
                        if(!isCheck) {
                            return;
                        }

                        string assetsPath = Path.GetDirectoryName(basePath) + "/";
                        string matPath = FileUtil.GetPathFromRelative(assetsPath, customOperation.argument);
                        //AssetDatabase.LoadAssetAtPathは'C:example/'などのフルパスじゃなくて'Assets/Example'じゃないといけない
                        matPath = FileUtil.RemoveBasePath(matPath);
                        Material material = AssetDatabase.LoadAssetAtPath<Material>(matPath);

                        if(material != null) {
                            for(int i = 0; i < customOperation.target_object_list.Length; i++) {
                                string targetObjectName = customOperation.target_object_list[i];
                                if(!customOperation.is_avatar_object) {
                                    targetObjectName = AppendID(targetObjectName);
                                }

                                Transform target = PrefabData.GetAvatar().transform.Find(targetObjectName);
                                if(target != null) {
                                    SkinnedMeshRenderer meshRenderer = target.gameObject.GetComponent<SkinnedMeshRenderer>();
                                    if(meshRenderer != null) {
                                        meshRenderer.material = material;
                                    }
                                }
                            }
                        }
                    break;

                    //MoveObjectは服側を参照するため、IDを付ける必要はない
                    case "MOVE_OBJECT":
                        if(!isCheck) {
                            return;
                        }
                        //move_objectが無い、空の場合は何もしない
                        if(customOperation.move_object_list != null && customOperation.move_object_list.Length > 0) {
                            for(int i = 0; i < customOperation.move_object_list.Length; i++) {
                                MoveObject moveObject = customOperation.move_object_list[i];

                                //移動させるオブジェクト
                                Transform fromObject = PrefabData.GetCloth().transform.Find(moveObject.from);
                                //移動先のオブジェクト (移動先オブジェクトの子要素になる)
                                Transform toObject = PrefabData.GetAvatar().transform.Find(moveObject.to);

                                if(fromObject != null && toObject != null) {
                                    //そのオブジェクトと子オブジェクトにIDをつける
                                    AppendIDToObject(fromObject);

                                    fromObject.SetParent(toObject);
                                }
                            }
                        }
                    break;

                    case "NONE":
                    break;
                }
            }

            //valueの後ろにIDを入れて返す
            public string AppendID(string value) {
                return value + "_" + expandOption.id;
            }

            /*
            * ボーンの名前にIDをつける
            * 子ボーンにも付ける
            */
            private void AppendIDToObject(Transform bone) {
                if(bone.childCount > 0) {
                    for(int i = 0; i < bone.childCount; i++) {
                        AppendIDToObject(bone.GetChild(i));
                    }
                }

                bone.name = AppendID(bone.name);
            }

            /*
            * そのオブジェクトがExpandOptionで除外リストに入っているかをチェックする
            * 入っていたらtrueを返す
            */
            public bool CheckExcludeObject(Transform transform) {
                bool result = false;
                if(expandOption.exclude_object_list == null) {
                    return false;
                }
                string fullPath = GameObjectUtil.GetFullPath(transform);
                for(int i = 0; i < expandOption.exclude_object_list.Length; i++) {
                    string excludeObject = expandOption.exclude_object_list[i];

                    /*
                    * スラッシュがある場合はフルパス比較
                    */
                    if(excludeObject.IndexOf("/") != -1) {
                        if(fullPath == excludeObject) {
                            result = true;
                        }
                    }
                    /*
                    * **がある場合はオブジェクトの名前にその文字が含まれているか
                    */
                    else if(excludeObject.IndexOf("**") == 0) {
                        if(transform.name.IndexOf(excludeObject.Replace("**", "")) != -1) {
                            result = true;
                        }
                    }
                    /*
                    * ***がある場合はフルパスの中にその文字が含まれているか
                    */
                    else if (excludeObject.IndexOf("***") == 0) {
                        if(fullPath.IndexOf(excludeObject.Replace("**", "")) != -1) {
                            result = true;
                        }
                    }
                    /*
                    * 特になにもない場合はそのオブジェクトの名前と比較
                    */
                    else {
                        if(transform.name == excludeObject) {
                            result = true;
                        }
                    }
                }
                return result;
            }

            /*
            * IDを返す
            */
            public string GetID() {
                return expandOption.id;
            }
        }
    }
}