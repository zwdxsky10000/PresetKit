using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace PresetKit
{
    public class PresetKitEditor : EditorWindow
    {
        [MenuItem("Assets/Create/Custom/Preset", false, 0)]
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

        [MenuItem("Assets/Create/Custom/PresetRule", false, 0)]
        static void CreatePresetRule()
        {
            Object obj = Selection.activeObject;

            string dir = AssetDatabase.GetAssetPath(obj);//folder

            PresetObject rule = ScriptableObject.CreateInstance<PresetObject>();
            rule.path = dir;

            string saveFile = string.Format("{0}/PresetRule.asset", dir);
            AssetDatabase.CreateAsset(rule, saveFile);

            AssetDatabase.Refresh();
            Debug.LogFormat("创建PresetRule成功! path={0}", saveFile);
        }

        private static PresetKitEditor s_Instance;

        [MenuItem("Tools/PresetEditor")]
        static void ShowWindow()
        {

            if (s_Instance == null)
            {
                s_Instance = EditorWindow.GetWindow<PresetKitEditor>();
            }
            s_Instance.minSize = new Vector2(480, 500);
            s_Instance.maxSize = new Vector2(480, 500);
            s_Instance.Show();
        }

        private Vector2 pos;
        private List<PresetObject> m_AssetPresetList;

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

        bool foldout = false;
        private void DrawRule(PresetObject rule)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("Path:", GUILayout.Width(30));
            GUI.enabled = false;
            EditorGUILayout.TextField(rule.path);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            //折叠的相关控件 返回bool类型表示开、关
            foldout = EditorGUILayout.Foldout(foldout, "Rules");
            if (foldout)
            {
                foreach (var preset in rule.rules)
                {
                    DrawPreset(preset);
                }
            }

            Color origin = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("Reimport"))
            {
                AssetDatabase.ImportAsset(rule.path, ImportAssetOptions.DontDownloadFromCacheServer | ImportAssetOptions.ImportRecursive);
            }
            GUI.color = origin;
            EditorGUILayout.EndVertical();
        }

        private void DrawPreset(PresetRule rule)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("Preset:", GUILayout.Width(40));
            rule.preset = (Preset)EditorGUILayout.ObjectField(rule.preset, typeof(Preset), false, GUILayout.Width(160));
            EditorGUILayout.LabelField("Regex:", GUILayout.Width(40));
            rule.type = (EMatchType)EditorGUILayout.EnumPopup(rule.type, GUILayout.Width(70));
            EditorGUILayout.LabelField("Ext:", GUILayout.Width(25));
            rule.pattern = EditorGUILayout.TextField(rule.pattern, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();
        }

        private List<PresetObject> GetPresetRules()
        {
            List<PresetObject> rules = new List<PresetObject>();
            string[] guids = AssetDatabase.FindAssets("t:PresetRule");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                PresetObject rule = AssetDatabase.LoadAssetAtPath<PresetObject>(path);
                rules.Add(rule);
            }
            return rules;
        }
    }

}

