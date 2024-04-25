using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;

namespace ModDataTools.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FactAsset), true)]
    public class FactEditor : DataAssetEditor
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
                if (target is FactAsset fact)
                {
                    if (GUILayout.Button("Delete"))
                    {
                        if (target is RumorFactAsset rumor) fact.Entry.RumorFacts.Remove(rumor);
                        if (target is ExploreFactAsset exploreFact) fact.Entry.ExploreFacts.Remove(exploreFact);
                        EditorUtility.SetDirty(fact);
                        EditorUtility.SetDirty(fact.Entry);
                        fact.Entry = null;
                        AssetDatabase.RemoveObjectFromAsset(target);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    GUILayout.Space(EditorGUIUtility.singleLineHeight);
                    if (GUILayout.Button("Open Ship Log Editor"))
                    {
                        if (fact && fact.Entry && fact.Entry.Planet && fact.Entry.Planet.StarSystem)
                        {
                            ShipLogEditorWindow.Open(fact.Entry.Planet.StarSystem);
                        }
                    }
                }
            }
        }
    }
}
