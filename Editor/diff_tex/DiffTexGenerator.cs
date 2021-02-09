using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class DiffTexGenerator: EditorWindow {
        Vector2 scrollPosition = Vector2.zero;
        GameObject prevClothPrefab = null;
        string clothPrefabParentPath = "";

        private V1.DiffTexGeneratorProcess V1DiffTextGen;

        void OnDestroy() {
            PrefabData.ClearPrefabData();
            I18N.Instance().ResetText();
        }

        void OnGUI() {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            PrefabData.SetCloth(EditorGUILayout.ObjectField( I18N.Instance().Get("option.input.cloth"), PrefabData.GetCloth(), typeof(GameObject), true ) as GameObject);
            //prevとの比較が必要な処理: 開始
            SetupDiffTexGen();
            //prevとの比較が必要な処理: 終了
            prevClothPrefab = PrefabData.GetCloth();

            DiffTexOnGUI();

            EditorGUILayout.EndScrollView();
        }

        void SetupDiffTexGen() {
            if(PrefabData.GetCloth() == null) {
                return;
            }

            if(prevClothPrefab == PrefabData.GetCloth()) {
                return;
            }

            LoadDiffTexture();
            I18N.Instance().LoadLanguage(clothPrefabParentPath);
        }

        void LoadDiffTexture() {
            ClearDiffTexture();
            if(PrefabData.GetCloth() == null) {
                return;
            }

            clothPrefabParentPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(PrefabData.GetCloth());
            if(clothPrefabParentPath == "") {
                return;
            }

            string basePath = Path.GetDirectoryName(clothPrefabParentPath) + "/";
            OptionPath optionPath = OptionPath.LoadOptionPathFromBaseDir(basePath);

            //プレハブ直下もしくは"RiClothesSetuper/" にOptionPathのjsonファイルを置くパターン
            if(optionPath != null) {
                if(optionPath.option_json_path != null && optionPath.option_json_path != "") {
                    string optionJsonPath = "";
                    if(optionPath.option_json_path.IndexOf("~/") == 0) {
                        //プレハブからの相対パス
                        optionJsonPath = FileUtil.GetPathFromRelative(basePath, optionPath.option_json_path.Replace("~/", ""));
                    } else {
                        //Assetsからの絶対パス
                        optionJsonPath = FileUtil.GetBasePath() + optionPath.option_json_path;
                    }
                    V1.ExpandOption expandOption = FileUtil.LoadJsonFile<V1.ExpandOption>(optionJsonPath);
                    string outputPath = basePath;
                    if(optionPath.base_path != null && optionPath.base_path != "") {
                        if(optionPath.base_path.IndexOf("~/") == 0) {
                            outputPath = FileUtil.GetPathFromRelative(basePath, optionPath.base_path.Replace("~/", ""));
                        } else {
                            outputPath = optionPath.base_path;
                        }
                    }
                    if(expandOption != null) {
                        V1DiffTextGen = new V1.DiffTexGeneratorProcess(expandOption.difference_textures, outputPath);
                    }
                } else if (optionPath.base_path != null && optionPath.base_path != "") {
                    string optionBasePath = "";
                    if(optionPath.base_path.IndexOf("~/") == 0) {
                        //プレハブからの相対パス
                        optionBasePath = FileUtil.GetPathFromRelative(basePath, optionPath.base_path.Replace("~/", ""));
                    } else {
                        optionBasePath = FileUtil.GetBasePath() + optionPath.base_path;
                    }

                    LoadDiffTexOnBasePath(optionBasePath);
                }
            }

            //この時点でロードできていたら終了
            if(V1DiffTextGen != null) {
                return;
            }

            //プレハブ直下もしくは"RiClothesSetuper/"にOption.jsonを置くパターン
            if(clothPrefabParentPath != "") {
                LoadDiffTexOnBasePath(basePath);
            }
        }

        void LoadDiffTexOnBasePath(string basePath) {
            string jsonPath = "";
            if(File.Exists(basePath + "Option.json")) {
                jsonPath = basePath + "Option.json";
            } else if(Directory.Exists(basePath + "RiClothesSetuper/")) {
                jsonPath = basePath + "RiClothesSetuper/" + "Option.json";
            }

            V1.ExpandOption expandOption = FileUtil.LoadJsonFile<V1.ExpandOption>(jsonPath);
            V1DiffTextGen = new V1.DiffTexGeneratorProcess(expandOption.difference_textures, basePath);
        }

        void DiffTexOnGUI() {
            if(V1DiffTextGen != null) {
                V1DiffTextGen.OnGUI();
            }
        }

        void ClearDiffTexture() {
            V1DiffTextGen = null;
        }
    }
}