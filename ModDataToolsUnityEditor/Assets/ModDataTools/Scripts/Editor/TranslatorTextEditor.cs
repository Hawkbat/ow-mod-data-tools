using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;
using System.Linq;

namespace ModDataTools.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TranslatorText), true)]
    public class TranslatorTextEditor : DataAssetEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!serializedObject.isEditingMultipleObjects)
            {
                if (target is TranslatorText text)
                {
                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);
                    EditorGUILayout.LabelField("Children", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Text Blocks");
                    EditorGUI.indentLevel++;
                    if (text.TextBlocks.Any())
                    {
                        foreach (var block in text.TextBlocks)
                        {
                            EditorGUILayout.ObjectField(block, typeof(TranslatorTextBlock), false);
                        }
                    }
                    if (GUILayout.Button("Add New Text Block"))
                    {
                        var block = CreateInstance<TranslatorTextBlock>();

                        block.TranslatorText = text;
                        block.name = "New Text Block";
                        text.TextBlocks.Add(block);
                        AssetDatabase.AddObjectToAsset(block, text);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            if (GUILayout.Button("Open Translator Text Editor"))
            {
                TranslatorTextEditorWindow.Open(target as TranslatorText);
            }
        }
    }
}
