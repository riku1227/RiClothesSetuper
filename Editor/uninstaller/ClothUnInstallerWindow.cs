using UnityEngine;
using UnityEditor;

namespace RiClothes {
    public class ClothUnInstallerWindow: EditorWindow {
        private ClothUnInstaller clothUnInstaller;
        private GameObject prevCloth;
        private GUIStyle styleTitleStyle;
        void Awake() {
            /*
            * Windowsのタイトルに使用するラベルのスタイル
            * フォントサイズ: 16
            * フォントスタイル: Bold
            * マージン: 左4, 他0
            */
            styleTitleStyle = new GUIStyle();
            styleTitleStyle.fontSize = 16;
            styleTitleStyle.fontStyle = FontStyle.Bold;
            styleTitleStyle.margin = new RectOffset(5, 0, 0, 0);
        }

        void OnGUI() {
            if(clothUnInstaller == null) {
                clothUnInstaller = new ClothUnInstaller();
            }

            GUILayout.Space(2);
            GUILayout.Label("Cloth UnInstaller", styleTitleStyle);
            GUIUtil.RenderLabel(I18N.Instance().Get("cloth_uninstaller.description"));

            GUILayout.Space(8);

            PrefabData.SetAvatar( EditorGUILayout.ObjectField( I18N.Instance().Get("option.input.avatar"), PrefabData.GetAvatar(), typeof(GameObject), true ) as GameObject );
            PrefabData.SetCloth(EditorGUILayout.ObjectField( I18N.Instance().Get("option.input.cloth"), PrefabData.GetCloth(), typeof(GameObject), true ) as GameObject);

            if(prevCloth != PrefabData.GetCloth()) {
                clothUnInstaller.UpdateClothPrefab();
                prevCloth = PrefabData.GetCloth();
            }

            //Setuper V2以上必須
            if(clothUnInstaller.GetExpandOptionVersion() <= 1 && PrefabData.GetCloth() != null) {
                GUILayout.Space(4);
                GUIUtil.RenderLabel(I18N.Instance().Get("cloth_uninstaller.un_support_version"));
                return;
            }

            if(PrefabData.GetAvatar() == null || PrefabData.GetCloth() == null) {
                return;
            }

            GUILayout.Space(12);
            if(GUILayout.Button(I18N.Instance().Get("cloth_uninstaller.uninstall_button"))) {
                string id = clothUnInstaller.GetExpandOptionID();
                if(id == "") {
                    return;
                }
                clothUnInstaller.UnInstall(PrefabData.GetAvatar().transform, id);
            }
        }
    }
}