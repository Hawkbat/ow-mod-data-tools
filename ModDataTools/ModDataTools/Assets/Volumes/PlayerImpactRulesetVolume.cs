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
    public class PlayerImpactRulesetVolumeData : GeneralVolumeData
    {
        [Tooltip("Minimum player impact speed. Player will take the minimum amount of damage if they impact something at this speed.")]
        public float MinImpactSpeed = 20f;
        [Tooltip("Maximum player impact speed. Players will die if they impact something at this speed.")]
        public float MaxImpactSpeed = 40f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (MinImpactSpeed != 20f)
                writer.WriteProperty("minImpactSpeed", MinImpactSpeed);
            if (MaxImpactSpeed != 40f)
                writer.WriteProperty("maxImpactSpeed", MaxImpactSpeed);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(PlayerImpactRulesetVolumeAsset))]
    public class PlayerImpactRulesetVolumeAsset : GeneralVolumeAsset<PlayerImpactRulesetVolumeData> { }
    public class PlayerImpactRulesetVolumeComponent : GeneralVolumeComponent<PlayerImpactRulesetVolumeData> { }
}
