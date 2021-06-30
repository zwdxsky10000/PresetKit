using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using System.IO;
using UnityEditor.IMGUI.Controls;

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
            s_Instance.titleContent = new GUIContent("PresetKit");
            s_Instance.Show();
        }

        private Vector2 pos;
        [SerializeField] 
        TreeViewState m_TreeViewState;
        RuleTreeView m_RuleTreeView;

        private void OnEnable()
        {
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();

            m_RuleTreeView = new RuleTreeView(m_TreeViewState);
        }

        private void OnGUI()
        {
            DoTreeView();
            
        }

        void DoTreeView()
        {
            m_RuleTreeView.OnGUI(new Rect(0, 0, 120, position.height));
        }

        private void OnFocus()
        {
            
        }

        private void DrawRule()
        {

        }

        private void DrawEmpty()
        {
            EditorGUILayout.HelpBox("", MessageType.Info);
        }

        bool foldout = false;
        private void DrawRuleDetail(PresetObject rule)
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
                string[] files = Directory.GetFiles(rule.path);
                if(files != null && files.Length > 0)
                {
                    foreach(string file in files)
                    {
                        if (file.Contains("PresetRule") || file.Contains(".meta"))
                        {
                            continue;
                        }
                        AssetDatabase.ImportAsset(file, ImportAssetOptions.DontDownloadFromCacheServer | ImportAssetOptions.ImportRecursive);
                    }
                }
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
            string[] guids = AssetDatabase.FindAssets("t:PresetObject");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                PresetObject rule = AssetDatabase.LoadAssetAtPath<PresetObject>(path);
                rules.Add(rule);
            }
            return rules;
        }
    }

    class RuleTreeView : TreeView
    {
        SearchField m_searchField;

        public RuleTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            m_searchField = new SearchField();
            // 对TreeView进行一些设置
            rowHeight = 20;
            showBorder = true;

            Reload();
        }

        public override void OnGUI(Rect rect)
        {
            Rect srect = rect;
            srect.height = 18f;
            searchString = m_searchField.OnGUI(rect, searchString);

            rect.y += 18f;
            base.OnGUI(rect);
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            var ruleObjects = GetPresetRules();
            var allItems = new List<TreeViewItem>();
            for (int i = 0, len = ruleObjects.Count; i < len; ++i)
            {
                var item = new TreeViewItem { id = i + 1, depth = 0, displayName = ruleObjects[i].name };
                allItems.Add(item);
            }

            SetupParentsAndChildrenFromDepths(root, allItems);

            return root;
        }

        private List<PresetObject> GetPresetRules()
        {
            List<PresetObject> rules = new List<PresetObject>();
            string[] guids = AssetDatabase.FindAssets("t:PresetObject");
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

