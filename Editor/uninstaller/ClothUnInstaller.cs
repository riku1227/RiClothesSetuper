using UnityEngine;

namespace RiClothes {
    /* 
    * ExpnadOptionを読み込む処理をなんやかんやするのがめんどくさいので読み込み処理があるクラスを継承
    * 闇
    */
    public class ClothUnInstaller: SetuperExpandOption {

        public ClothUnInstaller(): base() {
        }

        public void UnInstall(Transform transform, string id) {
            int removeCount = 0;
            int childCount = transform.childCount;
            for(int i = 0; i < childCount; i++) {
                Transform child = transform.GetChild(i - removeCount);
                child.gameObject.SetActive(true);
                child.gameObject.tag = "Untagged";

                if(0 < child.childCount) {
                    this.UnInstall(child, id);
                }

                if(child.name.IndexOf(id) != -1) {
                    GameObject.DestroyImmediate(child.gameObject);
                    removeCount++;
                }
            }
        }
    }
}