using System;
using UnityEngine;
using UnityEditor;

namespace RiClothes
{
    class LogUtil
    {
        public static void ErrorOnCustomOperation(string title, string description, string optionName)
        {
            Debug.LogError("RiClothes Setuper エラー" + "<b>" + title + "</b>" + "\n"
                            + description + "\n"
                            + "オプションの名前: " + optionName);
        }

        public static void WarningOnCustomOperation(string title, string description, string optionName) {
            Debug.LogWarning(
                "RiClothes Setuper 警告" + "<b>" + title + "</b>" + "\n"
                + description + "\n" 
                + "その動作が正常な可能性もあります" + "\n"
                + "オプションの名前: " + optionName
            );
        }
    }
}