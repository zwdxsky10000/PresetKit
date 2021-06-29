using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Presets;

[CustomEditor(typeof(PresetRule))]
public class PresetRuleInspector : Editor
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
        var presetsProp = serializedObject.FindProperty("presets");
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
        };

        presetReorderableList.onAddCallback = (ReorderableList l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);

            string path = EditorUtility.OpenFilePanel("Select a preset file", Application.dataPath, "preset");
            if(string.IsNullOrEmpty(path))
            {
                return;
            }

            path = FileUtil.GetProjectRelativePath(path);
            Preset preset = AssetDatabase.LoadAssetAtPath<Preset>(path);
            element.FindPropertyRelative("preset").objectReferenceValue = preset;
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
        rect.y += 2;
        SerializedProperty preset = element.FindPropertyRelative("preset");
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), new GUIContent("preset"), EditorStyles.boldLabel);
        EditorGUI.PropertyField(
            new Rect(rect.x + 50, rect.y, 120, EditorGUIUtility.singleLineHeight),
            preset, GUIContent.none);

        SerializedProperty regex = element.FindPropertyRelative("regex");
        EditorGUI.LabelField(new Rect(rect.x + 170, rect.y, 45, EditorGUIUtility.singleLineHeight), new GUIContent("regex"), EditorStyles.boldLabel);
        EditorGUI.PropertyField(
            new Rect(rect.x + 215, rect.y, rect.width - 285, EditorGUIUtility.singleLineHeight),
            regex, GUIContent.none);

        SerializedProperty ext = element.FindPropertyRelative("extension");
        EditorGUI.LabelField(new Rect(rect.x + rect.width - 70, rect.y, 30, EditorGUIUtility.singleLineHeight), new GUIContent("ext"), EditorStyles.boldLabel);
        EditorGUI.PropertyField(
            new Rect(rect.x + rect.width - 40, rect.y, 40, EditorGUIUtility.singleLineHeight),
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
