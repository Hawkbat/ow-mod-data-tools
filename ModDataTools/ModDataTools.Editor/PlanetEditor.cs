﻿using ModDataTools.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModDataTools.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PlanetAsset), true)]
    public class PlanetEditor : DataAssetEditor
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
}