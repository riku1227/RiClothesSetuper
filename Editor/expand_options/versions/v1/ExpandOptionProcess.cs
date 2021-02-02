using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    namespace V1 {
        public class ExpandOptionProcess {
            private ExpandOption expandOption;
            private string clothPrefabParentPath;

            private bool isDeleteUnnecessaryObject = true;
            private bool showAdvancedOption = false;
            private bool isDeleteIsEditorOnlyTag = true;

            public ExpandOptionProcess(ExpandOption _expandOption, string _clothPrefabParentPath) {
                expandOption = _expandOption;
                clothPrefabParentPath = _clothPrefabParentPath;
            }

            public void OnGUI() {
                //8のマージン
                GUILayout.Space(8);

                //不要なオブジェクトを削除
                if(expandOption.unnecessary_objects != null) {
                    if(expandOption.unnecessary_objects.Length > 0) {
                        isDeleteUnnecessaryObject = GUILayout.Toggle(isDeleteUnnecessaryObject, I18N.Instance().Get("option.toggle.is_delete_unnecessary_object"));
                    }
                }

                OnCustomOptionGUI();

                GUILayout.Space(4);
                showAdvancedOption = GUILayout.Toggle(showAdvancedOption, I18N.Instance().Get("option.toggle.show_advanced_option"));
                if(showAdvancedOption) {
                    GUILayout.Space(4);
                    isDeleteIsEditorOnlyTag = GUILayout.Toggle(isDeleteIsEditorOnlyTag, I18N.Instance().Get("option.toggle.delete_is_set_editor_only_tag"));
                }
            }

            //ExpandOptionのGUI処理
            private void OnCustomOptionGUI() {
                if(expandOption.custom_options == null) {
                    return;
                }

                for(int i = 0; i < expandOption.custom_options.Length; i++) {
                    CustomOption customOption = expandOption.custom_options[i];

                    //オプションが非表示の場合
                    if(customOption.is_not_visible_option) {
                        return;
                    }

                    GUILayout.Space(4);

                    switch(customOption.option_type.ToLower()) {
                        case "toggle":
                            customOption.is_check = GUILayout.Toggle(customOption.is_check, I18N.Instance().Get(customOption.name));
                        break;

                        case "grid":
                            GUILayout.Label(I18N.Instance().Get(customOption.name));

                            //GUI用の文字列リストを作る
                            List<string> selectGridList = new List<string>();
                            //CustomOptionのgrid_listから名前を取り出し文字列リストに追加
                            for(int selectGridCount = 0; selectGridCount < customOption.grid_list.Length; selectGridCount++) {
                                selectGridList.Add(I18N.Instance().Get(customOption.grid_list[selectGridCount].name));
                            }
                            //GUI表示。処理するときにはselect(index)を使ってCustomOptionのgrid_listから取り出す
                            customOption.select = GUILayout.SelectionGrid(customOption.select, selectGridList.ToArray(), 3);
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

                    if(customOption.display_apply_button && filterPassed) {
                        GUILayout.Space(4);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button(I18N.Instance().Get("option.button.apply") + ": " + I18N.Instance().Get(customOption.name), new GUILayoutOption[] {
                            GUILayout.MinWidth(200)
                        })) {
                            //RunExpandOption(customOption);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            /*
            * ボーンを移動する前に実行される
            * SetuperExpandOptionから呼び出し
            */
            public void BeforeMoveArmature() {
                if(expandOption.original_bones == null) {
                    return;
                }
                for(int i = 0; i < expandOption.original_bones.Length; i++) {
                    OriginalBone originalBone = expandOption.original_bones[i];

                    Transform fromBone = PrefabData.GetCloth().transform.Find(originalBone.from);
                    Transform toBone = PrefabData.GetAvatar().transform.Find(originalBone.to);

                    if(fromBone != null && toBone != null) {
                        fromBone.SetParent(toBone);
                    }
                }
            }
        
            /*
            * Setuper側の処理が終わったあとに実行される
            * SetuperExpandOptionから呼び出し
            */
            public void AfterSetuperProcess() {
                //不要なオブジェクトを削除
                if(isDeleteUnnecessaryObject) {
                    for(int i = 0; i < expandOption.unnecessary_objects.Length; i++) {
                        GameObject unnecessaryObject = PrefabData.GetAvatar().transform.Find(expandOption.unnecessary_objects[i]).gameObject;
                        if(unnecessaryObject != null) {
                            GameObject.DestroyImmediate(unnecessaryObject);
                        }
                    }
                }

                if(expandOption.custom_options == null) {
                    return;
                }

                for(int i = 0; i < expandOption.custom_options.Length; i++) {
                    CustomOption customOption = expandOption.custom_options[i];
                    RunCustomOption(customOption);
                }
            }

            public void RunCustomOption(CustomOption customOption) {
                Debug.Log("RunCustomOption");
                switch(customOption.option_type.ToLower()) {
                    case "toggle":
                        if(customOption.multi_operation != null) {
                            for(int i = 0; i < customOption.multi_operation.Length; i++) {
                                CustomOperation customOperation = customOption.multi_operation[i];

                                ProcessCustomOption(customOperation.operation, true, customOperation.argment, customOperation.target_object_list);
                            }
                        } else {
                            ProcessCustomOption(customOption.operation, customOption.is_check, customOption.argment, customOption.target_object_list);
                        }
                    break;

                    case "grid":
                        CustomGridOption customGridOption = customOption.grid_list[customOption.select];

                        if(customGridOption.multi_operation != null) {
                            for(int multiOperationCount = 0; multiOperationCount < customGridOption.multi_operation.Length; multiOperationCount++) {
                                CustomOperation customOperation = customGridOption.multi_operation[multiOperationCount];

                                ProcessCustomOption(customOperation.operation, true, customOperation.argment, customOperation.target_object_list);
                            }
                        } else {
                            ProcessCustomOption(customGridOption.operation, true, customGridOption.argment, customGridOption.target_object_list);
                        }
                    break;
                }
            }

            public void ProcessCustomOption(String optionOperation, bool isCheck, String argument, String[] targetObjectList) {
                switch (optionOperation.ToUpper()) {
                    case "DELETE_OBJECT":
                        if(isCheck) {
                            for(int i = 0; i < targetObjectList.Length; i++) {
                                Transform target = PrefabData.GetAvatar().transform.Find(targetObjectList[i]);
                                if(target != null) {
                                    if(isDeleteIsEditorOnlyTag) {
                                        target.gameObject.tag = "EditorOnly";
                                        target.gameObject.SetActive(false);
                                    } else {
                                        GameObject.DestroyImmediate(target.gameObject);
                                    }
                                }
                            }
                        }
                    break;

                    case "NOT_DELETE_OBJECT":
                        if(!isCheck) {
                            for(int i = 0; i < targetObjectList.Length; i++) {
                                Transform target = PrefabData.GetAvatar().transform.Find(targetObjectList[i]);
                                if(target != null) {
                                    if(isDeleteUnnecessaryObject) {
                                        target.tag = "EditorOnly";
                                        target.gameObject.SetActive(false);   
                                    } else {
                                        GameObject.DestroyImmediate(target.gameObject);
                                    }
                                }
                            }
                        }
                    break;
                    
                    case "ENABLE_OBJECT":
                    if(isCheck) {
                        for(int i = 0; i < targetObjectList.Length; i++) {
                            Transform target = PrefabData.GetAvatar().transform.Find(targetObjectList[i]);
                            if(target != null) {
                                target.gameObject.SetActive(true);
                            }
                        }
                    }
                    break;

                    case "DISABLE_OBJECT":
                        if(isCheck) {
                        for(int i = 0; i < targetObjectList.Length; i++) {
                            Transform target = PrefabData.GetAvatar().transform.Find(targetObjectList[i]);
                            if(target != null) {
                                target.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;

                    case "SET_MATERIAL":
                        if(isCheck) {
                            string assetsPath = Path.GetDirectoryName(clothPrefabParentPath) + "/";
                            Debug.Log(assetsPath);
                            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetsPath + argument);
                            if(material != null) {
                                for(int i = 0; i < targetObjectList.Length; i++) {
                                    Transform target = PrefabData.GetAvatar().transform.Find (targetObjectList[i]);
                                    if(target != null) {
                                        SkinnedMeshRenderer meshRenderer = target.gameObject.GetComponent<SkinnedMeshRenderer>();
                                        if(meshRenderer != null) {
                                            meshRenderer.material = material;
                                        }
                                    }
                                }
                            }
                        }
                    break;

                    case "NONE":
                    break;
                }
            }
        }
    }
}