﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools;
using ModDataTools.Assets;

namespace ModDataTools.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EntryAsset), true)]
    public class EntryEditor : DataAssetEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!serializedObject.isEditingMultipleObjects)
            {
                if (target is EntryAsset entry)
                {
                    var exploreFacts = entry.ExploreFacts;
                    var rumorFacts = entry.RumorFacts;

                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);
                    EditorGUILayout.LabelField("Children", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Explore Facts");
                    EditorGUI.indentLevel++;
                    if (exploreFacts.Any())
                    {
                        foreach (var fact in exploreFacts)
                        {
                            EditorGUILayout.ObjectField(fact, typeof(ExploreFactAsset), false);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField(string.IsNullOrEmpty(fact.Text) ? "<empty>" : fact.Text, EditorStyles.wordWrappedLabel);
                            EditorGUI.indentLevel--;
                        }
                    }
                    if(GUILayout.Button("Add New Explore Fact"))
                    {
                        var fact = CreateInstance<ExploreFactAsset>();
                        fact.Entry = entry;
                        fact.name = "New Fact";
                        entry.ExploreFacts.Add(fact);
                        EditorUtility.SetDirty(fact);
                        EditorUtility.SetDirty(entry);
                        AssetDatabase.AddObjectToAsset(fact, entry);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Rumor Facts");
                    EditorGUI.indentLevel++;
                    if (rumorFacts.Any())
                    {
                        foreach (var fact in rumorFacts)
                        {
                            EditorGUILayout.ObjectField(fact, typeof(RumorFactAsset), false);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField(string.IsNullOrEmpty(fact.Text) ? "<empty>" : fact.Text, EditorStyles.wordWrappedLabel);
                            if (fact.Source == entry)
                                EditorGUILayout.ObjectField("Target", fact.Entry, typeof(EntryAsset), false);
                            if (fact.Entry == entry)
                                EditorGUILayout.ObjectField("Source", fact.Source, typeof(EntryAsset), false);
                            EditorGUI.indentLevel--;
                        }
                    }
                    if (GUILayout.Button("Add New Rumor Fact"))
                    {
                        var fact = CreateInstance<RumorFactAsset>();
                        fact.Entry = entry;
                        fact.name = "New Rumor";
                        entry.RumorFacts.Add(fact);
                        EditorUtility.SetDirty(fact);
                        EditorUtility.SetDirty(entry);
                        AssetDatabase.AddObjectToAsset(fact, entry);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                    GUILayout.Space(EditorGUIUtility.singleLineHeight);
                    if (GUILayout.Button("Open Ship Log Editor"))
                    {
                        if (entry && entry.Planet && entry.Planet.StarSystem)
                        {
                            ShipLogEditorWindow.Open(entry.Planet.StarSystem);
                        }
                    }
                }
            }
        }
    }
}
