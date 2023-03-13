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
        TranslatorText translatorText;

        [MenuItem("Window/Translator Text Editor")]
        public static void Open() => Open(null);
        public static void Open(TranslatorText translatorText)
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
                translatorText = Selection.activeObject as TranslatorText;
                if (!translatorText)
                {
                    var block = Selection.activeObject as TranslatorTextBlock;
                    if (block)
                    {
                        translatorText = block.TranslatorText;
                    }
                }
            }
            if (!translatorText)
            {
                GUILayout.Label($"Select a {nameof(TranslatorText)} asset");
                return;
            }
            EditorGUILayout.ObjectField(translatorText, typeof(TranslatorText), false);
            var blockIndex = 0;
            foreach (var block in translatorText.TextBlocks.ToList())
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.ObjectField(block, typeof(TranslatorTextBlock), false);
                EditorGUILayout.BeginHorizontal();
                var text = EditorGUILayout.DelayedTextField(block.Text);
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
                if (translatorText.EditType == TranslatorText.TextEditType.Branching)
                {
                    var blockOptions = translatorText.TextBlocks.Select(n => n.GetFullName()).ToList();
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
            var canAdd = translatorText.EditType != TranslatorText.TextEditType.Single || translatorText.TextBlocks == null || translatorText.TextBlocks.Count < 1;
            if (canAdd && GUILayout.Button("+ Add Text Block"))
            {
                var block = CreateInstance<TranslatorTextBlock>();
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
