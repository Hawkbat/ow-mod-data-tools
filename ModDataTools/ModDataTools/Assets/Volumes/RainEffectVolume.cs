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
    public class RainEffectVolumeData : GeneralPriorityVolumeData
    {
        [Tooltip("The rate at which the rain droplet effect will happen")]
        public float DropletRate = 0.1f;
        [Tooltip("The rate at which the rain streak effect will happen")]
        public float StreakRate = 1f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (DropletRate != 0.1f)
                writer.WriteProperty("dropletRate", DropletRate);
            if (StreakRate != 1f)
                writer.WriteProperty("streakRate", StreakRate);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(RainEffectVolumeAsset))]
    public class RainEffectVolumeAsset : GeneralPriorityVolumeAsset<RainEffectVolumeData> { }
    public class RainEffectVolumeComponent : GeneralPriorityVolumeComponent<RainEffectVolumeData> { }
}
