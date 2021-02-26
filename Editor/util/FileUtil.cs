using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class FileUtil {

        /* そのパスを文字列tとして読み込んでその文字列を返す
        *  ファイルが存在しない場合は空の文字列を返す
        */
        public static string readText(String filePath) {
            String result = "";
            if(File.Exists(filePath)) {
                StreamReader stream = new StreamReader(filePath);
                result = stream.ReadToEnd();
                stream.Close();
            }

            return result;
        }

        public static T LoadJsonFile <T>(string jsonFilePath) {
            if(!File.Exists(jsonFilePath)) {
                return default(T);
            }
            string jsonStr = readText(jsonFilePath);
            return JsonUtility.FromJson<T>(jsonStr);
        }

        public static string GetBasePath() {
            return Application.dataPath + "/";
        }

        public static string RemoveBasePath(string value) {
            string replaceToSlash = value.Replace(@"\", "/");
            return "Assets/" + replaceToSlash.Replace(Application.dataPath + "/", "");
        }

        /*
        * "~/"から始まる相対パスから絶対パスを取得する
        * basePathでその相対パスの始まり場所を指定する
        */
        public static string GetPathFromRelative(string basePath, string relativePath) {
            List<string> nowPath = new List<string>( Regex.Split(basePath, @"\/|\\") );
            /* 
            * basePathの最後が'/'(ディレクトリ)のとき空の文字列が配列の最後に入るのを消す
            * relativePathは最後がディレクトリのときに'/'をつけないと行けないので、消さない
            */

            if(nowPath[nowPath.Count -1] == "") {
                nowPath.RemoveAt(nowPath.Count -1);
            }

            string resultPath = "";

            if(relativePath == "") {
                return basePath;
            }

            //"/"か"\"で文字列を分割
            string[] relativeSplit = Regex.Split(relativePath, @"\/|\\");
            for(int i = 0; i < relativeSplit.Length; i++) {
                string relativeStr = relativeSplit[i];

                //"../"で一つディレクトリを戻る
                if(relativeStr == "..") {
                    nowPath.RemoveAt(nowPath.Count - 1);
                }
                else {
                    nowPath.Add(relativeStr);
                }
            }

            for(int i = 0; i < nowPath.Count; i++) {
                string pathTemp = nowPath[i];
                if(i > 0) {
                    resultPath += "/";
                }
                resultPath += pathTemp;
            }

            return resultPath;
        }

        public static Texture2D PNGToTexture2D(String path) {
            byte[] value;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    value = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                }
            }

            int pos = 16;
            int width = 0;
            for (int i = 0; i < 4; i++) {
                width = width * 256 + value[pos++];
            }

            int height = 0;
            for (int i = 0; i < 4; i++) {
                height = height * 256 + value[pos++];
            }

            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(value);
            return texture;
        }
    }
}