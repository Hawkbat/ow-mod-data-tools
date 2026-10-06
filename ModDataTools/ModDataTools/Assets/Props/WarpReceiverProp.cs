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
    public class WarpReceiverPropData : GeneralPropData
    {
        [Tooltip("The custom frequency of the warp receiver.")]
        public FrequencyAsset FrequencyAsset;
        [Tooltip("The frequency of the warp receiver, if not using a custom value.")]
        [ConditionalField(nameof(FrequencyAsset), (FrequencyAsset)null)]
        public NomaiWarpPlatform.Frequency Frequency;
        [Tooltip("The body the transmitter must be aligned with to warp to this receiver. Defaults to the body the receiver is on.")]
        public PlanetAsset AlignmentTargetBody;
        [Tooltip("Set to true if you want to include Nomai ruin details around the warp pad.")]
        public bool Detailed;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (FrequencyAsset)
                writer.WriteProperty("frequency", FrequencyAsset.FullID);
            else
                writer.WriteProperty("frequency", Frequency, false);
            if (AlignmentTargetBody)
                writer.WriteProperty("alignmentTargetBody", AlignmentTargetBody.FullID);
            if (Detailed)
                writer.WriteProperty("detailed", Detailed);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(WarpReceiverPropAsset))]
    public class WarpReceiverPropAsset : GeneralPropAsset<WarpReceiverPropData>
    {
        public NomaiComputerPropAsset Computer;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Computer)
                writer.WriteProperty("computer", context.MakeSibling(Computer));
        }
    }
    public class WarpReceiverPropComponent : GeneralPropComponent<WarpReceiverPropData>
    {
        public NomaiComputerPropAsset ComputerAsset;
        public NomaiComputerPropComponent Computer;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (ComputerAsset)
                writer.WriteProperty("computer", context.MakeSibling(ComputerAsset));
            else if (Computer)
                writer.WriteProperty("computer", context.MakeSibling(Computer));
        }
    }
}
