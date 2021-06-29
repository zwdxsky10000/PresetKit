using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;
using System.IO;
using System.Collections.Generic;

public class PresetRuleEditor : EditorWindow
{
    [MenuItem("Assets/Create/Preset", false, 0)]
    static void CreatePreset()
    {
        Object obj = Selection.activeObject;
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

    [MenuItem("Assets/Create/Preset&Rule", false, 0)]
    static void CreatePresetAndRule()
    {
        Object obj = Selection.activeObject;

        string path = AssetDatabase.GetAssetPath(obj);

        AssetImporter importer = AssetImporter.GetAtPath(path);
        Preset preset = new Preset(importer);

        string presetSavePath = EditorUtility.SaveFilePanel("保存Preset", Application.dataPath, "DefaultPreset", "preset");
        if(string.IsNullOrEmpty(presetSavePath))
        {
            EditorUtility.DisplayDialog("提示", "请选择一个正确的保存目录", "确定");
            return;
        }
        presetSavePath = FileUtil.GetProjectRelativePath(presetSavePath);
        AssetDatabase.CreateAsset(preset, presetSavePath);

        string dir = Path.GetDirectoryName(path);

        PresetRuleItem ruleItem = new PresetRuleItem();
        ruleItem.preset = preset;

        PresetRule rule = ScriptableObject.CreateInstance<PresetRule>();
        rule.path = dir;
        rule.presets = new PresetRuleItem[] { ruleItem };

        string ruleSavePath = EditorUtility.SaveFilePanel("保存PresetRule", dir, "DefaultRule", "asset");
        if (string.IsNullOrEmpty(presetSavePath))
        {
            EditorUtility.DisplayDialog("提示", "请选择一个正确的保存目录", "确定");
            return;
        }
        ruleSavePath = FileUtil.GetProjectRelativePath(ruleSavePath);
        AssetDatabase.CreateAsset(rule, ruleSavePath);

        AssetDatabase.Refresh();
        Debug.LogFormat("创建PresetRule成功! path={0}", ruleSavePath);
    }

    private static PresetRuleEditor s_Instance;

    [MenuItem("Tools/PresetEditor")]
    static void ShowWindow()
    {
       
        if(s_Instance == null)
        {
            s_Instance = EditorWindow.GetWindow<PresetRuleEditor>();
        }
        s_Instance.minSize = new Vector2(480, 600);
        s_Instance.Show();
    }

    private Vector2 pos;
    private List<PresetRule> m_AssetPresetList;

    private void OnEnable()
    {
        m_AssetPresetList = GetPresetRules();
    }

    private void OnGUI()
    {
        if (m_AssetPresetList == null || m_AssetPresetList.Count == 0)
        {
            DrawEmpty();
        }
        else
        {
            DrawRules();
        }
    }

    private void OnFocus()
    {
        m_AssetPresetList = GetPresetRules();
    }

    private void DrawEmpty()
    {
        EditorGUILayout.HelpBox("", MessageType.Info);
    }

    private void DrawRules()
    {
        pos = EditorGUILayout.BeginScrollView(pos);
        foreach (var rule in m_AssetPresetList)
        {
            DrawRule(rule);
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawRule(PresetRule rule)
    {
        EditorGUILayout.BeginVertical("box");
        GUI.enabled = false;
        EditorGUILayout.TextField("Path", rule.path);
        GUI.enabled = true;
        //rule.preset = (Preset)EditorGUILayout.ObjectField("Preset", rule.preset, typeof(Preset), false);
        //rule.recursive = EditorGUILayout.Toggle("Recursive", rule.recursive);
        //rule.regex = EditorGUILayout.TextField("Regex", rule.regex);
        //rule.extension = EditorGUILayout.TextField("Extension", rule.extension);

        Color origin = GUI.color;
        GUI.color = Color.green;
        if (GUILayout.Button("Reimport"))
        {
            AssetDatabase.ImportAsset(rule.path, ImportAssetOptions.DontDownloadFromCacheServer | ImportAssetOptions.ImportRecursive);
        }
        GUI.color = origin;
        EditorGUILayout.EndVertical();
    }

    private List<PresetRule> GetPresetRules()
    {
        List<PresetRule> rules = new List<PresetRule>();
        string[] guids = AssetDatabase.FindAssets("t:PresetRule");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PresetRule rule = AssetDatabase.LoadAssetAtPath<PresetRule>(path);
            rules.Add(rule);
        }
        return rules;
    }
}
