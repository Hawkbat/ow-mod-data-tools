using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;
using System.Linq;

namespace ModDataTools.Editors
{
    public class DialogueEditorWindow : EditorWindow
    {
        DialogueAsset dialogue;

        [MenuItem("Window/Dialogue Editor")]
        public static void Open() => Open(null);
        public static void Open(DialogueAsset dialogue)
        {
            var window = GetWindow<DialogueEditorWindow>();
            if (dialogue) window.dialogue = dialogue;
            window.Show();
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("Dialogue Editor");
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;

            if (!dialogue)
            {
                dialogue = Selection.activeObject as DialogueAsset;
                if (!dialogue)
                {
                    var node = Selection.activeObject as DialogueNodeAsset;
                    if (node)
                    {
                        dialogue = node.Dialogue;
                    }
                }
            }
            if (!dialogue)
            {
                GUILayout.Label($"Select a {nameof(DialogueAsset)} asset");
                return;
            }
            EditorGUILayout.ObjectField(dialogue, typeof(DialogueAsset), false);
            foreach (var node in dialogue.Nodes)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.ObjectField(node, typeof(DialogueNodeAsset), false);
                if (node.Pages.Any())
                {
                    var pageIndex = 0;
                    foreach (var page in node.Pages.ToList())
                    {
                        EditorGUILayout.BeginHorizontal();
                        var text = EditorGUILayout.TextField(page);
                        if (text != page)
                        {
                            Undo.RecordObject(node, "Change page text");
                            node.Pages[pageIndex] = text;
                        }
                        if (pageIndex > 0 && GUILayout.Button("↑", GUILayout.Width(25f)))
                        {
                            Undo.RecordObject(node, "Change page order");
                            node.Pages.RemoveAt(pageIndex);
                            node.Pages.Insert(pageIndex - 1, page);
                            pageIndex--;
                        }
                        if (pageIndex < node.Pages.Count - 1 && GUILayout.Button("↓", GUILayout.Width(25f)))
                        {
                            Undo.RecordObject(node, "Change page order");
                            node.Pages.RemoveAt(pageIndex);
                            node.Pages.Insert(pageIndex + 1, page);
                            pageIndex--;
                        }
                        if (GUILayout.Button("X", GUILayout.Width(25f)))
                        {
                            Undo.RecordObject(node, "Remove page");
                            node.Options.RemoveAt(pageIndex);
                        }
                        EditorGUILayout.EndHorizontal();
                        pageIndex++;
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+ Add Page", GUILayout.ExpandWidth(false)))
                {

                    Undo.RecordObject(node, "Add page");
                    node.Pages.Add("");
                }
                EditorGUILayout.EndHorizontal();
                if (!node.Options.Any())
                {
                    var nodeOptions = dialogue.Nodes.Select(n => n.FullName).ToList();
                    nodeOptions.Insert(0, "(Exit Dialogue)");
                    var currentIndex = node.Target ? dialogue.Nodes.IndexOf(node.Target) : -1;
                    var targetIndex = EditorGUILayout.Popup("Target", currentIndex + 1, nodeOptions.ToArray()) - 1;
                    if (targetIndex != currentIndex)
                    {
                        Undo.RecordObject(node, "Change target");
                        node.Target = targetIndex == -1 ? null : dialogue.Nodes[targetIndex];
                    }
                }
                if (!node.Target)
                {
                    var optionIndex = 0;
                    foreach (var option in node.Options.ToList())
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.BeginHorizontal();
                        string text = EditorGUILayout.TextField("Option", option.Text);
                        if (text != option.Text)
                        {
                            Undo.RecordObject(node, "Change option text");
                            option.Text = text;
                        }
                        if (optionIndex > 0 && GUILayout.Button("↑", GUILayout.Width(25f)))
                        {
                            Undo.RecordObject(node, "Change option order");
                            node.Options.RemoveAt(optionIndex);
                            node.Options.Insert(optionIndex - 1, option);
                            optionIndex--;
                        }
                        if (optionIndex < node.Options.Count - 1 && GUILayout.Button("↓", GUILayout.Width(25f)))
                        {
                            Undo.RecordObject(node, "Change option order");
                            node.Options.RemoveAt(optionIndex);
                            node.Options.Insert(optionIndex + 1, option);
                            optionIndex--;
                        }
                        if (GUILayout.Button("X", GUILayout.Width(25f)))
                        {
                            Undo.RecordObject(node, "Remove option");
                            node.Options.RemoveAt(optionIndex);
                        }
                        EditorGUILayout.EndHorizontal();
                        var nodeOptions = dialogue.Nodes.Select(n => n.FullName).ToList();
                        nodeOptions.Insert(0, "(Exit Dialogue)");
                        var currentIndex = option.Target ? dialogue.Nodes.IndexOf(option.Target) : -1;
                        var targetIndex = EditorGUILayout.Popup("Target", currentIndex + 1, nodeOptions.ToArray()) - 1;
                        if (targetIndex != currentIndex)
                        {
                            Undo.RecordObject(node, "Change option target");
                            option.Target = targetIndex == -1 ? null : dialogue.Nodes[targetIndex];
                        }
                        EditorGUILayout.EndVertical();
                        optionIndex++;
                    }
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("+ Add Option", GUILayout.ExpandWidth(false)))
                    {
                        Undo.RecordObject(node, "Add option");
                        node.Options.Add(new DialogueNodeAsset.Option());
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            if (GUILayout.Button("+ Add Node"))
            {
                var node = CreateInstance<DialogueNodeAsset>();

                node.Dialogue = dialogue;
                node.name = "New Node";
                dialogue.Nodes.Add(node);
                if (!dialogue.DefaultNode) dialogue.DefaultNode = node;
                AssetDatabase.AddObjectToAsset(node, dialogue);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
