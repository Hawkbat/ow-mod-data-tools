using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;
using System.Linq;

namespace ModDataTools.Editors
{
    public class TranslatorTextEditorWindow : EditorWindow
    {
        TranslatorTextAsset translatorText;

        [MenuItem("Window/Translator Text Editor")]
        public static void Open() => Open(null);
        public static void Open(TranslatorTextAsset translatorText)
        {
            var window = GetWindow<TranslatorTextEditorWindow>();
            if (translatorText) window.translatorText = translatorText;
            window.Show();
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("Translator Text Editor");
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;

            if (!translatorText)
            {
                translatorText = Selection.activeObject as TranslatorTextAsset;
                if (!translatorText)
                {
                    var block = Selection.activeObject as TranslatorTextBlockAsset;
                    if (block)
                    {
                        translatorText = block.TranslatorText;
                    }
                }
            }
            if (!translatorText)
            {
                GUILayout.Label($"Select a {nameof(TranslatorTextAsset)} asset");
                return;
            }
            EditorGUILayout.ObjectField(translatorText, typeof(TranslatorTextAsset), false);
            var blockIndex = 0;
            foreach (var block in translatorText.TextBlocks.ToList())
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.ObjectField(block, typeof(TranslatorTextBlockAsset), false);
                EditorGUILayout.BeginHorizontal();
                var text = EditorGUILayout.TextField(block.Text);
                if (text != block.Text)
                {
                    Undo.RecordObject(block, "Change block text");
                    block.Text = text;
                }
                if (blockIndex > 0 && GUILayout.Button("↑", GUILayout.Width(25f)))
                {
                    Undo.RecordObject(block, "Change block order");
                    translatorText.TextBlocks.RemoveAt(blockIndex);
                    translatorText.TextBlocks.Insert(blockIndex - 1, block);
                    blockIndex--;
                }
                if (blockIndex < translatorText.TextBlocks.Count - 1 && GUILayout.Button("↓", GUILayout.Width(25f)))
                {
                    Undo.RecordObject(block, "Change block order");
                    translatorText.TextBlocks.RemoveAt(blockIndex);
                    translatorText.TextBlocks.Insert(blockIndex + 1, block);
                    blockIndex--;
                }
                EditorGUILayout.EndHorizontal();
                //if (translatorText.Type == TranslatorText.TextType.Wall || translatorText.Type == TranslatorText.TextType.Scroll)
                {
                    var blockOptions = translatorText.TextBlocks.Select(n => n.FullName).ToList();
                    blockOptions.Insert(0, "(None)");
                    var currentIndex = block.Parent ? translatorText.TextBlocks.IndexOf(block.Parent) : -1;
                    var targetIndex = EditorGUILayout.Popup("Parent", currentIndex + 1, blockOptions.ToArray()) - 1;
                    if (targetIndex != currentIndex)
                    {
                        Undo.RecordObject(block, "Change block parent");
                        block.Parent = targetIndex == -1 ? null : translatorText.TextBlocks[targetIndex];
                    }
                }
                EditorGUILayout.EndVertical();
                blockIndex++;
            }
            //var canAdd = (translatorText.Type != TranslatorText.TextType.Trailmarker && translatorText.Type != TranslatorText.TextType.Cairn && translatorText.Type != TranslatorText.TextType.CairnVariant) || translatorText.TextBlocks == null || translatorText.TextBlocks.Count < 1;
            var canAdd = true;
            if (canAdd && GUILayout.Button("+ Add Text Block"))
            {
                var block = CreateInstance<TranslatorTextBlockAsset>();
                block.TranslatorText = translatorText;
                block.name = "New Text Block";
                translatorText.TextBlocks.Add(block);
                AssetDatabase.AddObjectToAsset(block, translatorText);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
