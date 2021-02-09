using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    namespace V1 {
        public class DiffTexGeneratorProcess {
            private DifferenceTexture[] differenceTextures;
            private string outputPath;
            
            private GUIStyle titleLabelStyle;
            public DiffTexGeneratorProcess(DifferenceTexture[] _differenceTextures, string _outputPath) {
                differenceTextures = _differenceTextures;
                outputPath = _outputPath;

                //タイトルに使用するラベル用のスタイルを定義
                titleLabelStyle = new GUIStyle();
                titleLabelStyle.fontSize = 18;
            }

            public void OnGUI() {
                if(differenceTextures == null) {
                    return;
                }
                for(int i = 0; i < differenceTextures.Length; i++) {
                    DifferenceTexture differenceTexture = differenceTextures[i];

                    GUILayout.Label(I18N.Instance().Get(differenceTexture.texture_name), titleLabelStyle);
                    if(differenceTexture.parts_list != null) {
                        for(int partsCount = 0; partsCount < differenceTexture.parts_list.Length; partsCount++) {
                        DifferenceParts differenceParts = differenceTexture.parts_list[partsCount];
                        GUILayout.Label(I18N.Instance().Get(differenceParts.parts_name));
                        //SelectGrid用のリストを作る
                        List<String> selectGridList = new List<String>();

                        for(int selectGridCount = 0; selectGridCount < differenceParts.texture_list.Length; selectGridCount++) {
                            selectGridList.Add(I18N.Instance().Get(differenceParts.texture_list[selectGridCount].name));
                        }

                        differenceParts.select = GUILayout.SelectionGrid(differenceParts.select, selectGridList.ToArray(), 3);
                        GUILayout.Space(4);
                    }

                    if(GUILayout.Button(I18N.Instance().Get(differenceTexture.texture_name))) {
                        GenerateDiffTex(i);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    }

                    GUILayout.Space(16);
                }

                if(GUILayout.Button(I18N.Instance().Get("difftex.button.generate_all"))) {
                    
                }
            }

            void GenerateAllDiffTex() {
                for(int i = 0; i < differenceTextures.Length; i++) {
                    GenerateDiffTex(i);
                }
            }

            public void GenerateDiffTex(int index) {
                Texture2D baseTexture = null;
                Color[] baseTexturePixels = null;

                DifferenceTexture diffTex = differenceTextures[index];

                for (int diffPartsCount = 0; diffPartsCount < diffTex.parts_list.Length; diffPartsCount++) {
                    DifferenceParts diffParts = diffTex.parts_list[diffPartsCount];
                    String texturePath = diffParts.texture_list[diffParts.select].texture_path;

                    if (File.Exists(outputPath + texturePath)) {
                        if (baseTexture == null) {
                            baseTexture = FileUtil.PNGToTexture2D(outputPath + texturePath);
                            baseTexturePixels = baseTexture.GetPixels();
                        }
                        else {
                            Color[] diffPixels = FileUtil.PNGToTexture2D(outputPath + texturePath).GetPixels();
                            for (int pixelCount = 0; pixelCount < baseTexturePixels.Length; pixelCount++) {
                                Color diffPixel = diffPixels[pixelCount];
                                if (diffPixel.a > 0) {
                                    baseTexturePixels[pixelCount] = diffPixel;
                                }
                            }
                        }
                    }

                    if (baseTexture == null) {
                        return;
                    }

                    baseTexture.SetPixels(baseTexturePixels);
                    baseTexture.Apply();
                    String textureOutputPath = outputPath + diffTex.output_path;

                    String outputFolder = textureOutputPath.Replace(Path.GetFileName(textureOutputPath), "");
                    Directory.CreateDirectory(outputFolder);

                    File.WriteAllBytes(textureOutputPath, baseTexture.EncodeToPNG());
                }
            }
        }
    }
}
