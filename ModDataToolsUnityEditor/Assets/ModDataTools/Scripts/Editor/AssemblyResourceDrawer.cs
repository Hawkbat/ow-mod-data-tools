using ModDataTools.Assets.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ModDataTools.Editors
{
    [CustomPropertyDrawer(typeof(AssemblyResource), true)]
    public class AssemblyResourceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var asmProp = property.FindPropertyRelative("Assembly");
            return EditorGUI.GetPropertyHeight(asmProp);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var options = AssetDatabase.FindAssets("t:DefaultAsset")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => Path.GetExtension(path).ToLower() == ".dll")
                .Select(path => (Path.GetFileName(path), AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset)))).ToList();
            options.Insert(0, ("None", null));
            var asmProp = property.FindPropertyRelative("Assembly");
            var selectedOptionIndex = options.FindIndex(o => o.Item2 == asmProp.objectReferenceValue);
            var optionLabels = options.Select(o => new GUIContent(o.Item1)).ToArray();
            EditorGUI.BeginChangeCheck();
            int index = EditorGUI.Popup(position, label, selectedOptionIndex, optionLabels);
            if (EditorGUI.EndChangeCheck())
            {
                asmProp.objectReferenceValue = options[index].Item2;
            }
        }
    }
}
