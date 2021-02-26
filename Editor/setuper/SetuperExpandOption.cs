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
        private V2.ExpandOptionProcess V2ExpandOptionProcess;

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
                I18N.Instance().LoadLanguage(clothPrefabParentPath);
                LoadExpandOption();
            }

            ExpandOptionOnGUI();
        }

        public void OnExpandAdvancedOptionGUI() {
            if(V2ExpandOptionProcess != null) {
                V2ExpandOptionProcess.OnAdvancedOptionGUI();
            } else if (V1ExpandOptionProcess != null) {
                V1ExpandOptionProcess.OnAdvancedOptionGUI();
            }
        }

        /*
        * ボーンを移動する前に実行される
        * Setuperから呼び出し
        */
        public void BeforeMoveArmature() {
            if(V2ExpandOptionProcess != null) {
                V2ExpandOptionProcess.BeforeMoveBone();
            } else if(V1ExpandOptionProcess != null) {
                V1ExpandOptionProcess.BeforeMoveArmature();
            }
        }

        /*
        * ボーンが移動した後に実行される
        * Setuperからの呼び出し
        * V2から実装
        */
        public void AfterMoveBone() {
            if(V2ExpandOptionProcess != null) {
                V2ExpandOptionProcess.AfterMoveBone();
            }
        }

        /*
        * Setuper側の処理が終わったあとに実行される
        * Setuperからの呼び出し
        */
        public void AfterSetuperProcess() {
            if(V2ExpandOptionProcess != null) {
                V2ExpandOptionProcess.AfterSetuperProcess();
            } else if(V1ExpandOptionProcess != null) {
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
                        //プレハブからの相対パス
                        expandJsonPath = FileUtil.GetPathFromRelative(basePath, optionPath.option_json_path);
                        expandOptionVersion = FileUtil.LoadJsonFile<ExpandOptionVersion>(expandJsonPath);
                    } else if (optionPath.base_path != null && optionPath.base_path != "") {
                        string optionBasePath = FileUtil.GetPathFromRelative(basePath, optionPath.base_path);

                        LoadOptionVersionJsonOnBasePath(optionBasePath);
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
            V2ExpandOptionProcess = null;

            if(expandOptionVersion == null) {
                return;
            }

            switch(expandOptionVersion.version) {
                case 1:
                    V1.ExpandOption v1ExpandOption = FileUtil.LoadJsonFile<V1.ExpandOption>(expandJsonPath);
                    if(v1ExpandOption != null) {
                        //V1のExpandOption内で使用されるパスのベースは服のプレハブがあるベースのフォルダになる
                        V1ExpandOptionProcess = new V1.ExpandOptionProcess(v1ExpandOption, clothPrefabParentPath);
                    }
                break;

                case 2:
                    V2.ExpandOption v2ExpandOption = FileUtil.LoadJsonFile<V2.ExpandOption>(expandJsonPath);
                    if(v2ExpandOption != null) {
                        //V2のExpandOption内で使用されるパスのベースはそのJSONファイルがあるフォルダになる
                        V2ExpandOptionProcess = new V2.ExpandOptionProcess(v2ExpandOption, Path.GetDirectoryName(expandJsonPath) + "/");
                    }
                break;

                default:
                    Debug.Log("非対応のバージョンです");
                break;
            }
        }

        private void ExpandOptionOnGUI() {
            if(V2ExpandOptionProcess != null) {
                V2ExpandOptionProcess.OnGUI();
            } else if(V1ExpandOptionProcess != null) {
                V1ExpandOptionProcess.OnGUI();
            }
        }

        public void UpdateClothPrefab() {
            isLoadExpandGUI = false;
            if(PrefabData.GetCloth() != null) {
                clothPrefabParentPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(PrefabData.GetCloth());
                LoadExpandFileVersion();
                I18N.Instance().LoadLanguage(clothPrefabParentPath);
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
            if(V2ExpandOptionProcess != null) {
                result = V2ExpandOptionProcess.AppendID(value);
            } else if(V1ExpandOptionProcess != null) {
                result = V1ExpandOptionProcess.AppendID(value);
            }

            return result;
        }

        /*
        * そのオブジェクトがExpandOptionで除外リストに入っているかをチェックする
        * 入っていたらtrueを返す
        */
        public bool CheckExcludeObject(Transform transform) {
            if(V2ExpandOptionProcess != null) {
                return V2ExpandOptionProcess.CheckExcludeObject(transform);
            } else {
                return false;
            }
        }

        /*
        * ExpandOptionのバージョンを返す
        * ExpandOptionがない場合は0を返す
        */
        public int GetExpandOptionVersion() {
            int version = 0;
            if(V2ExpandOptionProcess != null) {
                version = 2;
            } else if (V1ExpandOptionProcess != null) {
                version = 1;
            }

            return version;
        }
    }
}