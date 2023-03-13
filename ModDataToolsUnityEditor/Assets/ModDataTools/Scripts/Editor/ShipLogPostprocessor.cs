using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ModDataTools.Editors
{
    public class ShipLogPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            ShipLogEditorWindow.currentAssetDatabaseTime = Time.realtimeSinceStartup;
        }
    }
}
