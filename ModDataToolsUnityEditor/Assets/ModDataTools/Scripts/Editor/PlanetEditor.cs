using ModDataTools.Assets;
using ModDataTools.Editors;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(PlanetAsset), true)]
public class PlanetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!serializedObject.isEditingMultipleObjects)
        {
            if (target is PlanetAsset planet)
            {
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                if (GUILayout.Button("Open Ship Log Editor"))
                {
                    ShipLogEditorWindow.Open(planet.StarSystem);
                }
            }
        }
    }
}
