using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PresetKit
{
    [CustomEditor(typeof(PresetObject))]
    public class PresetObjectInspector : Editor
    {
        private SerializedProperty pathProperty;
        private ReorderableList presetReorderableList;
        private Vector2 position;

        private void OnEnable()
        {
            pathProperty = serializedObject.FindProperty("path");
            InitPresetGUI();
        }

        private void InitPresetGUI()
        {
            var presetsProp = serializedObject.FindProperty("rules");
            presetReorderableList = new ReorderableList(serializedObject, presetsProp, true, true, true, true);
            presetReorderableList.elementHeight = 70;

            //绘制Header
            presetReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, presetsProp.displayName);
            };

            //绘制背景
            presetReorderableList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    EditorStyles.helpBox.Draw(rect, false, isActive, isFocused, false);
                }
            };

            //绘制元素
            presetReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = presetsProp.GetArrayElementAtIndex(index);
                rect.height -= 4;
                rect.y += 3;
                EditorGUI.PropertyField(rect, element);
            };

            presetReorderableList.onAddCallback = (ReorderableList l) =>
            {
                var index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);

                element.FindPropertyRelative("preset").objectReferenceValue = null;
                element.FindPropertyRelative("type").enumValueIndex = (int)EMatchType.Regex;
                element.FindPropertyRelative("pattern").stringValue = string.Empty;

                serializedObject.ApplyModifiedProperties();
            };

            //绑定remove事件
            presetReorderableList.onRemoveCallback += (list) =>
            {
                if (!EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the wave?", "Yes",
                    "No")) return;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                serializedObject.ApplyModifiedProperties();
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            position = EditorGUILayout.BeginScrollView(position);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path:",GUILayout.MaxWidth(35f));
            pathProperty.stringValue = EditorGUILayout.TextField(pathProperty.stringValue);
            EditorGUILayout.EndHorizontal();
            

            presetReorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            Color origin = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("Reimporter"))
            {
                AssetDatabase.ImportAsset(pathProperty.stringValue, 
                    ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            }
            GUI.color = origin;
            serializedObject.ApplyModifiedProperties();
        }
    }
}

