using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class RiClothesSetuper: MonoBehaviour {
        [MenuItem("RiClothes/Setuper")]
        static void Setup() {
            EditorWindow.GetWindow<Setuper> (true, "RiClothes Setuper");
        }

        [MenuItem("RiClothes/DiffTextureGenerator")]
        static void DiffTextureGenerator() {
            EditorWindow.GetWindow<DiffTexGenerator> (true, "Diff Texture Generator");
        }

        [MenuItem("RiClothes/Cloth UnInstaller")]
        static void ClothUnInstaller() {
            EditorWindow.GetWindow<ClothUnInstallerWindow> (true, "RiClothes Cloth Uninstaller");
        }
    }
}