using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;
using System.Linq;

namespace ModDataTools.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DialogueAsset), true)]
    public class DialogueEditor : DataAssetEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!serializedObject.isEditingMultipleObjects)
            {
                if (target is DialogueAsset dialogue)
                {
                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);
                    EditorGUILayout.LabelField("Children", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Dialogue Nodes");
                    EditorGUI.indentLevel++;
                    if (dialogue.Nodes.Any())
                    {
                        foreach (var node in dialogue.Nodes)
                        {
                            EditorGUILayout.ObjectField(node, typeof(Assets.DialogueNodeAsset), false);
                        }
                    }
                    if (GUILayout.Button("Add New Dialogue Node"))
                    {
                        var node = CreateInstance<Assets.DialogueNodeAsset>();

                        node.Dialogue = dialogue;
                        node.name = "New Node";
                        dialogue.Nodes.Add(node);
                        if (!dialogue.DefaultNode) dialogue.DefaultNode = node;
                        AssetDatabase.AddObjectToAsset(node, dialogue);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            if (GUILayout.Button("Open Dialogue Editor"))
            {
                DialogueEditorWindow.Open(target as DialogueAsset);
            }
        }
    }
}
