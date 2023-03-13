using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Utilities;

namespace ModDataTools.Editors
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (attribute is ConditionalFieldAttribute conditional && !IsConditionTrue(property, conditional)) return -EditorGUIUtility.standardVerticalSpacing;
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is ConditionalFieldAttribute conditional && !IsConditionTrue(property, conditional)) return;
            EditorGUI.PropertyField(position, property, label, true);
        }

        bool IsConditionTrue(SerializedProperty property, ConditionalFieldAttribute conditional)
        {
            var path = property.propertyPath;
            var parentPath = path.Contains(".") ? path.Substring(0, path.LastIndexOf('.') + 1) : "";
            var prop = property.serializedObject.FindProperty(parentPath + conditional.Field);
            if (prop == null)
            {
                Debug.LogError("Could not find property named: " + conditional.Field);
                return false;
            }
            if (conditional.Values != null)
            {
                foreach (var value in conditional.Values)
                {
                    switch (prop.propertyType)
                    {
                        case SerializedPropertyType.Boolean:
                            if (prop.boolValue == (bool)value) return true;
                            break;
                        case SerializedPropertyType.Enum:
                        case SerializedPropertyType.Integer:
                            if (prop.intValue == (int)value) return true;
                            break;
                        case SerializedPropertyType.Float:
                            if (prop.floatValue == (float)value) return true;
                            break;
                        case SerializedPropertyType.String:
                            if (prop.stringValue == (string)value) return true;
                            break;
                        case SerializedPropertyType.ObjectReference:
                            if (prop.objectReferenceValue == (Object)value) return true;
                            break;
                        default:
                            Debug.LogError("Unsupported property type: " + prop.propertyType);
                            return false;
                    }
                }
                return false;
            }
            else
            {
                switch (prop.propertyType)
                {
                    case SerializedPropertyType.Boolean:
                        return prop.boolValue;
                    case SerializedPropertyType.Enum:
                    case SerializedPropertyType.Integer:
                        return prop.intValue > 0;
                    case SerializedPropertyType.Float:
                        return prop.floatValue > 0;
                    case SerializedPropertyType.String:
                        return !string.IsNullOrEmpty(prop.stringValue);
                    case SerializedPropertyType.ObjectReference:
                        return prop.objectReferenceValue != null;
                    default:
                        Debug.LogError("Unsupported property type: " + prop.propertyType);
                        return false;
                }
            }
        }
    }
}
