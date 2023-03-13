using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using ModDataTools.Assets;
using ModDataTools.Utilities;
using Codice.Utils;
using UnityEditor.VersionControl;

namespace ModDataTools.Editors
{
    public static class ModDataExporter
    {
        static ModDataToolsSettings settings;
        static string modExportPath;

        [MenuItem("Export/Mod Data")]
        public static void ExportAll()
        {
            settings = ModDataToolsSettings.GetOrCreateInstance();
            
            Log(LogLevel.Info, "Exporting mod data...");

            var manifests = LoadAllAssetsOfType<ModManifest>();

            if (manifests.Count() != 1)
            {
                Log(LogLevel.Fatal, $"You must have exactly one Mod Manifest in your project with {nameof(ModManifest)} checked to export your mod.");
                return;
            }
            foreach (var modManifest in manifests)
            {
                var adapter = new ModDataAdapter(true);
                AssetRepository.Initialize(adapter);

                if (!adapter.Validate(modManifest)) continue;

                modExportPath = settings.ModExportPath.Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                modExportPath = JoinPaths(modExportPath, modManifest.GetFullID());

                WriteJsonAssetToMod(modManifest, modManifest.OverrideJsonFile, "manifest.json");
                WriteJsonAssetToMod(modManifest.AddonManifest, modManifest.AddonManifest.OverrideJsonFile, "addon-manifest.json");

                if (modManifest.AddonManifest.Icon)
                    WriteImageToMod(modManifest.AddonManifest.Icon, JoinPaths("icons/", modManifest.GetFullName() + ".png"));

                var achievements = adapter.LoadAssets<Achievement>().Where(a => a.GetMod() == modManifest);
                foreach (var achievement in achievements)
                {
                    if (achievement.Icon)
                        WriteImageToMod(achievement.Icon, JoinPaths("icons/", achievement.GetFullID() + ".png"));
                }

                var starSystems = adapter.LoadAssets<StarSystem>().Where(a => a.GetMod() == modManifest);
                foreach (var starSystem in starSystems)
                {
                    if (!adapter.Validate(starSystem)) continue;
                    if (starSystem.ExportConfigFile)
                        WriteJsonAssetToMod(starSystem, starSystem.OverrideConfigFile, JoinPaths("systems/", starSystem.GetFullName() + ".json"));
                    if (starSystem.NewHorizons.TravelAudio)
                        WriteSoundToMod(starSystem.NewHorizons.TravelAudio, JoinPaths("systems/", starSystem.GetFullName(), "travel.wav"));
                    if (starSystem.NewHorizons.Skybox.HasCustomSkybox && starSystem.NewHorizons.Skybox.IsCustomSkyboxValid())
                    {
                        WriteImageToMod(starSystem.NewHorizons.Skybox.Left, JoinPaths("systems/", starSystem.GetFullName(), "skybox/", "left.png"));
                        WriteImageToMod(starSystem.NewHorizons.Skybox.Right, JoinPaths("systems/", starSystem.GetFullName(), "skybox/", "right.png"));
                        WriteImageToMod(starSystem.NewHorizons.Skybox.Top, JoinPaths("systems/", starSystem.GetFullName(), "skybox/", "top.png"));
                        WriteImageToMod(starSystem.NewHorizons.Skybox.Bottom, JoinPaths("systems/", starSystem.GetFullName(), "skybox/", "bottom.png"));
                        WriteImageToMod(starSystem.NewHorizons.Skybox.Front, JoinPaths("systems/", starSystem.GetFullName(), "skybox/", "front.png"));
                        WriteImageToMod(starSystem.NewHorizons.Skybox.Back, JoinPaths("systems/", starSystem.GetFullName(), "skybox/", "back.png"));
                    }
                }

                var planets = adapter.LoadAssets<Planet>().Where(a => a.GetMod() == modManifest);
                foreach (var planet in planets)
                {
                    if (!adapter.Validate(planet)) continue;
                    if (planet.ExportShipLogFile)
                        WriteXmlAssetToMod(planet, planet.OverrideShipLogFile, JoinPaths("shiplogs/", planet.SolarSystem.GetFullName(), planet.GetFullName() + ".xml"));
                    if (planet.ExportConfigFile)
                        WriteJsonAssetToMod(planet, planet.OverrideConfigFile, JoinPaths("planets/", planet.SolarSystem.GetFullName(), planet.GetFullName() + ".json"));
                    var entries = adapter.LoadAssets<EntryBase>().Where(e => e.Planet == planet);
                    foreach (var entry in entries)
                    {
                        if (entry.Photo)
                            WriteImageToMod(entry.Photo, JoinPaths("shiplogs/", planet.SolarSystem.GetFullName(), planet.GetFullName(), "sprites/", entry.GetFullID() + ".png"));
                        if (entry.AltPhoto)
                            WriteImageToMod(entry.Photo, JoinPaths("shiplogs/", planet.SolarSystem.GetFullName(), planet.GetFullName(), "sprites/", entry.GetFullID() + "_ALT.png"));
                    }
                }

                var dialogues = adapter.LoadAssets<Dialogue>().Where(a => a.GetMod() == modManifest);
                foreach (var dialogue in dialogues)
                {
                    if (!adapter.Validate(dialogue)) continue;
                    if (!dialogue.ExportXmlFile) continue;
                    WriteXmlAssetToMod(dialogue, dialogue.OverrideXmlFile, JoinPaths("dialogue/", dialogue.Planet.GetFullName(), dialogue.Planet.GetFullName(), dialogue.GetFullName() + ".xml"));
                }

                var translatorTexts = adapter.LoadAssets<TranslatorText>().Where(a => a.GetMod() == modManifest);
                foreach (var translatorText in translatorTexts)
                {
                    if (!adapter.Validate(translatorText)) continue;
                    if (!translatorText.ExportXmlFile) continue;
                    WriteXmlAssetToMod(translatorText, translatorText.OverrideXmlFile, JoinPaths("text/", translatorText.Planet.GetFullName(), translatorText.GetFullName() + ".xml"));
                }
                Log(LogLevel.Info, "Mod data exported.");

                BuildAllAssetBundles();
            }

            Log(LogLevel.Info, "Export complete.");
        }

