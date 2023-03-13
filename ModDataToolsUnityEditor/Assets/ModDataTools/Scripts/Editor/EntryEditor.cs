using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools;
using ModDataTools.Assets;

namespace ModDataTools.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EntryBase), true)]
    public class EntryEditor : DataAssetEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!serializedObject.isEditingMultipleObjects)
            {
                if (target is EntryBase entry)
                {
                    var exploreFacts = entry.ExploreFacts;
                    var rumorFacts = entry.RumorFacts;

                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);
                    EditorGUILayout.LabelField("Children", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Explore Facts");
                    EditorGUI.indentLevel++;
                    if (exploreFacts != null && exploreFacts.Any())
                    {
                        foreach (var fact in exploreFacts)
                        {
                            EditorGUILayout.ObjectField(fact, typeof(ExploreFact), false);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField(string.IsNullOrEmpty(fact.Text) ? "<empty>" : fact.Text, EditorStyles.wordWrappedLabel);
                            EditorGUI.indentLevel--;
                        }
                    }
                    if(GUILayout.Button("Add New Explore Fact"))
                    {
                        var fact = CreateInstance<ExploreFact>();
                        fact.Entry = entry;
                        fact.name = "New Fact";
                        entry.ExploreFacts.Add(fact);
                        AssetDatabase.AddObjectToAsset(fact, entry);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Rumor Facts");
                    EditorGUI.indentLevel++;
                    if (rumorFacts != null && rumorFacts.Any())
                    {
                        foreach (var fact in rumorFacts)
                        {
                            EditorGUILayout.ObjectField(fact, typeof(RumorFact), false);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField(string.IsNullOrEmpty(fact.Text) ? "<empty>" : fact.Text, EditorStyles.wordWrappedLabel);
                            if (fact.Source == entry)
                                EditorGUILayout.ObjectField("Target", fact.Entry, typeof(EntryBase), false);
                            if (fact.Entry == entry)
                                EditorGUILayout.ObjectField("Source", fact.Source, typeof(EntryBase), false);
                            EditorGUI.indentLevel--;
                        }
                    }
                    if (GUILayout.Button("Add New Rumor Fact"))
                    {
                        var fact = CreateInstance<RumorFact>();
                        fact.Entry = entry;
                        fact.name = "New Rumor";
                        entry.RumorFacts.Add(fact);
                        AssetDatabase.AddObjectToAsset(fact, entry);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            if (GUILayout.Button("Open Ship Log Editor"))
            {
                ShipLogEditorWindow.Open();
            }
        }
    }
}
