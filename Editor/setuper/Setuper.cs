using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class Setuper: EditorWindow {
        //EditorGUILayoutのScrollView用
        Vector2 scrollPosition = Vector2.zero;

        GameObject avatarPrefab = null;
        GameObject clothPrefab = null;

        // RiClothes SetuperのGUIを描画
        void OnGUI() {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            avatarPrefab = EditorGUILayout.ObjectField( "Avatar", avatarPrefab, typeof(GameObject), true ) as GameObject;
            clothPrefab = EditorGUILayout.ObjectField( "Cloth", clothPrefab, typeof(GameObject), true ) as GameObject;

            EditorGUILayout.EndScrollView();
        }
    }
}