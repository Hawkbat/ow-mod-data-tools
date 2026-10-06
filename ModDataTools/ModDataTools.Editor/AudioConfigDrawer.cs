using ModDataTools.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModDataTools.Editor
{
    [CustomPropertyDrawer(typeof(AudioConfig), true)]
    public class AudioConfigDrawer : PropertyDrawer
    {
        readonly EnumValuePickerDrawer typePickerDrawer = new EnumValuePickerDrawer();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var clipProp = property.FindPropertyRelative(nameof(AudioConfig.Clip));
            var typeProp = property.FindPropertyRelative(nameof(AudioConfig.Type));

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            if (clipProp.objectReferenceValue || clipProp.hasMultipleDifferentValues)
            {
                EditorGUI.PropertyField(position, clipProp, GUIContent.none);
            }
            else
            {
                var clipRect = new Rect(position.x, position.y, position.width * 0.4f - 2f, position.height);
                var typeRect = new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.6f, position.height);
                EditorGUI.PropertyField(clipRect, clipProp, GUIContent.none);
                typePickerDrawer.OnGUI(typeRect, typeProp, GUIContent.none);
            }
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
