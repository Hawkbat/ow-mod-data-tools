using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;

namespace ModDataTools.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TranslatorTextBlockAsset), true)]
    public class TranslatorTextBlockEditor : DataAssetEditor
    {

        public override void OnInspectorGUI()
        {
            var nameProp = serializedObject.FindProperty("m_Name");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.DelayedTextField(nameProp);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            base.OnInspectorGUI();
            if (!serializedObject.isEditingMultipleObjects)
            {
                if (target is TranslatorTextBlockAsset block)
                {
                    if (GUILayout.Button("Delete"))
                    {
                        block.TranslatorText.TextBlocks.Remove(block);
                        block.TranslatorText = null;
                        AssetDatabase.RemoveObjectFromAsset(target);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            if (GUILayout.Button("Open Translator Text Editor"))
            {
                if (!serializedObject.isEditingMultipleObjects && target is TranslatorTextBlockAsset block)
                    TranslatorTextEditorWindow.Open(block.TranslatorText);
                else
                    TranslatorTextEditorWindow.Open(null);
            }
        }
    }
}
