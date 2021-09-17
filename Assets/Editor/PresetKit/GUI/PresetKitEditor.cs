using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEditorInternal;
using UnityEngine;

namespace PresetKit
{
    public class PresetKitEditor : EditorWindow
    {
        [MenuItem("Assets/Create/PresetKit/Preset", false, 0)]
        private static void CreatePreset()
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

        [MenuItem("Assets/Create/PresetKit/PresetRule", false, 0)]
        private static void CreatePresetRule()
        {
            UnityEngine.Object obj = Selection.activeObject;

            string dir = AssetDatabase.GetAssetPath(obj);//folder

            PresetObject rule = ScriptableObject.CreateInstance<PresetObject>();
            rule.path = dir;

            string saveFile = $"{dir}/PresetRule.asset";
            AssetDatabase.CreateAsset(rule, saveFile);

            AssetDatabase.Refresh();
            Debug.LogFormat("创建PresetRule成功! path={0}", saveFile);
        }

        [MenuItem("GameKits/PresetKitEditor")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<PresetKitEditor>();
            wnd.titleContent = new GUIContent("PresetKitEditor");
            wnd.minSize = new Vector2(300, 500);
            wnd.Show();
        }

        private List<PresetObject> ruleObjects;
        private PresetObject selectObj;

        private Vector2 pos;
        private ReorderableList objReorderableList;
        private ReorderableList presetReorderableList;
        private SerializedObject so;

        private void OnEnable()
        {
            ruleObjects = new List<PresetObject>();

            string[] guids = AssetDatabase.FindAssets("t:PresetObject");
            if (guids != null && guids.Length > 0)
            {
                foreach (var guid in guids)
                {
                    var p = AssetDatabase.GUIDToAssetPath(guid);
                    var obj = AssetDatabase.LoadAssetAtPath<PresetObject>(p);
                    if (obj != null)
                    {
                        ruleObjects.Add(obj);
                    }
                }
            }
            selectObj = ruleObjects[0];

            DrawRuleList();
            DrawRuleItem();
        }

        private void OnGUI()
        {
            pos = EditorGUILayout.BeginScrollView(pos);
            objReorderableList.DoLayoutList();
            presetReorderableList?.DoLayoutList();
            EditorGUILayout.EndScrollView();
        }

        private void DrawRuleList()
        {
            objReorderableList = new ReorderableList(ruleObjects, typeof(PresetObject), true, true, false, false);
            objReorderableList.elementHeight = 30;

            //绘制Header
            objReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "PresetRuleSets");
            };

            //绘制背景
            objReorderableList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    rect.x += 2;
                    rect.width -= 4;
                    rect.y += 2;
                    rect.height -= 4;
                    EditorStyles.helpBox.Draw(rect, false, isActive, isFocused, false);
                }
            };

            //绘制元素
            objReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = ruleObjects[index];
                rect.height -= 4;
                rect.y += 2;

                EditorGUI.LabelField(rect, element.name);
            };

            objReorderableList.onSelectCallback = (ReorderableList l) => {

                selectObj = ruleObjects[l.index];
                if (selectObj != null)
                    EditorGUIUtility.PingObject(selectObj);

                DrawRuleItem();
            };
        }

        private void DrawRuleItem()
        {
            so = new SerializedObject(selectObj);
            var presetsProp = so.FindProperty("rules");
            presetReorderableList = new ReorderableList(so, presetsProp, true, true, false, false);
            presetReorderableList.elementHeight = 70;

            //绘制Header
            presetReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "PresetRules");
            };

            //绘制背景
            presetReorderableList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    rect.x += 2;
                    rect.width -= 4;
                    rect.y += 2;
                    rect.height -= 4;
                    EditorStyles.helpBox.Draw(rect, false, isActive, isFocused, false);
                }
            };

            //绘制元素
            presetReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = presetsProp.GetArrayElementAtIndex(index);
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.PropertyField(rect, element);
            };
        }
    }
}

