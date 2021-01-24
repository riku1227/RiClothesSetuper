using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class I18N {
        public Dictionary<string, string> textDictionary;

        public I18N() {
            textDictionary = new Dictionary<string, string>();

            InitSystem();
        }

        //RiClothes Setuperで使用されているテキストを読み込む
        private void InitSystem() {
            switch(Application.systemLanguage) {
                case SystemLanguage.Japanese:
                    LoadText(SystemTexts.JAPANESE);
                break;
                default:
                    LoadText(SystemTexts.ENGLISH);
                break;
            }
        }

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
                FileUtil.readText(textFolderPath + Application.systemLanguage + ".txt");
            } else {
                FileUtil.readText(textFolderPath + "English" + ".txt");
            }
        }

        //'='で分割して0をkey, 1をvalueにする
        public void LoadText(string textstring) {
            StringReader stringReader = new StringReader(textstring);
            while(stringReader.Peek() != -1) {
                string[] split = stringReader.ReadLine().Split('=');
                //splitのサイズが2じゃない場合はスキップ
                if(split.Length == 2) {
                    this.Set(split[0], split[1]);
                }
            }
        }

        //textDictionaryにそのkeyが無ければ追加あれば上書き
        public void Set(string key, string value) {
            if(!textDictionary.ContainsKey(key)) {
                textDictionary.Add(key, value);
            } else {
                textDictionary[key] = value;
            }
        }

        public string Get(string key) {
            string result = key;
            if(textDictionary.ContainsKey(key)) {
                result = textDictionary[key];
            }
            return result;
        }
    }
}