using System;
using System.IO;

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
    }
}