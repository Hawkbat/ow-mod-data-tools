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
    public class WarpTransmitterPropData : GeneralPropData
    {
        [Tooltip("The custom frequency of the warp transmitter.")]
        public FrequencyAsset Frequency;
        [Tooltip("The frequency of the warp transmitter, if not using a custom value.")]
        [ConditionalField(nameof(Frequency), (FrequencyAsset)null)]
        public NomaiWarpPlatform.Frequency WarpFrequency;
        [Tooltip("In degrees. Gives a margin of error for alignments.")]
        public float AlignmentWindow = 5f;
        [Tooltip("This makes the alignment happen if the destination planet is BELOW you rather than above.")]
        public bool FlipAlignment;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Frequency)
                writer.WriteProperty("frequency", Frequency.FullID);
            else
                writer.WriteProperty("frequency", WarpFrequency, false);
            if (AlignmentWindow != 5f)
                writer.WriteProperty("alignmentWindow", AlignmentWindow);
            if (FlipAlignment)
                writer.WriteProperty("flipAlignment", FlipAlignment);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(WarpTransmitterPropAsset))]
    public class WarpTransmitterPropAsset : GeneralPropAsset<WarpTransmitterPropData> { }
    public class WarpTransmitterPropComponent : GeneralPropComponent<WarpTransmitterPropData> { }
}
