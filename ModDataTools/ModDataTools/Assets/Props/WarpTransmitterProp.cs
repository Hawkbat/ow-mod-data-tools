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
        public FrequencyAsset FrequencyAsset;
        [Tooltip("The frequency of the warp transmitter, if not using a custom value.")]
        [ConditionalField(nameof(FrequencyAsset), (FrequencyAsset)null)]
        public NomaiWarpPlatform.Frequency Frequency;
        [Tooltip("In degrees. Gives a margin of error for alignments.")]
        public float AlignmentWindow = 5f;
        [Tooltip("This makes the alignment happen if the destination planet is BELOW you rather than above.")]
        public bool FlipAlignment;
        [Tooltip("Volumes to explicitly add the warped player/scout to when using this warp pad.")]
        public EntrywayVolumesConfig EntrywayVolumes;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (FrequencyAsset)
                writer.WriteProperty("frequency", FrequencyAsset.FullID);
            else
                writer.WriteProperty("frequency", Frequency, false);
            if (AlignmentWindow != 5f)
                writer.WriteProperty("alignmentWindow", AlignmentWindow);
            if (FlipAlignment)
                writer.WriteProperty("flipAlignment", FlipAlignment);
            EntrywayVolumes.WriteJsonProperty(context.Planet, writer);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            EntrywayVolumes.Validate(context, context.Planet, validator);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(WarpTransmitterPropAsset))]
    public class WarpTransmitterPropAsset : GeneralPropAsset<WarpTransmitterPropData> { }
    public class WarpTransmitterPropComponent : GeneralPropComponent<WarpTransmitterPropData> { }
}
