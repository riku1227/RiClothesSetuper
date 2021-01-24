using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class RiClothesSetuper: MonoBehaviour {
        [MenuItem("RiClothes/Setuper")]
        static void Setup() {
            EditorWindow.GetWindow<Setuper> (true, "RiClothes Setuper");
        }
    }
}