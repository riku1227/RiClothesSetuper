using System;
using UnityEngine;

namespace RiClothes {
    public class PrefabData {
        private static GameObject avatarPrefab;
        private static GameObject clothPrefab;

        public static void Set(GameObject _avatatPrefab, GameObject _clothPrefab) {
            avatarPrefab = _avatatPrefab;
            clothPrefab = _clothPrefab;
        }

        public static void SetAvatar(GameObject _avatarPrefab) {
            avatarPrefab = _avatarPrefab;
        }

        public static void SetCloth(GameObject _clothPrefab) {
            clothPrefab = _clothPrefab;
        }

        public static GameObject GetAvatar() {
            return avatarPrefab;
        }

        public static GameObject GetCloth() {
            return clothPrefab;
        }
    }
}