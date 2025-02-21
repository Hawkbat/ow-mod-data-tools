using ModDataTools.Assets.Props;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Volumes
{
    [Serializable]
    public class CreditsVolumeData : GeneralVolumeData
    {
        [Tooltip("The type of credits to play")]
        public CreditsType Type;
        [Tooltip("Text displayed in orange on game over. For localization, put translations under UI.")]
        public string Text;
        [Tooltip("Change the colour of the game over text. Leave empty to use the default orange.")]
        public NullishColor Colour;
        [Tooltip("Condition that must be true for this game over to trigger. If this is on a LoadCreditsVolume, leave empty to always trigger this game over. Note this is a regular dialogue condition, not a persistent condition.")]
        public ConditionAsset Condition;
        [Tooltip("The type of death the player will have if they enter this volume.")]
        public DeathType DeathType = DeathType.Default;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (DeathType != DeathType.Default)
                writer.WriteProperty("deathType", DeathType);
            writer.WritePropertyName("gameOver");
            writer.WriteStartObject();
            writer.WriteProperty("text", Text);
            writer.WriteProperty("colour", Colour);
            if (Condition)
                writer.WriteProperty("condition", Condition.FullID);
            if (Type != CreditsType.Fast)
                writer.WriteProperty("creditsType", Type);
            writer.WriteEndObject();
        }

        public override void Localize(PropContext context, Localization l10n)
        {
            l10n.AddUI(context.GetProp().PropID, Text);
        }

        public override void Validate(PropContext context, DataAsset asset, IAssetValidator validator)
        {
            if (Condition && Condition.Persistent)
                validator.Error(asset, $"Credits volume condition '{Condition.FullID}' must not be persistent.");
        }

        public enum CreditsType
        {
            Fast = 0,
            Final = 1,
            Kazoo = 2,
            None = 3,
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(CreditsVolumeAsset))]
    public class CreditsVolumeAsset : GeneralVolumeAsset<CreditsVolumeData> { }
    public class CreditsVolumeComponent : GeneralVolumeComponent<CreditsVolumeData> { }
}
