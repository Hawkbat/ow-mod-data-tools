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
        public string GameOverText;
        [Tooltip("The type of death the player will have if they enter this volume.")]
        public DeathType DeathType = DeathType.Default;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Type != CreditsType.Fast)
                writer.WriteProperty("creditsType", Type);
            if (!string.IsNullOrEmpty(GameOverText))
                writer.WriteProperty("gameOverText", GameOverText);
            if (DeathType != DeathType.Default)
                writer.WriteProperty("deathType", DeathType);
        }

        public enum CreditsType
        {
            Fast = 0,
            Final = 1,
            Kazoo = 2,
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(CreditsVolumeAsset))]
    public class CreditsVolumeAsset : GeneralVolumeAsset<CreditsVolumeData> { }
    public class CreditsVolumeComponent : GeneralVolumeComponent<CreditsVolumeData> { }
}
