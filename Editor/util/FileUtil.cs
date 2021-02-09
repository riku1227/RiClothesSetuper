using System;
using System.IO;
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

        /*
        * "~/"から始まる相対パスから絶対パスを取得する
        * basePathでその相対パスの始まり場所を指定する
        */
        public static string GetPathFromRelative(string basePath, string relativePath) {
            string nowPath = basePath;

            if(!Directory.Exists(basePath)) {
                return nowPath;
            }

            //"/"か"\"で文字列を分割
            string[] relativeSplit = Regex.Split(relativePath, @"\/|\\");
            for(int i = 0; i < relativeSplit.Length; i++) {
                string relativeStr = relativeSplit[i];

                //"../"で一つディレクトリを戻る
                if(relativeStr == "..") {
                    //Directory.GetParentは最後に"/"や"\"がついているとそのディレクトリを返すので、Path.GetDirectoryNameで"/"や"\"を除いている
                    nowPath = Directory.GetParent(Path.GetDirectoryName(nowPath)).FullName + "/";
                } else {
                    //ディレクトリだったら最後に"/"をつける
                    if(Directory.Exists(nowPath + relativeStr)) {
                        nowPath = nowPath + relativeStr + "/";
                    } else if (File.Exists(nowPath + relativeStr)) {
                        nowPath = nowPath + relativeStr;
                    }
                }
            }

            return nowPath;
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