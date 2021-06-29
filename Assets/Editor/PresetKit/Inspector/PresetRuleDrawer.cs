using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace PresetKit
{
    [CustomPropertyDrawer(typeof(PresetRule))]
    public class PresetRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                EditorGUIUtility.labelWidth = 60;

                position.height = EditorGUIUtility.singleLineHeight;

                Rect presetRect = new Rect(position)
                {
                    width = position.width - 5,
                    y = position.y + 5
                };

                Rect typeRect = new Rect(presetRect)
                {
                    y = presetRect.y + EditorGUIUtility.singleLineHeight + 2
                };

                Rect patternRect = new Rect(typeRect)
                {
                    y = typeRect.y + EditorGUIUtility.singleLineHeight + 2
                };

                SerializedProperty presetProperty = property.FindPropertyRelative("preset");
                SerializedProperty typeProperty = property.FindPropertyRelative("type");
                SerializedProperty patternProperty = property.FindPropertyRelative("pattern");

                presetProperty.objectReferenceValue = EditorGUI.ObjectField(presetRect, presetProperty.displayName, presetProperty.objectReferenceValue, typeof(Preset), false);

                typeProperty.enumValueIndex = (int)(EMatchType)EditorGUI.EnumPopup(typeRect, typeProperty.displayName, (EMatchType)typeProperty.enumValueIndex);

                patternProperty.stringValue = EditorGUI.TextField(patternRect, patternProperty.displayName, patternProperty.stringValue);
            }  
        }
    }

}

