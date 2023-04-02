using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    public abstract class DataAsset : ScriptableObject, IValidateableAsset
    {
        public const string ASSET_MENU_PREFIX = "Mod Data Assets/";
        public const string PROP_MENU_PREFIX = "Mod Data Props/";
        public const string VOLUME_MENU_PREFIX = "Mod Data Volumes/";

        public string Name { get => name; set => name = value; }
        [Header("Overrides")]
        [Tooltip("If set, uses this name instead of the filename")]
        public string OverrideFullName;
        [Tooltip("If set, uses this name instead of generating an ID")]
        public string OverrideFullID;
        [Header("Data Asset")]
        [Tooltip("The ID of this asset, which will be concatenated with any applicable parent prefixes")]
        public string ID;

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(OverrideFullName))
                    return OverrideFullName;
                return Name;
            }
        }

        public string FullID
        {
            get
            {
                if (!string.IsNullOrEmpty(OverrideFullID))
                    return OverrideFullID;
                if (!string.IsNullOrEmpty(ID))
                    return GetIDPrefix() + ID;
                return GetIDPrefix() + FullName.ToUpper().Replace(' ', '_');
            }
        }

        public virtual ModManifestAsset GetMod()
        {
            foreach (var parent in GetParentAssets())
            {
                if (parent && parent.GetMod()) return parent.GetMod();
            }
            return null;
        }

        public virtual IEnumerable<DataAsset> GetParentAssets()
            => Enumerable.Empty<DataAsset>();

        public virtual IEnumerable<DataAsset> GetNestedAssets()
            => Enumerable.Empty<DataAsset>();

        public virtual IEnumerable<AssetResource> GetResources()
            => Enumerable.Empty<AssetResource>();

        public virtual string GetIDPrefix()
        {
            string prefix = "";
            foreach (var parentAsset in GetParentAssets())
            {
                prefix += parentAsset.GetChildIDPrefix();
            }
            return prefix;
        }

        public virtual string GetChildIDPrefix() => GetIDPrefix();

        public virtual void Validate(IAssetValidator validator)
        {
            if (string.IsNullOrEmpty(FullID) || FullID.EndsWith("_") || FullID.EndsWith("."))
                validator.Error(this, "Invalid ID");
            if (string.IsNullOrEmpty(FullName))
                validator.Error(this, "Invalid Name");
            if (!GetMod())
                validator.Error(this, "Not attached to any mod");
        }
    }
}
