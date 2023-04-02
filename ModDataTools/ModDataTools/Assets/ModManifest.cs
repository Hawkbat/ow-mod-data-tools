using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(ModManifestAsset))]
    public class ModManifestAsset : DataAsset, IValidateableAsset, IJsonSerializable
    {
        [Header("Export")]
        [Tooltip("Whether to export this mod's data")]
        public bool ExportMod;
        [Tooltip("The manifest.json file to use as-is instead of generating one")]
        [ConditionalField(nameof(ExportMod))]
        public TextAsset OverrideJsonFile;
        [Header("Data")]
        [Tooltip("The author of the mod")]
        public string Author;
        [Tooltip("The name of the dll file to load")]
        public string FileName;
        [Tooltip("The version of the mod (should follow semver)")]
        public string Version;
        [Tooltip("The version of the OWML that this mod was created with")]
        public string OWMLVersion;
        [Tooltip("The path to the patcher to use")]
        public string Patcher;
        [Tooltip("The dependencies of the mod")]
        public List<string> Dependencies = new();
        [Tooltip("Whether or not to load the mod before other mods")]
        public bool PriorityLoad;
        [Tooltip("The minimum version of the game that this mod is compatible with")]
        public string MinGameVersion;
        [Tooltip("The maximum version of the game that this mod is compatible with")]
        public string MaxGameVersion;
        [Tooltip("Whether this mod needs the very latest game version")]
        public bool RequireLatestVersion;
        [Tooltip("The vendors this mod does not work on")]
        public Vendors IncompatibleVendors;
        [Tooltip("The paths to preserve when updating the mod. Automatically includes config.json, manifest.json, and save.json")]
        public List<string> PathsToPreserve = new();
        [Tooltip("The mods that this mod conflicts with")]
        public List<string> Conflicts = new();
        [Tooltip("The warning to display when starting the mod for the first time")]
        public string Warning;
        [Tooltip("The New Horizons addon manifest for this mod")]
        public NewHorizonsAddonManifest NewHorizons;

        public override ModManifestAsset GetMod() => this;

        public override string GetIDPrefix() => $"{Author}.";

        public override string GetChildIDPrefix() => string.Empty;

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (string.IsNullOrEmpty(Author))
                validator.Error(this, $"Missing a value for {nameof(Author)}");
            if (string.IsNullOrEmpty(FileName))
                validator.Error(this, $"Missing a value for {nameof(FileName)}");
            if (string.IsNullOrEmpty(Version))
                validator.Error(this, $"Missing a value for {nameof(Version)}");
            else if (!Regex.IsMatch(Version, @"^\d+\.\d+\.\d+$"))
                validator.Error(this, $"{nameof(Version)} does not follow semver format (X.X.X)");
            if (string.IsNullOrEmpty(OWMLVersion))
                validator.Error(this, $"Missing a value for {nameof(OWMLVersion)}");
            else if (!Regex.IsMatch(OWMLVersion, @"^\d+\.\d+\.\d+$"))
                validator.Error(this, $"{nameof(OWMLVersion)} does not follow semver format (X.X.X)");
        }

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("filename", FileName);
            if (!string.IsNullOrEmpty(Patcher))
                writer.WriteProperty("patch", Patcher);
            writer.WriteProperty("author", Author);
            writer.WriteProperty("name", FullName);
            writer.WriteProperty("uniqueName", FullID);
            writer.WriteProperty("version", Version);
            writer.WriteProperty("owmlVersion", OWMLVersion);
            if (Dependencies.Any())
                writer.WriteProperty("dependencies", Dependencies);
            if (PriorityLoad)
                writer.WriteProperty("priorityLoad", PriorityLoad);
            if (!string.IsNullOrEmpty(MinGameVersion))
                writer.WriteProperty("minGameVersion", MinGameVersion);
            if (!string.IsNullOrEmpty(MaxGameVersion))
                writer.WriteProperty("maxGameVersion", MaxGameVersion);
            if (RequireLatestVersion)
                writer.WriteProperty("requireLatestVersion", RequireLatestVersion);
            if (PathsToPreserve.Any())
                writer.WriteProperty("pathsToPreserve", PathsToPreserve);
            if (Conflicts.Any())
                writer.WriteProperty("conflicts", Conflicts);
            if (!string.IsNullOrEmpty(Warning))
                writer.WriteProperty("warning", Warning);
            writer.WriteEndObject();
        }

        public override IEnumerable<AssetResource> GetResources()
        {
            if (ExportMod)
            {
                if (OverrideJsonFile)
                    yield return new TextResource(OverrideJsonFile, "manifest.json");
                else
                    yield return new TextResource(ExportUtility.ToJsonString(this), "manifest.json");
            }
            if (NewHorizons.ExportJsonFile)
            {
                if (NewHorizons.OverrideJsonFile)
                    yield return new TextResource(NewHorizons.OverrideJsonFile, "addon-manifest.json");
                else
                    yield return new TextResource(ExportUtility.ToJsonString(NewHorizons), "addon-manifest.json");
            }
            if (NewHorizons.Icon)
                yield return new ImageResource(NewHorizons.Icon, $"icons/{FullID}.png");
        }

        public enum Vendors
        {
            None = 0,
            Steam = 1 << 0,
            Epic = 1 << 1,
            Gamepass = 1 << 2,
        }

        [Serializable]
        public class NewHorizonsAddonManifest : IJsonSerializable
        {
            [Header("Export")]
            [Tooltip("Whether to export this mod's New Horizons addon-manifest.json file")]
            public bool ExportJsonFile = true;
            [Tooltip("The addon-manifest.json file to use as-is instead of generating one")]
            [ConditionalField(nameof(ExportJsonFile))]
            public TextAsset OverrideJsonFile;
            [Header("Data")]
            [Tooltip("The icon to display for this addon.")]
            public Texture2D Icon;
            [Tooltip("Credits info for this mod. A list of contributors and their roles.")]
            public List<CreditsRow> Credits = new();
            [Tooltip("A pop-up message for the first time a user runs the add-on.")]
            public string PopUpMessage;

            public void ToJson(JsonTextWriter writer)
            {
                writer.WriteStartObject();
                writer.WriteProperty("$schema", "https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/addon_manifest_schema.json");
                var achievements = AssetRepository.GetAllAssets<AchievementAsset>();
                if (achievements.Any())
                    writer.WriteProperty("achievements", achievements);
                if (Credits.Any())
                    writer.WriteProperty("credits", Credits.Select(c => $"{c.Name}#{c.Role}"));
                if (!string.IsNullOrEmpty(PopUpMessage))
                    writer.WriteProperty("popupMessage", PopUpMessage);

                writer.WriteEndObject();
            }

            [Serializable]
            public class CreditsRow
            {
                public string Name;
                public string Role;
            }
        }
    }
}
