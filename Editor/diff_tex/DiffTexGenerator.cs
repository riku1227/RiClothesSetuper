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
        private V2.DiffTexGeneratorProcess V2DiffTextGen;

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

            string basePath = FileUtil.GetBasePath().Replace("Assets/", "") + Path.GetDirectoryName(clothPrefabParentPath) + "/";
            OptionPath optionPath = OptionPath.LoadOptionPathFromBaseDir(basePath);

            TryLoadV2(basePath, optionPath);
            //V2の読み込みができていたら終了
            if(V2DiffTextGen != null) {
                return;
            }

            //プレハブ直下もしくは"RiClothesSetuper/" にOptionPathのjsonファイルを置くパターン
            if(optionPath != null) {
                //DiffText V1の読み込み
                if(optionPath.option_json_path != null && optionPath.option_json_path != "") {
                    string optionJsonPath = "";
                    optionJsonPath = FileUtil.GetPathFromRelative(basePath, optionPath.option_json_path);
                    V1.ExpandOption expandOption = FileUtil.LoadJsonFile<V1.ExpandOption>(optionJsonPath);
                    string outputPath = basePath;
                    if(optionPath.base_path != null && optionPath.base_path != "") {
                        outputPath = FileUtil.GetPathFromRelative(basePath, optionPath.base_path);
                    }
                    if(expandOption != null) {
                        V1DiffTextGen = new V1.DiffTexGeneratorProcess(expandOption.difference_textures, outputPath);
                    }
                } else if (optionPath.base_path != null && optionPath.base_path != "") {
                    string optionBasePath = FileUtil.GetPathFromRelative(basePath, optionPath.base_path);

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

        /*
        * V2の読み込み処理
        */
        void TryLoadV2(string basePath, OptionPath optionPath) {
            if(optionPath != null) {
                if(optionPath.diff_tex_json_path != null && optionPath.diff_tex_json_path != "") {
                    string diffTexJsonPath = diffTexJsonPath = FileUtil.GetPathFromRelative(basePath, optionPath.diff_tex_json_path);

                    V2.DifferenceTextureGenerator diffTex = FileUtil.LoadJsonFile<V2.DifferenceTextureGenerator>(diffTexJsonPath);
                    if(diffTex != null) {
                        V2DiffTextGen = new V2.DiffTexGeneratorProcess(diffTex, Path.GetDirectoryName(diffTexJsonPath));
                    }
                } else if (optionPath.base_path != null && optionPath.base_path != "") {
                    string diffTexBasePath = diffTexBasePath = FileUtil.GetPathFromRelative(basePath, optionPath.base_path);

                    V2DiffTextGen = V2.DifferenceTextureGenerator.LoadDiffTexGenFromBaseDir(diffTexBasePath);
                }
            }

            if(V2DiffTextGen != null) {
                return;
            }

            if(clothPrefabParentPath != "") {
                V2DiffTextGen = V2.DifferenceTextureGenerator.LoadDiffTexGenFromBaseDir(basePath);
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
            if(V2DiffTextGen != null) {
                V2DiffTextGen.OnGUI();
            } else if(V1DiffTextGen != null) {
                V1DiffTextGen.OnGUI();
            }
        }

        void ClearDiffTexture() {
            V2DiffTextGen = null;
            V1DiffTextGen = null;
        }
    }
}