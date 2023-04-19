using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Utilities;
using System;

namespace ModDataTools.Editors
{
    [CustomPropertyDrawer(typeof(EnumValuePickerAttribute))]
    public class EnumValuePickerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var textRect = new Rect(rect.x, rect.y, rect.width - 50f, rect.height);
            var searchRect = new Rect(rect.x + (rect.width - 50f), rect.y, 50f, rect.height);
            EditorGUI.BeginChangeCheck();
            string value = EditorGUI.TextField(textRect, label, property.enumNames[property.enumValueIndex]);
            if (EditorGUI.EndChangeCheck())
            {
                int index = Array.IndexOf(property.enumNames, value);
                if (index != -1)
                {
                    property.enumValueIndex = index;
                    property.serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                }
            }
            if (GUI.Button(searchRect, "Search"))
            {
                EnumValuePickerWindow.Show(property.enumDisplayNames, result =>
                {
                    if (result != -1)
                    {
                        property.enumValueIndex = result;
                        property.serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                    }
                });
            }
        }

        public class EnumValuePickerWindow : EditorWindow
        {
            string search;
            string[] names;
            Action<int> callback;

            Vector2 scrollPos;

            public static void Show(string[] names, Action<int> callback)
            {
                var window = CreateInstance<EnumValuePickerWindow>();
                window.titleContent = new GUIContent("Enum Value Picker");
                window.names = names;
                window.callback = callback;
                window.ShowUtility();
            }

            private void OnEnable()
            {
                Selection.selectionChanged += Close;
            }

            private void OnDisable()
            {
                Selection.selectionChanged -= Close;
                if (callback != null)
                {
                    callback(-1);
                    callback = null;
                }
            }

            private void OnGUI()
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                search = EditorGUILayout.TextField("Search", search);
                for (int i = 0; i < names.Length; i++)
                {
                    if (!string.IsNullOrEmpty(search) && !names[i].ToLower().Contains(search.ToLower())) continue;
                    if (GUILayout.Button(names[i]))
                    {
                        callback(i);
                        Close();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
