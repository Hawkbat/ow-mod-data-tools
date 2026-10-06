using ModDataTools.Assets.Props;
using ModDataTools.Assets.Resources;
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
        [Tooltip("The game over message and credits to show when entering this volume.")]
        public GameOverConfig GameOver;
        [Tooltip("The type of death the player will have if they enter this volume.")]
        public DeathType DeathType = DeathType.Default;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (DeathType != DeathType.Default)
                writer.WriteProperty("deathType", DeathType);
            writer.WritePropertyName("gameOver");
            GameOver.ToJson(writer, context.GetProp().PropID, GameOver.Audio ? context.Planet.GetResourcePath(GameOver.Audio) : null);
        }

        public override void Localize(PropContext context, Localization l10n)
        {
            GameOver.Localize(context.GetProp().PropID, l10n);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            GameOver.Validate(context.Planet, validator);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            if (GameOver.Audio)
                foreach (var resource in GameOver.GetResources(context.Planet.GetResourcePath(GameOver.Audio)))
                    yield return resource;
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(CreditsVolumeAsset))]
    public class CreditsVolumeAsset : GeneralVolumeAsset<CreditsVolumeData> { }
    public class CreditsVolumeComponent : GeneralVolumeComponent<CreditsVolumeData> { }
}
