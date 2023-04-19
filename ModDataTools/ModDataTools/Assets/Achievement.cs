using ModDataTools.Assets.Props;
using ModDataTools.Assets.Resources;
using ModDataTools.Assets.Volumes;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(AchievementAsset))]
    public class AchievementAsset : DataAsset, IValidateableAsset, IJsonSerializable
    {
        [Tooltip("The mod this asset belongs to")]
        public ModManifestAsset Mod;
        [Header("Data")]
        [Tooltip("The short description for this achievement.")]
        public string Description;
        [Tooltip("The icon to display for this achievement.")]
        public Texture2D Icon;
        [Tooltip("Should the name and description of the achievement be hidden until it is unlocked. Good for hiding spoilers!")]
        public bool Secret;
        [Tooltip("A list of facts that must be discovered before this achievement is unlocked.")]
        public List<FactAsset> Facts = new();
        [Tooltip("A list of signals that must be discovered before this achievement is unlocked.")]
        public List<SignalPropAsset> Signals = new();
        [Tooltip("A list of conditions that must be true before this achievement is unlocked. Conditions can be set via dialogue.")]
        public List<ConditionAsset> Conditions = new();

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Mod) yield return Mod;
        }

        public override string GetIDPrefix()
        {
            if (Mod) return Mod.FullID + "_";
            return base.GetIDPrefix();
        }

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("ID", FullID);
            writer.WriteProperty("secret", Secret);
            if (Facts.Any())
                writer.WriteProperty("factIDs", Facts.Select(f => f.FullID));
            if (Signals.Any())
                writer.WriteProperty("signalIDs", Signals.Select(s => s.FullID));
            if (Conditions.Any())
                writer.WriteProperty("conditionIDs", Conditions.Select(c => c.FullID));
            writer.WriteEndObject();
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!Icon)
                validator.Warn(this, $"Missing {nameof(Icon)}");
            var revealVolumes = AssetRepository.GetAllProps<RevealVolumeData>().Where(r => r.Data.Achievement == this);
            if (!Facts.Any() && !Signals.Any() && !Conditions.Any() && !revealVolumes.Any())
                validator.Warn(this, $"No unlock criteria defined");
        }

        public override void Localize(Localization l10n)
        {
            l10n.AddAchivement(FullID, FullName, Description);
        }

        public override IEnumerable<AssetResource> GetResources()
        {
            if (Icon)
                yield return new ImageResource(Icon, $"icons/{FullID}.png");
        }
    }
}
