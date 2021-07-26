using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class RiClothesSetuper: MonoBehaviour {
        static string SETUPER_VERSION = "v2.1.0";
        [MenuItem("RiClothes/Setuper")]
        static void Setup() {
            EditorWindow.GetWindow<Setuper> (true, "RiClothes Setuper " + SETUPER_VERSION);
        }

        [MenuItem("RiClothes/DiffTextureGenerator")]
        static void DiffTextureGenerator() {
            EditorWindow.GetWindow<DiffTexGenerator> (true, "Diff Texture Generator " + SETUPER_VERSION);
        }

        [MenuItem("RiClothes/Cloth UnInstaller")]
        static void ClothUnInstaller() {
            EditorWindow.GetWindow<ClothUnInstallerWindow> (true, "RiClothes Cloth Uninstaller " + SETUPER_VERSION);
        }
    }
}