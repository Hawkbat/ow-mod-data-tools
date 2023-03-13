using ModDataTools.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ModDataTools.Editors
{
    public class ModDataToolsSettings : ScriptableObject
    {
        [Tooltip("The path that mods will be exported to. The mod's ID (UniqueName) is appended to this.")]
        public string ModExportPath = "%APPDATA%/OuterWildsModManager/OWML/Mods/";
        [Tooltip("The lowest level of logging to output when exporting.")]
        public LogLevel LogLevel = LogLevel.Info;

        public static ModDataToolsSettings GetOrCreateInstance()
        {
            var assetPath = "Assets/ModDataTools/Settings/Settings.asset";
            var settings = AssetDatabase.LoadAssetAtPath<ModDataToolsSettings>(assetPath);
            if (!settings)
            {
                settings = CreateInstance<ModDataToolsSettings>();
                Directory.CreateDirectory(assetPath);
                AssetDatabase.CreateAsset(settings, assetPath);
            }
            return settings;
        }
    }

}