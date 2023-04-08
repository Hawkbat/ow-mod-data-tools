using ModDataTools.Assets;
using ModDataTools.Editors;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(StarSystemAsset), true)]
public class StarSystemEditor : DataAssetEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!serializedObject.isEditingMultipleObjects)
        {
            if (target is StarSystemAsset starSystem)
            {
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                if (GUILayout.Button("Open Ship Log Editor"))
                {
                    ShipLogEditorWindow.Open(starSystem);
                }
            }
        }
    }
}