        public static void BuildAllAssetBundles()
        {
            var assetBundleDirectory = JoinPaths(modExportPath, "assetbundles");
            if (!Directory.Exists(assetBundleDirectory))
                Directory.CreateDirectory(assetBundleDirectory);
            Log(LogLevel.Info, "Building asset bundles...");
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            Log(LogLevel.Info, "Asset bundles built.");
        }

        private static string WriteJsonAssetToMod(IJsonAsset asset, TextAsset overrideFile, string modAssetPath)
        {
            return WriteJsonTextToMod(overrideFile ? overrideFile.text : asset.ToJsonString(), modAssetPath);
        }

        private static string WriteJsonTextToMod(string json, string modAssetPath)
        {
            var modAbsolutePath = Path.Combine(modExportPath, modAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(modAbsolutePath));
            File.WriteAllText(modAbsolutePath, json, Encoding.UTF8);
            Log(LogLevel.Trace, $"Wrote JSON to {modAbsolutePath}");
            return modAssetPath;
        }

        private static string WriteXmlAssetToMod(IXmlAsset asset, TextAsset overrideFile, string modAssetPath)
        {
            return WriteXmlTextToMod(overrideFile ? overrideFile.text : asset.ToXmlString(), modAssetPath);
        }

        private static string WriteXmlTextToMod(string xml, string modAssetPath)
        {
            var modAbsolutePath = Path.Combine(modExportPath, modAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(modAbsolutePath));
            File.WriteAllText(modAbsolutePath, xml, Encoding.UTF8);
            Log(LogLevel.Trace, $"Wrote XML to {modAbsolutePath}");
            return modAssetPath;
        }

        private static string WriteImageToMod(Texture2D tex, string modAssetPath)
        {
            if (Path.GetExtension(modAssetPath) == Path.GetExtension(GetAssetPath(tex)))
                return CopyAssetToMod(tex, modAssetPath);
            var modAbsolutePath = Path.Combine(modExportPath, modAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(modAbsolutePath));
            File.WriteAllBytes(modAbsolutePath, tex.EncodeToPNG());
            Log(LogLevel.Trace, $"Wrote PNG to {modAbsolutePath}");
            return modAssetPath;
        }

        private static string WriteSoundToMod(AudioClip clip, string modAssetPath)
        {
            return CopyAssetToMod(clip, modAssetPath);
        }

        private static string CopyAssetToMod(UnityEngine.Object asset, string modAssetPath)
        {
            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), GetAssetPath(asset));
            if (Path.GetExtension(sourcePath) != Path.GetExtension(modAssetPath))
                throw new Exception($"Asset must be a .{Path.GetExtension(modAssetPath)} file: {GetAssetPath(asset)}");
            var modAbsolutePath = Path.Combine(modExportPath, modAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(modAbsolutePath));
            File.Copy(sourcePath, modAbsolutePath, true);
            Log(LogLevel.Trace, $"Wrote {Path.GetExtension(modAssetPath).ToUpper()} to {modAbsolutePath}");
            return modAssetPath;
        }

        private static List<T> LoadAllAssetsOfType<T>() where T : UnityEngine.Object
        {
            return AssetDatabase.FindAssets("t:" + typeof(T).Name)
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<T>(path))
                .ToList();
        }

        private static string GetAssetPath(UnityEngine.Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path)) throw new NullReferenceException("Asset does not exist: " + asset);
            return path;
        }

        private static string GetAssetBundle(UnityEngine.Object asset)
        {
            var path = GetAssetPath(asset);
            var bundle = AssetDatabase.GetImplicitAssetBundleName(path);
            if (string.IsNullOrEmpty(bundle)) throw new NullReferenceException("Asset does not belong to a bundle: " + path);
            return bundle;
        }

        private static string JoinPaths(params string[] paths)
        {
            return string.Join("" + Path.DirectorySeparatorChar, paths.Select(p =>
            {
                if (p.StartsWith("/")) p = p.Substring(1);
                if (p.StartsWith("\\")) p = p.Substring(1);
                if (p.EndsWith("/")) p = p.Substring(0, p.Length - 1);
                if (p.EndsWith("\\")) p = p.Substring(0, p.Length - 1);
                if (p.Contains("/")) p = p.Replace('/', Path.DirectorySeparatorChar);
                if (p.Contains("\\")) p = p.Replace('\\', Path.DirectorySeparatorChar);
                return p;
            }));
        }

        private static T[] NullIfEmpty<T>(this T[] arr)
        {
            if (arr != null && arr.Length == 0) return null;
            return arr;
        }

        static void Log(LogLevel level, string msg)
        {
            if (settings.LogLevel >= level)
                Debug.Log(msg);
        }
    }
}
