using ModDataTools.Assets.PlanetModules;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModDataTools.Editor
{
    [CustomPropertyDrawer(typeof(PlanetModule), true)]
    public class PlanetModuleDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var isEnabledProp = property.FindPropertyRelative("IsEnabled");
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded && isEnabledProp.boolValue);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var isEnabledProp = property.FindPropertyRelative("IsEnabled");

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var enableRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2f, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            if (isEnabledProp.boolValue)
            {
                property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);
                if (property.isExpanded)
                {
                    EditorGUI.PropertyField(position, property, GUIContent.none, true);
                }
            }
            else
            {
                EditorGUI.PrefixLabel(position, label);
            }
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(enableRect, isEnabledProp, GUIContent.none);
        }
    }
}