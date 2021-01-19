using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace RiClothes {

    [Serializable]
    public class ClothExpandOption {
        public int version;
        public String id = "";
        public String avatar_armature = "Armature";
        public String[] unnecessary_objects;
        public OriginalBone[] original_bones;
        public CustomOption[] custom_options;
        public DifferenceTexture[] difference_textures;
    }

    [Serializable]
    public class CustomOption {
        public String option_type;
        public String operation;
        public String argment = "";
        public String name;
        public String[] target_object_list;
        public Boolean is_check = false;
        public Boolean display_apply_button = false;
        public String[] apply_filter;
        public int select = 0;
        public Boolean is_not_visible_option = false;
        public CustomGridOption[] grid_list;
        public CustomOperation[] multi_operation;
    }

    [Serializable]
    public class CustomGridOption {
        public String name;
        public String operation;
        public String argment = "";
        public String[] target_object_list;
        public CustomOperation[] multi_operation;
    }

    [Serializable]
    public class CustomOperation {
        public String operation;
        public String argment = "";
        public String[] target_object_list;
    }

    [Serializable]
    public class OriginalBone {
        public String from;
        public String to;
    }

    [Serializable]
    public class DifferenceTexture {
        public String texture_name;
        public String output_path;
        public DifferenceParts[] parts_list;
    }

    [Serializable]
    public class DifferenceParts {
        public String parts_name;
        public int select = 0;
        public PartsTexture[] texture_list;
    }

    [Serializable]
    public class PartsTexture {
        public String name;
        public String texture_path;
    }

    public class I18N {
        public Dictionary<String, String> textDictionary;

        public I18N() {
            textDictionary = new Dictionary<String, String>();
        }

        public void LoadTextFile(String basePath, bool isAbsolute) {
            String textFolderPath = basePath;
            if(!isAbsolute && Directory.Exists(textFolderPath + "RiClothesSetuper/")) {
                textFolderPath = textFolderPath + "RiClothesSetuper/";
            }

            if (File.Exists(textFolderPath + Application.systemLanguage + ".txt")) {
                LoadText(textFolderPath + Application.systemLanguage + ".txt");
            } else if (File.Exists(textFolderPath + "English" + ".txt")) {
                LoadText(textFolderPath + "English" + ".txt");
            }
        }

        public void InitSystemText() {
            String systemTexts = "";
            switch (Application.systemLanguage) {
                case SystemLanguage.Japanese:
                    systemTexts = $@"
option.toggle.delete_unnecessary_object=不要なオブジェクトを削除する
option.input.avatar=アバター
option.input.cloth=服
option.toggle.is_generate_diff_tex=テクスチャの差分を生成する
option.button.generate_diff_tex=テクスチャ差分を生成
option.button.change_clothes=着替える
option.button.apply=適用
";
                break;
                default:
                    systemTexts = $@"
option.toggle.delete_unnecessary_object=Delete unnecessary object
option.input.avatar=Avatar
option.input.cloth=Cloth
option.toggle.is_generate_diff_tex=Generate texture difference
option.button.generate_diff_tex=Generate texture diff
option.button.change_clothes=Change of clothes
option.button.apply=Apply
";
                break;
            }
            LoadText(textString: systemTexts);
        }

        private void LoadText(String textFilePath = "", String textString = "") {
            String textTemp = textString;
            if(File.Exists(textFilePath)) {
                StreamReader stream = new StreamReader(textFilePath);
                textTemp = stream.ReadToEnd();
                stream.Close();
            }
            StringReader stringReader = new StringReader(textTemp);
            while(stringReader.Peek() != -1) {
                string[] split = stringReader.ReadLine().Split('=');
                if(split.Length == 2) {
                    if(!textDictionary.ContainsKey(split[0])) {
                        textDictionary.Add(split[0], split[1]);
                    } else {
                        textDictionary[split[0]] = split[1];
                    }
                }
            }
        }

        public String GetText(String key) {
            String result = "";
            if(textDictionary.ContainsKey(key)) {
                result = textDictionary[key];
            } else {
                result = key;
            }
            return result;
        }
    }

    public class ExpandFileLoader {
        static public ClothExpandOption LoadOptionJson(String prefabFolderPath, bool isAbsolute) {
            String jsonFilePath = prefabFolderPath;
            if(!isAbsolute) {
                if(Directory.Exists(jsonFilePath + "RiClothesSetuper/")) {
                    jsonFilePath = jsonFilePath + "RiClothesSetuper/";
                }
                jsonFilePath = jsonFilePath + "Option.json";
            }

            if(File.Exists(jsonFilePath)) {
                using (StreamReader stream = new StreamReader(jsonFilePath)) {
                    String jsonStr = stream.ReadToEnd();
                    return JsonUtility.FromJson<ClothExpandOption>(jsonStr);;
                }
            } else {
                return null;
            }
        }

        static public String GetBaseFolderFromPrefab(GameObject clothPrefab) {
            String clothPrefabPath = AssetDatabase.GetAssetPath(clothPrefab);
            String result = "";
            
            if(clothPrefabPath != "") {
                String[] splitPath = clothPrefabPath.Split('/');
                String clothPrefabFolder = Path.GetFullPath( 
                    clothPrefabPath.Replace(
                        splitPath[splitPath.Length - 1],
                        "") 
                    );
                    
                if(Path.GetExtension(clothPrefabPath) == ".fbx") {
                    clothPrefabFolder = clothPrefabFolder.Replace("model\\", "").Replace("model/", "");
                }

                result = clothPrefabFolder;
            }

            return result;
        }
    }

    public class Setuper: MonoBehaviour {
        [MenuItem("RiClothes/Setuper")]
        static void Setup() {
            EditorWindow.GetWindow<SetuperGUI> (true, "RiClothes Setuper");
        }

        [MenuItem("RiClothes/DiffTextureGenerator")]
        static void DiffTextureGenerator() {
            EditorWindow.GetWindow<DiffTexGenerator> (true, "DiffTextureGenerator");
        }
    }
    public class SetuperGUI: EditorWindow {
        GameObject avatarPrefab = null;
        GameObject clothPrefab = null;
        Vector2 scrollPosition = Vector2.zero;
        I18N i18NSystem = null;
        Boolean isSetupSystemText = false;
        Boolean isLoadedClothExpandGUI = false;
        ClothExpandOption expandOption = null;

        String prefabBasePath = "";
        String avatarArmatureName = "Armature";

        //Options
        Boolean isDeleteUnnecessaryObject = true;
        Boolean isGenerateDiffTex = false;

        void OnGUI() {
            if(!isSetupSystemText) {
                i18NSystem = new I18N();
                i18NSystem.InitSystemText();
                isSetupSystemText = true;
            }
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if(!isLoadedClothExpandGUI) {
                if(clothPrefab != null) {
                    PathInfo pathInfo = clothPrefab.GetComponent<PathInfo>();
                    if(pathInfo != null) {
                        prefabBasePath = Application.dataPath + "/" + pathInfo.basePath;

                        if(pathInfo.jsonPath != "") {
                            expandOption = ExpandFileLoader.LoadOptionJson(Application.dataPath + "/" + pathInfo.jsonPath, true);
                        } else {
                            expandOption = ExpandFileLoader.LoadOptionJson(prefabBasePath, false);
                        }

                        if(pathInfo.languagePath != "") {
                            i18NSystem.LoadTextFile(Application.dataPath + "/" + pathInfo.languagePath, true);
                        } else {
                            i18NSystem.LoadTextFile(prefabBasePath, false);
                        }

                        if(expandOption != null) {
                            isLoadedClothExpandGUI = true;
                        }
                    } else {
                        String clothPrefabPath = ExpandFileLoader.GetBaseFolderFromPrefab(clothPrefab);

                        if(clothPrefabPath != "") {
                            prefabBasePath = clothPrefabPath;
                            i18NSystem.LoadTextFile(prefabBasePath, false);
                            expandOption = ExpandFileLoader.LoadOptionJson(prefabBasePath, false);
                            if(expandOption != null) {
                                isLoadedClothExpandGUI = true;
                            }
                        }
                    }
                }
            }

            OnBaseGUI();
            if(clothPrefab != null) {
                GUILayout.Space(16);
                OnCLothExpandGUI();
            }
            GUILayout.Space(60);

            OnChangeClothButton();

            EditorGUILayout.EndScrollView();
        }

        void OnCLothExpandGUI() {
            if(expandOption == null) {
                return;
            }

            if(expandOption.unnecessary_objects != null) {
                if(expandOption.unnecessary_objects.Length > 0) {
                    isDeleteUnnecessaryObject = GUILayout.Toggle(isDeleteUnnecessaryObject, i18NSystem.GetText("option.toggle.delete_unnecessary_object"));
                    GUILayout.Space(4);
                }
            }

            if(expandOption.custom_options != null) {
                for(int i = 0; i < expandOption.custom_options.Length; i++) {
                    if(!expandOption.custom_options[i].is_not_visible_option) {
                        CustomOption customOption = expandOption.custom_options[i];
                        switch (customOption.option_type.ToLower()) {
                            case "toggle":
                                customOption.is_check = GUILayout.Toggle(customOption.is_check, i18NSystem.GetText(customOption.name));
                            break;

                            case "grid":
                                GUILayout.Label(i18NSystem.GetText(customOption.name));

                                List<String> selectGridList = new List<String>();
                                for(int selectGridCount = 0; selectGridCount < customOption.grid_list.Length; selectGridCount++) {
                                    selectGridList.Add(i18NSystem.GetText(customOption.grid_list[selectGridCount].name));
                                }
                                customOption.select = GUILayout.SelectionGrid(customOption.select, selectGridList.ToArray(), 3);
                            break;
                        }

                        GUILayout.Space(4);

                        bool filterClear = false;

                        if(customOption.apply_filter != null) {
                            for(int filterCount = 0; filterCount < customOption.apply_filter.Length; filterCount++) {
                                if(avatarPrefab != null) {
                                    Transform filterObject = avatarPrefab.transform.Find(customOption.apply_filter[filterCount]);
                                    if(filterObject != null) {
                                        filterClear = true;
                                    }
                                }
                            }
                        }

                        if(customOption.display_apply_button && filterClear) {
                            GUILayout.Space(4);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if(GUILayout.Button(i18NSystem.GetText("option.button.apply") + ": " + i18NSystem.GetText(customOption.name), new GUILayoutOption[] {
                                GUILayout.MinWidth(200)
                            })) {
                                try {
                                    PrefabUtility.UnpackPrefabInstance(avatarPrefab, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                                    PrefabUtility.UnpackPrefabInstance(clothPrefab, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                                } catch (ArgumentException e) {
                                }

                                RunExpandOption(customOption);
                            }

                            EditorGUILayout.EndHorizontal();

                            GUILayout.Space(4);
                        }
                    }
                }
            }
        }

        void OnBaseGUI() {
            avatarPrefab = EditorGUILayout.ObjectField( i18NSystem.GetText("option.input.avatar") , avatarPrefab, typeof(GameObject), true) as GameObject;
            clothPrefab = EditorGUILayout.ObjectField( i18NSystem.GetText("option.input.cloth"), clothPrefab, typeof(GameObject), true) as GameObject;
        }

        void OnChangeClothButton() {
            if (GUILayout.Button ( i18NSystem.GetText("option.button.change_clothes") )) {
                if(AssetDatabase.GetAssetPath(avatarPrefab) != "") {
                    avatarPrefab = PrefabUtility.InstantiatePrefab(avatarPrefab) as GameObject;
                }
                if(AssetDatabase.GetAssetPath(clothPrefab) != "") {
                    clothPrefab = PrefabUtility.InstantiatePrefab(clothPrefab) as GameObject;
                }
                
                try {
                    PrefabUtility.UnpackPrefabInstance(avatarPrefab, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                    PrefabUtility.UnpackPrefabInstance(clothPrefab, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                } catch (ArgumentException e) {
                }

                MoveOriginalArmature();

                if(expandOption != null) {
                    avatarArmatureName = expandOption.avatar_armature;
                }
                SetupArmature(avatarPrefab.transform.Find(avatarArmatureName), clothPrefab.transform.Find(avatarArmatureName));

                SetupCloth(avatarPrefab.transform, clothPrefab.transform);

                GameObject.DestroyImmediate (clothPrefab);

                if(expandOption != null && expandOption.unnecessary_objects != null) {
                    for(int i = 0; i < expandOption.unnecessary_objects.Length; i++) {
                        GameObject gameObject = avatarPrefab.transform.Find(expandOption.unnecessary_objects[i]).gameObject;
                        if(gameObject != null) {
                            GameObject.DestroyImmediate(gameObject);
                        }
                    }
                }

                RunExpandOptions();

                RemoveMissingScript(avatarPrefab);

                EditorApplication.ExecuteMenuItem("Edit/Play");

                Close();
            }
        }

        void MoveOriginalArmature() {
            if(expandOption == null) {
                return;
            }

            if(expandOption.original_bones != null) {
                for(int i = 0; i < expandOption.original_bones.Length; i++) {
                    OriginalBone originalBone = expandOption.original_bones[i];
                    Transform fromBone = clothPrefab.transform.Find(originalBone.from);
                    Transform toBone = avatarPrefab.transform.Find(originalBone.to);
                    if(fromBone != null && toBone != null) {
                        fromBone.name = fromBone.name + "_" + expandOption.id;
                        fromBone.SetParent(toBone);
                    }
                }
            }
        }

        void SetupArmature(Transform avatarArmature, Transform clothArmature) {
            if (avatarArmature == null || clothArmature == null) {
                return;
            }

            if (clothArmature.childCount > 0) {
                for (int i = 0; i < clothArmature.childCount; i++) {
                    Transform clothChildArmature = clothArmature.GetChild (i);
                    Transform avatarChildArmature = avatarArmature.Find (clothChildArmature.name);

                    if(avatarChildArmature != null) {
                        SetupArmature (avatarChildArmature, clothChildArmature);
                    }
                }
            } else {
                Transform clothParentArmature = clothArmature.parent;
                if (clothParentArmature != null) {
                    if(expandOption.id != "") {
                        clothArmature.name = clothArmature.name + "_" + expandOption.id;
                    }

                    Transform nameObject = avatarArmature.Find(clothArmature.name);

                    if(nameObject == null) {
                        clothArmature.SetParent (avatarArmature);
                        SetupArmature (avatarArmature.parent, clothParentArmature);
                           
                    }
                }
            }
        }

        void SetupCloth(Transform avatarTransform, Transform clothTransform) {
            int clothChileCount = clothTransform.childCount;
            for (int i = 0; i < clothChileCount; i++) {
                clothTransform.transform.GetChild (0).parent = avatarTransform.transform;
            }
        }

        void RemoveMissingScript(GameObject gameObject) {
            Component[] components = gameObject.GetComponents<Component>();
            int count = 0;
            for(int i = 0;i < components.Length; i++) {
                Component component = components[i];
                if(component == null) {
                    SerializedObject sObject = new SerializedObject(gameObject);
                    SerializedProperty property = sObject.FindProperty("m_Component");
                    property.DeleteArrayElementAtIndex(i - count);
                    count++;
                    sObject.ApplyModifiedProperties();
                }
            }

            if(gameObject.transform.childCount > 0) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    RemoveMissingScript(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }

        void RunExpandOptions() {
            if(expandOption == null) {
                return;
            }
            if(expandOption.custom_options == null) {
                return;
            }
            for(int i = 0; i < expandOption.custom_options.Length; i++) {
                CustomOption customOption = expandOption.custom_options[i];
                RunExpandOption(customOption);
            }
        }

        void RunExpandOption(CustomOption customOption) {
            switch (customOption.option_type.ToLower()) {
                    case "toggle":
                    if(customOption.multi_operation != null) {
                        for(int multiOperationCount = 0; multiOperationCount < customOption.multi_operation.Length; multiOperationCount++) {
                            CustomOperation customOperation = customOption.multi_operation[multiOperationCount];

                            ProcessCustomOption(customOperation.operation, customOption.is_check, customOperation.argment, customOperation.target_object_list);                            
                        }
                    } else {
                        ProcessCustomOption(customOption.operation, customOption.is_check, customOption.argment, customOption.target_object_list);
                    }
                    break;

                    case "grid":
                        CustomGridOption customGridOption = customOption.grid_list[customOption.select];

                        if(customGridOption.multi_operation != null) {
                            for(int multiOperationCount = 0; multiOperationCount < customGridOption.multi_operation.Length; multiOperationCount++) {
                                CustomOperation customOperation = customGridOption.multi_operation[multiOperationCount];

                                ProcessCustomOption(customGridOption.multi_operation[multiOperationCount].operation, true, customOperation.argment, customOperation.target_object_list);
                            }
                        } else {
                            ProcessCustomOption(customGridOption.operation, true, customGridOption.argment, customGridOption.target_object_list);
                        } 
                    break;
                }
        }

        void ProcessCustomOption(String optionOperation, bool isCheck, String argment, String[] targetObjectList) {
            if(avatarPrefab == null) {
                return;
            }
            switch (optionOperation) {
                case "DELETE_OBJECT":
                    if(isCheck) {
                        for (int i = 0; i < targetObjectList.Length; i++) {
                            Transform target = avatarPrefab.transform.Find (targetObjectList[i]);
                            if(target != null) {
                                GameObject.DestroyImmediate(target.gameObject);
                            }
                        }
                    }
                break;

                case "NOT_DELETE_OBJECT":
                    if(!isCheck) {
                        for(int i = 0; i < targetObjectList.Length; i++) {
                            Transform target = avatarPrefab.transform.Find (targetObjectList[i]);
                            if(target != null) {
                                GameObject.DestroyImmediate(target.gameObject);
                            }
                        }
                    }
                break;

                case "ENABLE_OBJECT":
                    if(isCheck) {
                        for(int i = 0; i < targetObjectList.Length; i++) {
                            Transform target = avatarPrefab.transform.Find (targetObjectList[i]);
                            if(target != null) {
                                target.gameObject.SetActive(true);
                            }
                        }
                    }
                break;

                case "DISABLE_OBJECT":
                    if(isCheck) {
                        for(int i = 0; i < targetObjectList.Length; i++) {
                            Transform target = avatarPrefab.transform.Find (targetObjectList[i]);
                            if(target != null) {
                                target.gameObject.SetActive(false);
                            }
                        }
                    }
                break;

                case "SET_MATERIAL":
                    if(isCheck) {
                        String assetsPath = prefabBasePath.Substring(prefabBasePath.IndexOf("Assets"));
                        Material material = AssetDatabase.LoadAssetAtPath<Material>(assetsPath + argment);
                        if(material != null) {
                            for(int i = 0; i < targetObjectList.Length; i++) {
                                Transform target = avatarPrefab.transform.Find (targetObjectList[i]);
                                if(target != null) {
                                    SkinnedMeshRenderer meshRenderer = target.gameObject.GetComponent<SkinnedMeshRenderer>();
                                    if(meshRenderer != null) {
                                        meshRenderer.material = material;
                                    }
                                }
                            }
                        }
                    }
                break;

                case "NONE":
                break;
            }
        }
    }

    public class DiffTexGenerator : EditorWindow {
        public DifferenceTexture[] differenceTextures;
        public String outputPath;
        public I18N i18NSystem;
        GUIStyle titleLabelStyle;
        Vector2 scrollPosition = Vector2.zero;

        GameObject clothPrefab = null;

        bool isInit = false;
        bool isLoadExpand = false;
        void OnGUI() {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            if(differenceTextures != null) {
                if(!isInit) {
                    titleLabelStyle =  new GUIStyle();
                    titleLabelStyle.fontSize = 18;
                    isInit = true;
                }
                for(int i = 0; i < differenceTextures.Length; i++) {
                    DifferenceTexture diffTex = differenceTextures[i];
                    GUILayout.Label(i18NSystem.GetText(diffTex.texture_name), titleLabelStyle);

                    for(int partsCount = 0; partsCount < diffTex.parts_list.Length; partsCount++) {
                        DifferenceParts diffParts = diffTex.parts_list[partsCount];
                        GUILayout.Label(i18NSystem.GetText(diffParts.parts_name));
                        List<String> selectGridList = new List<String>();

                        for(int selectGridCount = 0; selectGridCount < diffParts.texture_list.Length; selectGridCount++) {
                            selectGridList.Add(i18NSystem.GetText(diffParts.texture_list[selectGridCount].name));
                        }

                        diffParts.select = GUILayout.SelectionGrid(diffParts.select, selectGridList.ToArray(), 3);
                        GUILayout.Space(4);
                    }
                    GUILayout.Space(4);
                    if(GUILayout.Button(i18NSystem.GetText(diffTex.texture_name))) {
                        GenerateDiffTex(i);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    GUILayout.Space(16);
                }

                if( GUILayout.Button(i18NSystem.GetText("option.button.generate_diff_tex")) ) {
                    GenerateAllDiffTex();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            } else {
                if(i18NSystem == null) {
                    i18NSystem = new I18N();
                    i18NSystem.InitSystemText();
                }
                clothPrefab = EditorGUILayout.ObjectField(i18NSystem.GetText("option.input.cloth"), clothPrefab, typeof(GameObject), true) as GameObject;

                if(clothPrefab != null && !isLoadExpand) {
                    PathInfo pathInfo = clothPrefab.GetComponent<PathInfo>();
                    if(pathInfo != null) {
                        outputPath = Application.dataPath + "/" + pathInfo.basePath;

                        if(pathInfo.jsonPath != "") {
                            differenceTextures = ExpandFileLoader.LoadOptionJson(Application.dataPath + "/" + pathInfo.jsonPath, true).difference_textures;
                        } else {
                            differenceTextures = ExpandFileLoader.LoadOptionJson(outputPath, false).difference_textures;
                        }

                        if(pathInfo.languagePath != "") {
                            i18NSystem.LoadTextFile(Application.dataPath + "/" + pathInfo.languagePath, true);
                        } else {
                            i18NSystem.LoadTextFile(outputPath, false);
                        }

                        if(differenceTextures != null) {
                            isLoadExpand = true;
                        }
                    } else {
                        String clothPrefabPath = ExpandFileLoader.GetBaseFolderFromPrefab(clothPrefab);
                        if(clothPrefabPath != "") {
                            outputPath = clothPrefabPath;
                            i18NSystem.LoadTextFile(outputPath, false);
                            differenceTextures = ExpandFileLoader.LoadOptionJson(outputPath, false).difference_textures;
                            if(differenceTextures != null) {
                                isLoadExpand = true;
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        void GenerateAllDiffTex() {
            for(int i = 0; i < differenceTextures.Length; i++) {
                GenerateDiffTex(i);
            }
        }

        void GenerateDiffTex(int index) {
            Texture2D baseTexture = null;
            Color[] baseTexturePixels = null;

            DifferenceTexture diffTex = differenceTextures[index];

            for(int diffPartsCount = 0; diffPartsCount < diffTex.parts_list.Length; diffPartsCount++) {
                DifferenceParts diffParts = diffTex.parts_list[diffPartsCount];
                String texturePath = diffParts.texture_list[diffParts.select].texture_path;

                if(File.Exists(outputPath + texturePath)) {
                    if(baseTexture == null) {
                        baseTexture = PNGToTexture2D(outputPath + texturePath);
                        baseTexturePixels = baseTexture.GetPixels();
                    } else {
                        Color[] diffPixels = PNGToTexture2D(outputPath + texturePath).GetPixels();
                        for(int pixelCount = 0; pixelCount < baseTexturePixels.Length; pixelCount++) {
                            Color diffPixel = diffPixels[pixelCount];
                            if(diffPixel.a > 0) {
                                baseTexturePixels[pixelCount] = diffPixel;
                            }
                        }
                    }
                }
            }

            if(baseTexture == null) {
                return;
            }

            baseTexture.SetPixels(baseTexturePixels);
            baseTexture.Apply();
            String textureOutputPath = outputPath + diffTex.output_path;

            String outputFolder = textureOutputPath.Replace(Path.GetFileName(textureOutputPath), "");
            Directory.CreateDirectory(outputFolder);

            File.WriteAllBytes(textureOutputPath, baseTexture.EncodeToPNG());
        }

        Texture2D PNGToTexture2D(String path) {
            byte[] value;
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    value = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                }
            }

            var pos = 16;
            int width = 0;
            for (int i = 0; i < 4; i++) {
                width = width * 256 + value[pos++];
            }

            int height = 0;
            for (int i = 0; i < 4; i++) {
                height = height * 256 + value[pos++];
            }



            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(value);
            return texture;
        }
    }
}