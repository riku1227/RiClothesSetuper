using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    namespace V2 {
        public class DiffTexGeneratorProcess {
            private DifferenceTextureGenerator differenceTextureGenerator;
            private string basePath;

            private GUIStyle styleTextureNameLabel;
            private GUIStyle stylePartsNameLabel;

            public DiffTexGeneratorProcess(DifferenceTextureGenerator _differenceTextureGenerator, string _basePath) {
                differenceTextureGenerator = _differenceTextureGenerator;
                basePath = _basePath;
                if(differenceTextureGenerator.base_path != null && differenceTextureGenerator.base_path != "") {
                    basePath = FileUtil.GetPathFromRelative(_basePath, differenceTextureGenerator.base_path.Replace("~/", ""));
                }

                /*
                * テクスチャの名前に使用されるラベルのスタイル
                * ---------------------
                * フォントサイズ: 14
                * フォントスタイル: Bold
                * マージン: 左4, 残り0
                * ---------------------
                */
                styleTextureNameLabel = new GUIStyle();
                styleTextureNameLabel.fontSize = 16;
                styleTextureNameLabel.fontStyle = FontStyle.Bold;
                styleTextureNameLabel.margin = new RectOffset(4, 0, 0, 0);

                /*
                * パーツの名前に使用されるラベルのスタイル
                */
                stylePartsNameLabel = new GUIStyle();
                stylePartsNameLabel.fontSize = 14;
                stylePartsNameLabel.fontStyle = FontStyle.Bold;
                stylePartsNameLabel.margin = new RectOffset(4, 0, 0, 0);
            }

            public void OnGUI() {
                if(differenceTextureGenerator.difference_texture_list == null) {
                    return;
                }

                OnDiffTexGUI();

                GUILayout.Space(12);
                if(GUILayout.Button(I18N.Instance().Get("difftex.button.generate_all"))) {
                    GenerateAllDiffTex();
                }
            }

            private void OnDiffTexGUI() {
                for(int i = 0; i < differenceTextureGenerator.difference_texture_list.Length; i++) {
                    if(i >= 1) {
                        GUILayout.Space(8);
                    }
                    DifferenceTexture diffTex = differenceTextureGenerator.difference_texture_list[i];

                    GUILayout.Label("・" + I18N.Instance().Get(diffTex.name) , styleTextureNameLabel);
                    if(diffTex.description != null && diffTex.description != "") {
                        GUIUtil.RenderLabel(I18N.Instance().Get(diffTex.description));
                    }

                    for(int partsListCount = 0; partsListCount < diffTex.parts_list.Length; partsListCount++) {
                        DifferenceParts diffParts = diffTex.parts_list[partsListCount];
                        GUILayout.Space(2);

                        GUILayout.Label(I18N.Instance().Get(diffParts.name), stylePartsNameLabel);
                        if(diffParts.description != null && diffParts.description != "") {
                            GUIUtil.RenderLabel(I18N.Instance().Get(diffParts.description));
                        }

                        List<string> selectGridList = new List<string>();
                        for(int selectGridCount = 0; selectGridCount < diffParts.texture_list.Length; selectGridCount++) {
                            selectGridList.Add(I18N.Instance().Get(diffParts.texture_list[selectGridCount].name));
                        }
                        
                        GUILayout.Space(2);
                        diffParts.select = GUILayout.SelectionGrid(diffParts.select, selectGridList.ToArray(), 3);
                    }

                    GUILayout.Space(8);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button(I18N.Instance().Get("difftex.button.generate_tex") + I18N.Instance().Get(diffTex.name))) {
                        GenerateDiffTex(diffTex);
                        UpdateAssets();
                    }
                    GUILayout.EndHorizontal();
                }
            }

            private void UpdateAssets() {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            /*
            * テクスチャを生成する
            */
            private void GenerateDiffTex(DifferenceTexture diffTex) {
                /*
                * ベースのテクスチャ2D
                * 最後にこのテクスチャ2Dに上書きしてエクスポートする
                */
                Texture2D baseTexture = null;
                /*
                * ベースのピクセル(カラー)配列
                * この配列にレイヤーを上書きしていく
                */
                Color[] baseTexturePixels = null;

                for(int i = 0; i < diffTex.parts_list.Length; i++) {
                    DifferenceParts diffParts = diffTex.parts_list[i];
                    PartsTexture partsTex = diffParts.texture_list[diffParts.select];
                    string texturePath = FileUtil.GetPathFromRelative(basePath, partsTex.texture_path);
                    
                    if(File.Exists(texturePath)) {
                        if(baseTexture == null) {
                            baseTexture = FileUtil.PNGToTexture2D(texturePath);
                            baseTexturePixels = baseTexture.GetPixels();
                        } else {
                            Color[] layerPixels = FileUtil.PNGToTexture2D(texturePath).GetPixels();
                            for(int pixelCount = 0; pixelCount < baseTexturePixels.Length; pixelCount++) {
                                Color layerPixel = layerPixels[pixelCount];
                                if(layerPixel.a > 0) {
                                    baseTexturePixels[pixelCount] = layerPixel;
                                }
                            }
                        }
                    }

                    if (baseTexture == null) {
                        return;
                    }

                    baseTexture.SetPixels(baseTexturePixels);
                    baseTexture.Apply();
                    string textureOutputPath = FileUtil.GetPathFromRelative(basePath, diffTex.output_path);

                    File.WriteAllBytes(textureOutputPath, baseTexture.EncodeToPNG());
                }
            }

            private void GenerateAllDiffTex() {
                for(int i = 0; i < differenceTextureGenerator.difference_texture_list.Length; i++) {
                    GenerateDiffTex(differenceTextureGenerator.difference_texture_list[i]);
                }
                UpdateAssets();
            }
        }
    }
}