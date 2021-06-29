using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PresetKit
{
    [CustomEditor(typeof(PresetObject))]
    public class PresetObjectInspector : Editor
    {
        private SerializedProperty pathProperty;
        private SerializedProperty recursiveProperty;
        private ReorderableList presetReorderableList;

        private void OnEnable()
        {
            pathProperty = serializedObject.FindProperty("path");
            recursiveProperty = serializedObject.FindProperty("recursive");
            InitPresetGUI();
        }

        private void InitPresetGUI()
        {
            var presetsProp = serializedObject.FindProperty("rules");
            presetReorderableList = new ReorderableList(serializedObject, presetsProp, false, true, true, true);

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
                    EditorStyles.miniButton.Draw(rect, false, isActive, isFocused, false);
                }
            };

            //绘制元素
            presetReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = presetsProp.GetArrayElementAtIndex(index);
                DrawPresetRuleElement(element, rect);
                rect.y += 5;
            };

            presetReorderableList.onAddCallback = (ReorderableList l) =>
            {
                var index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);

                element.FindPropertyRelative("preset").objectReferenceValue = null;
                element.FindPropertyRelative("regex").stringValue = string.Empty;
                element.FindPropertyRelative("extension").stringValue = string.Empty;

                serializedObject.ApplyModifiedProperties();
            };

            //绑定remove事件
            presetReorderableList.onRemoveCallback += (list) =>
            {
                if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the wave?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    serializedObject.ApplyModifiedProperties();
                }
            };
        }

        private void DrawPresetRuleElement(SerializedProperty element, Rect rect)
        {

            SerializedProperty preset = element.FindPropertyRelative("preset");
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 40, EditorGUIUtility.singleLineHeight), new GUIContent("Preset"), EditorStyles.boldLabel);
            EditorGUI.PropertyField(
                new Rect(rect.x + 45, rect.y, 160, EditorGUIUtility.singleLineHeight),
                preset, GUIContent.none);

            SerializedProperty regex = element.FindPropertyRelative("regex");
            EditorGUI.LabelField(new Rect(rect.x + 215, rect.y, 40, EditorGUIUtility.singleLineHeight), new GUIContent("Regex"), EditorStyles.boldLabel);
            EditorGUI.PropertyField(
                new Rect(rect.x + 260, rect.y, rect.width - 355, EditorGUIUtility.singleLineHeight),
                regex, GUIContent.none);

            SerializedProperty ext = element.FindPropertyRelative("extension");
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 90, rect.y, 30, EditorGUIUtility.singleLineHeight), new GUIContent("Ext"), EditorStyles.boldLabel);
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 60, rect.y, 60, EditorGUIUtility.singleLineHeight),
                ext, GUIContent.none);
        }

        public override void OnInspectorGUI()
        {
            //绘制reorderableList
            serializedObject.Update();

            EditorGUILayout.PropertyField(pathProperty);

            EditorGUILayout.PropertyField(recursiveProperty);

            //绘制ruleList
            presetReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }

}

