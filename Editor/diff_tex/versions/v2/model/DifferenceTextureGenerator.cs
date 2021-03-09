using System;
using System.IO;

namespace RiClothes {
    namespace V2 {
        [Serializable]
        public class DifferenceTextureGenerator {
            /*
            * 対応しているDiffTexGenのバージョン
            */
            public int version;
            /*
            * 服/プロジェクト/商品 の名前
            */
            public string name;
            /*
            * テクスチャの出力/入力のベースパス
            * 指定するパスのベースパスはDiffTexGenのjsonファイルがあるフォルダ
            * ない場合はベースパスはDiffTexGenのjsonファイルがあるフォルダになる
            */
            public string base_path;
            /*
            * 差分テクスチャのリスト
            */
            public DifferenceTexture[] difference_texture_list;
            

            static private string[] diffTexJsonFileNames = new string[] { 
                "DiffTex.json",
                "DiffTexGen.json",
                "DifferenceTexture.json",
                "DifferenceTextureGenerator.json", 
                "diff_tex.json",
                "diff_tex_gen.json",
                "difference_texture.json",
                "difference_texture_generator.json"};

            //ベースパスからDiffTexGenのjsonファイルを読み込む
            public static DiffTexGeneratorProcess LoadDiffTexGenFromBaseDir(string basePath) {
                for(int i = 0; i < diffTexJsonFileNames.Length; i++) {
                    string jsonPath = basePath + diffTexJsonFileNames[i];
                    if(File.Exists(jsonPath)) {
                        DifferenceTextureGenerator diffTexGen =  FileUtil.LoadJsonFile<DifferenceTextureGenerator>(jsonPath);
                        return new DiffTexGeneratorProcess(diffTexGen, jsonPath);
                    }

                    jsonPath = basePath + "RiClothesSetuper/" + diffTexJsonFileNames[i];
                    if(File.Exists(jsonPath)) {
                        DifferenceTextureGenerator diffTexGen =  FileUtil.LoadJsonFile<DifferenceTextureGenerator>(jsonPath);
                        return new DiffTexGeneratorProcess(diffTexGen, jsonPath);
                    }
                }

                return null;
            }
        }
    }
}