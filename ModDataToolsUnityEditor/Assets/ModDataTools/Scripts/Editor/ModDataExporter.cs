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
using ModDataTools.Assets.Resources;

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

            var manifests = LoadAllAssetsOfType<ModManifestAsset>();

            if (manifests.Count() != 1)
            {
                Log(LogLevel.Fatal, $"You must have exactly one Mod Manifest in your project with {nameof(ModManifestAsset.ExportMod)} checked to export your mod.");
                return;
            }
            foreach (var modManifest in manifests)
            {
                var adapter = new ModDataAdapter(true);
                AssetRepository.Initialize(adapter);

                if (!adapter.Validate(modManifest)) continue;

                modExportPath = settings.ModExportPath.Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                modExportPath = JoinPaths(modExportPath, modManifest.FullID);

                var localization = new Localization("english");

                foreach (var asset in AssetRepository.GetAllAssets<DataAsset>().Where(a => a.GetMod() == modManifest))
                {
                    if (!adapter.Validate(asset)) continue;

                    asset.Localize(localization);

                    foreach (var resource in asset.GetResources())
                    {
                        if (string.IsNullOrEmpty(resource.OutputPath) && string.IsNullOrEmpty(resource.AssetBundle))
                        {
                            Log(LogLevel.Error, $"Resource {resource.GetResource()} was expected to be in an asset bundle but is not assigned to one");
                            continue;
                        } else if (string.IsNullOrEmpty(resource.OutputPath))
                        {
                            continue;
                        }
                        if (resource is ImageResource img)
                            WriteImageToMod(img.Image, img.OutputPath);
                        else if (resource is AudioResource snd)
                            WriteSoundToMod(snd.Audio, snd.OutputPath);
                        else if (resource is TextResource txt)
                            WriteTextToMod(txt.Text.text, txt.OutputPath);
                        else if (resource is AssemblyResource asm)
                            CopyAssetToMod(asm.Assembly, asm.OutputPath);
                        else
                            Log(LogLevel.Error, $"Unspported resource type encountered (Resource {resource.GetResource()}) (Type {resource.GetType().Name})");

                    }
                }

                WriteTextToMod(ExportUtility.ToJsonString(localization), $"translations/{localization.LanguageName}.json");

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

        private static string WriteTextToMod(string text, string modAssetPath)
        {
            var modAbsolutePath = Path.Combine(modExportPath, modAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(modAbsolutePath));
            File.WriteAllText(modAbsolutePath, text, Encoding.UTF8);
            Log(LogLevel.Trace, $"Wrote {Path.GetExtension(modAssetPath).ToUpper()} to {modAbsolutePath}");
            return modAssetPath;
        }

        private static string WriteImageToMod(Texture2D tex, string modAssetPath)
        {
            if (Path.GetExtension(modAssetPath) == Path.GetExtension(GetAssetPath(tex)))
                return CopyAssetToMod(tex, modAssetPath);
            var modAbsolutePath = Path.Combine(modExportPath, modAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(modAbsolutePath));
            if (Path.GetExtension(modAssetPath).ToLower() == ".png")
                File.WriteAllBytes(modAbsolutePath, tex.EncodeToPNG());
            else if (Path.GetExtension(modAssetPath).ToLower() == ".jpg")
                File.WriteAllBytes(modAbsolutePath, tex.EncodeToJPG());
            Log(LogLevel.Trace, $"Wrote {Path.GetExtension(modAssetPath).ToUpper()} to {modAbsolutePath}");
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

        static void Log(LogLevel level, string msg)
        {
            if (settings.LogLevel >= level)
                Debug.Log(msg);
        }
    }
}
