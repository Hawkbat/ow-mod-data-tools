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
    public class VesselWarpExitPropData : GeneralSolarSystemPropData
    {
        [Tooltip("If set, keeps the warp exit attached to the vessel.")]
        public bool AttachToVessel;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (AttachToVessel)
                writer.WriteProperty("attachToVessel", AttachToVessel);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(VesselWarpExitPropAsset))]
    public class VesselWarpExitPropAsset : GeneralSolarSystemPropAsset<VesselWarpExitPropData> { }
    public class VesselWarpExitPropComponent : GeneralSolarSystemPropComponent<VesselWarpExitPropData> { }
}
