using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class GeyserPropData : GeneralPropData
    {
        [Tooltip("Vertical offset of the geyser. From 0, the bubbles start at a height of 10, the shaft at 67, and the spout at 97.5.")]
        public float Offset = -97.5f;
        [Tooltip("Force of the geyser on objects")]
        public float Force = 55f;
        [Tooltip("Time in seconds eruptions last for")]
        public float ActiveDuration = 10f;
        [Tooltip("Time in seconds between eruptions")]
        public float InactiveDuration = 19f;
        [Tooltip("Color of the geyser. Alpha sets the particle density.")]
        public NullishColor Tint;
        [Tooltip("Disable the individual particle systems of the geyser")]
        public bool DisableBubbles;
        [Tooltip("Disable the individual particle systems of the geyser")]
        public bool DisableShaft;
        [Tooltip("Disable the individual particle systems of the geyser")]
        public bool DisableSpout;
        [Tooltip("Loudness of the geyser")]
        public float Volume = 0.7f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("offset", Offset);
            writer.WriteProperty("force", Force);
            writer.WriteProperty("activeDuration", ActiveDuration);
            writer.WriteProperty("inactiveDuration", InactiveDuration);
            writer.WriteProperty("tint", Tint);
            writer.WriteProperty("disableBubbles", DisableBubbles);
            writer.WriteProperty("disableShaft", DisableShaft);
            writer.WriteProperty("disableSpout", DisableSpout);
            writer.WriteProperty("volume", Volume);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(GeyserPropAsset))]
    public class GeyserPropAsset : GeneralPropAsset<GeyserPropData> { }
    public class GeyserPropComponent : GeneralPropComponent<GeyserPropData> { }
}
