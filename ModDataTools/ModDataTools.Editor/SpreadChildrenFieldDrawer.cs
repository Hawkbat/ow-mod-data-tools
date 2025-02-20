using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Utilities;

namespace ModDataTools.Editor
{
    public class SpreadChildrenFieldDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property.isExpanded = true;
            var totalHeight = EditorGUI.GetPropertyHeight(property, label, true);
            var selfHeight = EditorGUI.GetPropertyHeight(property, label, false);
            return totalHeight - selfHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.y += 1f;
            var depth = property.depth;
            var enumerator = property.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var child = enumerator.Current as SerializedProperty;
                if (child.depth > depth + 1) continue;
                if (child.depth <= depth) break;
                EditorGUI.PropertyField(position, child, true);
                position.y += EditorGUI.GetPropertyHeight(child, true) + 2f;
            }
        }
    }
}
