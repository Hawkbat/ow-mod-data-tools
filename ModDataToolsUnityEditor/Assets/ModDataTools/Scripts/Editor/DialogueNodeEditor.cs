using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;

namespace ModDataTools.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DialogueNodeAsset), true)]
    public class DialogueNodeEditor : DataAssetEditor
    {

        public override void OnInspectorGUI()
        {
            var nameProp = serializedObject.FindProperty("m_Name");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(nameProp);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            base.OnInspectorGUI();
            if (!serializedObject.isEditingMultipleObjects)
            {
                if (target is DialogueNodeAsset node)
                {
                    if (GUILayout.Button("Delete"))
                    {
                        if (node.Dialogue.DefaultNode == node)
                            node.Dialogue.DefaultNode = null;
                        node.Dialogue.Nodes.Remove(node);
                        EditorUtility.SetDirty(node.Dialogue);
                        node.Dialogue = null;
                        AssetDatabase.RemoveObjectFromAsset(target);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            if (GUILayout.Button("Open Dialogue Editor"))
            {
                if (!serializedObject.isEditingMultipleObjects && target is DialogueNodeAsset node)
                    DialogueEditorWindow.Open(node.Dialogue);
                else
                    DialogueEditorWindow.Open(null);
            }
        }
    }
}
