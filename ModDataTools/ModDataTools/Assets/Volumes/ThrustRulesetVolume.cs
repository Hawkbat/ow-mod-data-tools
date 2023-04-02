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
    public class ThrustRulesetVolumeData : GeneralVolumeData
    {
        [Tooltip("Limit how fast you can fly with your ship while in this ruleset volume.")]
        public NullishSingle ThrustLimit;
        [Tooltip("How long the jetpack booster will be nerfed.")]
        public NullishSingle JetpackBoosterNerfDuration;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("thrustLimit", ThrustLimit);
            if (JetpackBoosterNerfDuration.HasValue)
            {
                writer.WriteProperty("nerfJetpackBooster", JetpackBoosterNerfDuration.HasValue);
                writer.WriteProperty("nerfDuration", JetpackBoosterNerfDuration.Value);
            }
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(ThrustRulesetVolumeAsset))]
    public class ThrustRulesetVolumeAsset : GeneralVolumeAsset<ThrustRulesetVolumeData> { }
    public class ThrustRulesetVolumeComponent : GeneralVolumeComponent<ThrustRulesetVolumeData> { }
}
