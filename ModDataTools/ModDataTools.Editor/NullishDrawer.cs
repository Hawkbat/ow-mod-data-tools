using ModDataTools.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModDataTools.Editor
{
    [CustomPropertyDrawer(typeof(Nullish<>), true)]
    public class NullishDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("value");
            return base.GetPropertyHeight(valueProp, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hasValueProp = property.FindPropertyRelative("hasValue");
            var valueProp = property.FindPropertyRelative("value");

            position = EditorGUI.PrefixLabel(position, label);
            if (valueProp.propertyType == SerializedPropertyType.Boolean)
            {
                DrawBoolPopup(position, hasValueProp, valueProp);
                return;
            }
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var hasValueRect = new Rect(position.x, position.y, 20f, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(hasValueRect, hasValueProp, GUIContent.none);
            if (hasValueProp.boolValue)
            {
                position.x += 20f;
                position.width -= 20f;
                EditorGUI.PropertyField(position, valueProp, GUIContent.none);
            }
            EditorGUI.indentLevel = indent;
        }

        static readonly string[] boolOptions = { "Default", "False", "True" };

        void DrawBoolPopup(Rect position, SerializedProperty hasValueProp, SerializedProperty valueProp)
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var mixed = hasValueProp.hasMultipleDifferentValues || (hasValueProp.boolValue && valueProp.hasMultipleDifferentValues);
            EditorGUI.showMixedValue = mixed;
            var current = !hasValueProp.boolValue ? 0 : valueProp.boolValue ? 2 : 1;
            EditorGUI.BeginChangeCheck();
            var selected = EditorGUI.Popup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), current, boolOptions);
            if (EditorGUI.EndChangeCheck())
            {
                hasValueProp.boolValue = selected != 0;
                valueProp.boolValue = selected == 2;
            }
            EditorGUI.showMixedValue = false;
            EditorGUI.indentLevel = indent;
        }
    }
}