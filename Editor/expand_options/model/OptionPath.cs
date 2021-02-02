using System;
using System.IO;
using UnityEngine;

namespace RiClothes {
    [Serializable]
    public class OptionPath {
        public string base_path;
        public string option_json_path;
        public string language_dir_path;

        static private string[] pathJsonFileNames = new string[] { "path.json", "Path.json", "option_path.json", "OptionPath.json", "file_path.json", "FilePath.json" };

        public static OptionPath LoadOptionPathFromBaseDir(string baseDir) {
            for(int i = 0; i < pathJsonFileNames.Length; i++) {
                string jsonPath = baseDir + pathJsonFileNames[i];
                if(File.Exists(jsonPath)) {
                    return FileUtil.LoadJsonFile<OptionPath>(jsonPath);
                }

                //{prefab}/RiClothesSetuper/{json} でも読み込めるように
                jsonPath = baseDir + "RiClothesSetuper/" + pathJsonFileNames[i];
                if(File.Exists(jsonPath)) {
                    return FileUtil.LoadJsonFile<OptionPath>(jsonPath);
                }
            }
            return null;
        }
    }
}