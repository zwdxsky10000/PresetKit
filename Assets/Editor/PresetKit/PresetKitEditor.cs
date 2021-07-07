using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using System.IO;
using UnityEditor.IMGUI.Controls;
using System;

namespace PresetKit
{
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

        private static PresetKitEditor s_Instance;

        [MenuItem("Tools/PresetEditor")]
        static void ShowWindow()
        {

            if (s_Instance == null)
            {
                s_Instance = EditorWindow.GetWindow<PresetKitEditor>();
            }
            s_Instance.minSize = new Vector2(560, 500);
            s_Instance.titleContent = new GUIContent("PresetKit");
            s_Instance.Show();
        }

        private Vector2 pos;
        [SerializeField] 
        TreeViewState m_TreeViewState;
        RuleTreeView m_RuleTreeView;
        PresetObject m_selectPresetObj;

        private void OnEnable()
        {
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();

            m_RuleTreeView = new RuleTreeView(m_TreeViewState);
            m_RuleTreeView.clickedItemCallback += (obj) =>
            {
                m_selectPresetObj = obj;
            };
        }

        private void OnGUI()
        {
            DoTreeView();
            DrawDetail();
        }

        void DoTreeView()
        {
            m_RuleTreeView.OnGUI(new Rect(5, 5, 120, position.height-5));
        }

        private void OnFocus()
        {
            
        }

        private void DrawDetail()
        {
            Rect rect = new Rect(130, 5, position.width - 135, position.height-5);
            if(m_selectPresetObj == null)
            {
                rect.height = 30f;
                DrawEmpty(rect);
            }
            else
            {
                DrawRule(m_selectPresetObj, rect);
            }
        }

        private void DrawEmpty(Rect rect)
        {
            EditorGUI.HelpBox(rect, "empty", MessageType.Info);
        }

        bool foldout = false;
        private void DrawRule(PresetObject rule, Rect rect)
        {
            EditorGUI.DrawRect(rect, Color.black);

            Rect pathRect = new Rect(rect)
            {
                height = 20
            };
            EditorGUI.TextField(pathRect, "Path:", rule.path);

            //EditorGUILayout.BeginHorizontal("box");
            //EditorGUILayout.LabelField("Path:", GUILayout.Width(30));
            //GUI.enabled = false;
            //EditorGUILayout.TextField(rule.path);
            //GUI.enabled = true;
            //EditorGUILayout.EndHorizontal();

            ////折叠的相关控件 返回bool类型表示开、关
            //foldout = EditorGUILayout.Foldout(foldout, "Rules");
            //if (foldout)
            //{
            //    foreach (var preset in rule.rules)
            //    {
            //        DrawPreset(preset);
            //    }
            //}

            //Color origin = GUI.color;
            //GUI.color = Color.green;
            //if (GUILayout.Button("Reimport"))
            //{
            //    string[] files = Directory.GetFiles(rule.path);
            //    if(files != null && files.Length > 0)
            //    {
            //        foreach(string file in files)
            //        {
            //            if (file.Contains("PresetRule") || file.Contains(".meta"))
            //            {
            //                continue;
            //            }
            //            AssetDatabase.ImportAsset(file, ImportAssetOptions.DontDownloadFromCacheServer | ImportAssetOptions.ImportRecursive);
            //        }
            //    }
            //}
            //GUI.color = origin;
            //EditorGUILayout.EndVertical();
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
    }

    class RuleTreeView : TreeView
    {
        SearchField m_searchField;
        List<PresetObject> m_RuleObjects;

        public Action<PresetObject> clickedItemCallback;

        public RuleTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            m_searchField = new SearchField();
            // 对TreeView进行一些设置
            rowHeight = 20;
            showBorder = true;
            m_RuleObjects = GetPresetRules();

            Reload();
        }

        public override void OnGUI(Rect rect)
        {
            Rect srect = rect;
            srect.height = 18f;
            searchString = m_searchField.OnGUI(srect, searchString);

            rect.y += 18f;
            base.OnGUI(rect);
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            
            var allItems = new List<TreeViewItem>();
            if(m_RuleObjects != null)
            {
                for (int i = 0, len = m_RuleObjects.Count; i < len; ++i)
                {
                    var item = new TreeViewItem { id = i + 1, depth = 0, displayName = m_RuleObjects[i].name };
                    allItems.Add(item);
                }
            }

            SetupParentsAndChildrenFromDepths(root, allItems);

            return root;
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            int index = id - 1;
            if(m_RuleObjects != null && m_RuleObjects.Count > index && index >= 0)
            {
                clickedItemCallback?.Invoke(m_RuleObjects[index]);
            }
            Debug.Log("Clicled index:" + id);
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

