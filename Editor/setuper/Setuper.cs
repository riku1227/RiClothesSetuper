using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class Setuper: EditorWindow {
        //EditorGUILayoutのScrollView用
        Vector2 scrollPosition = Vector2.zero;
        I18N i18N = null;

        GameObject avatarPrefab = null;
        GameObject clothPrefab = null;

        // RiClothes SetuperのGUIを描画
        void OnGUI() {
            if(i18N == null) {
                i18N = new I18N();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            avatarPrefab = EditorGUILayout.ObjectField( i18N.Get("option.input.avatar"), avatarPrefab, typeof(GameObject), true ) as GameObject;
            clothPrefab = EditorGUILayout.ObjectField( i18N.Get("option.input.cloth"), clothPrefab, typeof(GameObject), true ) as GameObject;

            EditorGUILayout.EndScrollView();
        }
    }
}