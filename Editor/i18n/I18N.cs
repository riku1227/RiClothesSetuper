using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class I18N {
        private Dictionary<string, string> systemTextDictionary;
        public Dictionary<string, string> textDictionary;

        static private I18N i18N;

        public I18N() {
            systemTextDictionary = new Dictionary<string, string>();
            textDictionary = new Dictionary<string, string>();

            InitSystem();
        }

        public static I18N Instance() {
            if(i18N == null) {
                i18N = new I18N();
            }

            return i18N;
        }

        //RiClothes Setuperで使用されているテキストを読み込む
        private void InitSystem() {
            switch(Application.systemLanguage) {
                case SystemLanguage.Japanese:
                    LoadText(SystemTexts.JAPANESE, true);
                break;
                default:
                    LoadText(SystemTexts.ENGLISH, true);
                break;
            }
        }

        //isAbsolute 絶対パスかどうか
        public void LoadTextFile(string path, bool isAbsolute) {
            //言語ファイルがあるフォルダのパス
            string textFolderPath = path;

            /* 
            *  言語フォルダーのパスがプレハブのベースパス(相対パス)のとき 
            *  そのベースパスのフォルダに"RiClothesSetuper"というフォルダがあれば
            *  そのフォルダを言語フォルダーのパスにする
            */
            if(!isAbsolute && 
                Directory.Exists(textFolderPath + "RiClothesSetuper/")
            ) {
                textFolderPath = textFolderPath + "RiClothesSetuper/";
            }

            /* 
            *  言語フォルダーに "システム言語 + .txt" のファイルが有ればそのファイルを読み込む
            *  無ければ "English + .txt" のファイルを読み込む
            */
            if(File.Exists(textFolderPath + Application.systemLanguage + ".txt")) {
                LoadText(FileUtil.readText(textFolderPath + Application.systemLanguage + ".txt"));
            } else {
                LoadText(FileUtil.readText(textFolderPath + "English" + ".txt"));
            }
        }

        public void ResetText() {
            textDictionary.Clear();
        }

        //'='で分割して0をkey, 1をvalueにする
        public void LoadText(string textstring, bool isSystem = false) {
            StringReader stringReader = new StringReader(textstring);
            while(stringReader.Peek() != -1) {
                string[] split = stringReader.ReadLine().Split('=');
                //splitのサイズが2じゃない場合はスキップ
                if(split.Length == 2) {
                    this.Set(split[0], split[1], isSystem);
                }
            }
        }

        //textDictionaryにそのkeyが無ければ追加あれば上書き
        public void Set(string key, string value, bool isSystem = false) {
            if(isSystem) {
                if(!systemTextDictionary.ContainsKey(key)) {
                    systemTextDictionary.Add(key, value);
                } else {
                    systemTextDictionary[key] = value;
                }
            } else {
                if(!textDictionary.ContainsKey(key)) {
                    textDictionary.Add(key, value);
                } else {
                    textDictionary[key] = value;
                }
            }
        }

        public string Get(string key) {
            if(systemTextDictionary.ContainsKey(key)) {
                return systemTextDictionary[key];
            } else if(textDictionary.ContainsKey(key)) {
                return textDictionary[key];
            } else {
                return key;
            }
        }
    }
}