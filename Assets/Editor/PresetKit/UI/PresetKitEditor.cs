using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PresetKit;
using UnityEditor.Presets;


public class PresetKitEditor : EditorWindow
{
    [MenuItem("Assets/Create/Custom/Preset", false, 0)]
    static void CreatePreset()
    {
        UnityEngine.Object obj = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(obj);

        AssetImporter importer = AssetImporter.GetAtPath(path);
        Preset preset = new Preset(importer);

        string presetSavePath = EditorUtility.SaveFilePanel("保存Preset", Application.dataPath, "DefaultPreset", "preset");
        if (string.IsNullOrEmpty(presetSavePath))
        {
            EditorUtility.DisplayDialog("提示", "请选择一个正确的保存目录", "确定");
            return;
        }
        presetSavePath = FileUtil.GetProjectRelativePath(presetSavePath);
        AssetDatabase.CreateAsset(preset, presetSavePath);

        AssetDatabase.Refresh();
        Debug.LogFormat("创建Preset成功! path={0}", presetSavePath);
    }

    [MenuItem("Assets/Create/Custom/PresetRule", false, 0)]
    static void CreatePresetRule()
    {
        UnityEngine.Object obj = Selection.activeObject;

        string dir = AssetDatabase.GetAssetPath(obj);//folder

        PresetObject rule = ScriptableObject.CreateInstance<PresetObject>();
        rule.path = dir;

        string saveFile = string.Format("{0}/PresetRule.asset", dir);
        AssetDatabase.CreateAsset(rule, saveFile);

        AssetDatabase.Refresh();
        Debug.LogFormat("创建PresetRule成功! path={0}", saveFile);
    }
    
    
    [MenuItem("Game Toolkit/PresetKitEditor")]
    public static void ShowExample()
    {
        PresetKitEditor wnd = GetWindow<PresetKitEditor>();
        wnd.titleContent = new GUIContent("PresetKitEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/PresetKit/UI/PresetKitEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/PresetKit/UI/PresetKitEditor.uss");
        root.styleSheets.Add(styleSheet);
    }
}