using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Utilities;
using System;

namespace ModDataTools.Editors
{
    [CustomPropertyDrawer(typeof(BitFieldAttribute))]
    public class BitFieldDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is BitFieldAttribute attr)
            {
                position = EditorGUI.PrefixLabel(position, label);
                position.width = 20f;
                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                for (int i = 0; i < attr.Length; i++)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
                    bool oldValue = BitFieldUtility.GetValue(property.intValue, i);
                    bool newValue = EditorGUI.Toggle(position, oldValue);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.intValue = BitFieldUtility.Truncate(BitFieldUtility.SetValue(property.intValue, i, newValue), attr.Length);
                    }
                    
                    position.x += 20f;
                }
                EditorGUI.indentLevel = indentLevel;
            }
        }
    }
}
