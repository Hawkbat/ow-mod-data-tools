﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;

namespace ModDataTools.Editors
{
    [CustomEditor(typeof(DataAsset), true)]
    public class DataAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!serializedObject.isEditingMultipleObjects)
            {
                if (target is DataAsset asset)
                {
                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);
                    EditorGUILayout.LabelField("Computed", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Full ID", asset.GetFullID());
                    EditorGUILayout.LabelField("Full Name", asset.GetFullName());
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("Mod", asset.GetMod(), typeof(ModManifest), false);
                    GUI.enabled = true;
                    EditorGUI.indentLevel--;

                    var adapter = new ModDataAdapter(false);
                    adapter.Validate(asset);
                    foreach (var error in adapter.GetErrors(asset))
                        EditorGUILayout.HelpBox(error, MessageType.Error);
                    foreach (var warning in adapter.GetWarnings(asset))
                        EditorGUILayout.HelpBox(warning, MessageType.Warning);
                }
            }
        }
    }
}