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
    public class ProbeRulesetVolumeData : GeneralVolumeData
    {
        [Tooltip("Override the speed of the probe while in this ruleset volume.")]
        public NullishSingle ProbeSpeed;
        [Tooltip("Override the range of probe's light while in this ruleset volume.")]
        public NullishSingle LanternRange;
        [Tooltip("Stop the probe from attaching to anything while in this ruleset volume.")]
        public bool IgnoreAnchor;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (ProbeSpeed.HasValue)
            {
                writer.WriteProperty("overrideProbeSpeed", ProbeSpeed.HasValue);
                writer.WriteProperty("probeSpeed", ProbeSpeed.Value);
            }
            if (LanternRange.HasValue)
            {
                writer.WriteProperty("overrideLanternRange", LanternRange.HasValue);
                writer.WriteProperty("lanternRange", LanternRange.Value);
            }
            if (IgnoreAnchor)
                writer.WriteProperty("ignoreAnchor", IgnoreAnchor);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(ProbeRulesetVolumeAsset))]
    public class ProbeRulesetVolumeAsset : GeneralVolumeAsset<ProbeRulesetVolumeData> { }
    public class ProbeRulesetVolumeComponent : GeneralVolumeComponent<ProbeRulesetVolumeData> { }
}
