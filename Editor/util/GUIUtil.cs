using System;
using UnityEngine;

namespace RiClothes {
    public class GUIUtil {
        /*
        * Labelを描画する
        * \nで改行される
        */
        public static void RenderLabel(string labelString) {
            string[] splitStr = { @"\n" };
            string[] labelSplit = labelString.Split(splitStr, StringSplitOptions.None);

            for(int i = 0; i < labelSplit.Length; i++) {
                GUILayout.Label(labelSplit[i]);
            }
        }
    }
}