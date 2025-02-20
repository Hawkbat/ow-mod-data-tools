using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;
using ModDataTools.Utilities;
using ModDataTools.Assets.Props;

namespace ModDataTools.Editor
{
    public class ModDataAdapter : IAssetRepositoryStore, IAssetValidator
    {
        public bool OutputToConsole;

        Dictionary<DataAsset, List<string>> errors = new Dictionary<DataAsset, List<string>>();
        Dictionary<DataAsset, List<string>> warnings = new Dictionary<DataAsset, List<string>>();

        public ModDataAdapter(bool outputToConsole)
        {
            OutputToConsole = outputToConsole;
        }

        [MenuItem("Export/Reload Assets")]
        public static void ReloadAssets()
        {
            AssetRepository.Initialize(new ModDataAdapter(false));
        }

        public void Error(DataAsset asset, string message)
        {
            if (!errors.ContainsKey(asset)) errors[asset] = new List<string>();
            errors[asset].Add(message);
            if (OutputToConsole && ModDataToolsSettings.GetOrCreateInstance().LogLevel >= LogLevel.Error)
                Debug.LogError(message, asset);
        }

        public void Warn(DataAsset asset, string message)
        {
            if (!warnings.ContainsKey(asset)) warnings[asset] = new List<string>();
            warnings[asset].Add(message);
            if (OutputToConsole && ModDataToolsSettings.GetOrCreateInstance().LogLevel >= LogLevel.Warn)
                Debug.LogWarning(message, asset);
        }

        public IEnumerable<string> GetErrors(DataAsset asset)
        {
            if (errors.ContainsKey(asset)) return errors[asset];
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetWarnings(DataAsset asset)
        {
            if (warnings.ContainsKey(asset)) return warnings[asset];
            return Enumerable.Empty<string>();
        }

        public bool Validate(DataAsset asset)
        {
            asset.Validate(this);
            if (errors.ContainsKey(asset) && errors[asset].Any()) return false;
            return true;
        }

        public string GetAssetBundle(UnityEngine.Object obj)
        {
            var path = GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) return null;
            return AssetDatabase.GetImplicitAssetBundleName(path);
        }

        public string GetAssetPath(UnityEngine.Object obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }

        public IEnumerable<T> LoadAllAssets<T>() where T : DataAsset
        {
            return AssetDatabase.FindAssets("t:" + nameof(DataAsset))
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Distinct()
                .Select(path => AssetDatabase.LoadAssetAtPath<T>(path))
                .Where(v => !!v)
                .ToList();
        }
    }
}
