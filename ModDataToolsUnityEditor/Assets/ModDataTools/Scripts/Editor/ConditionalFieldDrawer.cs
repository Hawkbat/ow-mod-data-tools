using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Utilities;
using System.Reflection;

namespace ModDataTools.Editors
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldDrawer : PropertyDrawer
    {
        PropertyDrawer baseDrawer;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (attribute is ConditionalFieldAttribute conditional && !CheckCondition(property, conditional)) return -EditorGUIUtility.standardVerticalSpacing;
            if (property.type.StartsWith("Nullish"))
            {
                if (baseDrawer == null || !(baseDrawer is NullishDrawer)) baseDrawer = new NullishDrawer();
                return baseDrawer.GetPropertyHeight(property, label);
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is ConditionalFieldAttribute conditional && !CheckCondition(property, conditional)) return;
            var enumValuePickerAttribute = fieldInfo.GetCustomAttribute<EnumValuePickerAttribute>();
            if (enumValuePickerAttribute != null)
            {
                if (baseDrawer == null || !(baseDrawer is EnumValuePickerDrawer)) baseDrawer = new EnumValuePickerDrawer();
                baseDrawer.OnGUI(position, property, label);
            } else if (property.type.StartsWith("Nullish"))
            {
                if (baseDrawer == null || !(baseDrawer is NullishDrawer)) baseDrawer = new NullishDrawer();
                baseDrawer.OnGUI(position, property, label);
            } else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        bool CheckCondition(SerializedProperty property, ConditionalFieldAttribute conditional)
        {
            var isTrue = IsConditionTrue(property, conditional);
            if (conditional.Invert) return !isTrue;
            return isTrue;
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
                    if (prop.type.StartsWith("Nullish"))
                    {
                        var hasValueProp = prop.FindPropertyRelative("hasValue");
                        if (value is bool b && hasValueProp.boolValue == b) return true;
                    }
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
                            Debug.LogError("Unsupported property type: " + prop.propertyType + " at " + prop.propertyPath);
                            return false;
                    }
                }
                return false;
            }
            else
            {
                if (prop.type.StartsWith("Nullish"))
                {
                    var hasValueProp = prop.FindPropertyRelative("hasValue");
                    return hasValueProp.boolValue;
                }
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
                        Debug.LogError("Unsupported property type: " + prop.propertyType + " at " + prop.propertyPath);
                        return false;
                }
            }
        }
    }
}
