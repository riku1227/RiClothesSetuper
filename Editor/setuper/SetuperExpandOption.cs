using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RiClothes {
    public class SetuperExpandOption {
        private string clothPrefabParentPath = "";

        //バージョンを読み込んだあと本体を読み込むときに使う
        private string expandJsonPath;
        private bool isLoadExpandGUI = false;
        //まずバージョンだけ読み込んでそのあとの処理をバージョンによって変える
        private ExpandOptionVersion expandOptionVersion;
        private V1.ExpandOptionProcess V1ExpandOptionProcess;

        public SetuperExpandOption() {
            if(PrefabData.GetCloth() != null) {
                clothPrefabParentPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(PrefabData.GetCloth());
            }
        }

        public void OnExpandGUI() {
            GUILayout.Space(8);
            GUILayout.Label(clothPrefabParentPath);

            if(!isLoadExpandGUI) {
                //まずバージョンを読み込む
                LoadExpandFileVersion();
                LoadLanguage();
                LoadExpandOption();
            }

            ExpandOptionOnGUI();
        }

        /*
        * ボーンを移動する前に実行される
        * Setuperから呼び出し
        */
        public void BeforeMoveArmature() {
            if(V1ExpandOptionProcess != null) {
                V1ExpandOptionProcess.BeforeMoveArmature();
            }
        }

        /*
        * Setuper側の処理が終わったあとに実行される
        * Setuperからの呼び出し
        */
        public void AfterSetuperProcess() {
            if(V1ExpandOptionProcess != null) {
                V1ExpandOptionProcess.AfterSetuperProcess();
            }
        }

        //ExpandOptionのバージョンをロードする
        private void LoadExpandFileVersion() {
            expandOptionVersion = null;
            if(PrefabData.GetCloth() == null) {
                return;
            }

            isLoadExpandGUI = true;

            if(clothPrefabParentPath != "") {
                //プレハブ直下もしくは"RiClothesSetuper/" にOptionPathのjsonファイルを置くパターン
                string basePath = Path.GetDirectoryName(clothPrefabParentPath) + "/";
                OptionPath optionPath = OptionPath.LoadOptionPathFromBaseDir(basePath);

                if(optionPath != null) {
                    if (optionPath.option_json_path != null && optionPath.option_json_path != "") {
                        expandJsonPath = FileUtil.GetBasePath() + optionPath.option_json_path;
                        expandOptionVersion = FileUtil.LoadJsonFile<ExpandOptionVersion>(FileUtil.GetBasePath() + optionPath.option_json_path);
                    } else if (optionPath.base_path != null && optionPath.base_path != "") {
                        LoadOptionVersionJsonOnBasePath(FileUtil.GetBasePath() + optionPath.base_path);
                    }
                }

                //この時点でexpandOptionに中身が入っていたら終了する
                if(expandOptionVersion != null) {
                    return;
                }

                //プレハブ直下もしくは"RiClothesSetuper/"にOption.jsonを置くパターン
                if(clothPrefabParentPath != "") {
                    LoadOptionVersionJsonOnBasePath(basePath);
                }
            }
        }

        //言語ファイルを読み込みむ
        private void LoadLanguage() {
            I18N.Instance().ResetText();

            if(PrefabData.GetCloth() == null) {
                return;
            }

            bool isLoadLanguage = false;

            if(clothPrefabParentPath != "") {
                //プレハブ直下もしくは"RiClothesSetuper/" にOptionPathのjsonファイルを置くパターン
                string basePath = Path.GetDirectoryName(clothPrefabParentPath) + "/";

                OptionPath optionPath = OptionPath.LoadOptionPathFromBaseDir(basePath);

                if(optionPath != null) {
                    if(optionPath.language_dir_path != null && optionPath.language_dir_path != "") {
                        I18N.Instance().LoadTextFile(FileUtil.GetBasePath() + optionPath.language_dir_path, true);
                        isLoadLanguage = true;
                    } else if (optionPath.base_path != null && optionPath.base_path != "") {
                        I18N.Instance().LoadTextFile(FileUtil.GetBasePath() + optionPath.base_path, false);
                        isLoadLanguage = true;
                    }
                }

                //この時点でロードできていたら終了
                if(isLoadLanguage) {
                    return;
                }

                I18N.Instance().LoadTextFile(basePath, false);
            }

        }

        //ベースパス (プレハブのある場所など)からExpandOptionのバージョンをロードする
        private void LoadOptionVersionJsonOnBasePath(string basePath) {
            if(File.Exists(basePath + "Option.json")) {
                expandJsonPath = basePath + "Option.json";
                expandOptionVersion = FileUtil.LoadJsonFile<ExpandOptionVersion>(basePath + "Option.json");
            } else if (Directory.Exists(basePath + "RiClothesSetuper/")) {
                expandJsonPath = basePath + "RiClothesSetuper/" + "Option.json";
                expandOptionVersion = FileUtil.LoadJsonFile<ExpandOptionVersion>(basePath + "RiClothesSetuper/" + "Option.json");
            }
        }

        /*
        * ExpandOptionを読み込む (バージョンごとに読み込み処理を変える)
        * 先にLoadExpandFileVersionを実行してバージョンを取得して置かないと動かない
        */
        private void LoadExpandOption() {
            V1ExpandOptionProcess = null;

            if(expandOptionVersion == null) {
                return;
            }

            switch(expandOptionVersion.version) {
                case 1:
                    V1.ExpandOption v1ExpandOption = FileUtil.LoadJsonFile<V1.ExpandOption>(expandJsonPath);
                    if(v1ExpandOption != null) {
                        V1ExpandOptionProcess = new V1.ExpandOptionProcess(v1ExpandOption, clothPrefabParentPath);
                    }
                break;

                default:
                    Debug.Log("非対応のバージョンです");
                break;
            }
        }

        private void ExpandOptionOnGUI() {
            if(V1ExpandOptionProcess != null) {
                V1ExpandOptionProcess.OnGUI();
            }
        }

        public void UpdateClothPrefab() {
            isLoadExpandGUI = false;
            if(PrefabData.GetCloth() != null) {
                clothPrefabParentPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(PrefabData.GetCloth());
                LoadExpandFileVersion();
                LoadLanguage();
                LoadExpandOption();
            } else {
                clothPrefabParentPath = "";
            }
        }

        /*
        * valueの後ろにIDを入れて返す
        * ExpandOptionが無い場合は何もせずに値を返す
        */
        public string AppendID(string value) {
            string result = value;
            if(V1ExpandOptionProcess != null) {
                result = V1ExpandOptionProcess.AppendID(value);
            }

            return result;
        }
    }
}