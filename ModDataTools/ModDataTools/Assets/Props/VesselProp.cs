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
    public class VesselPropData : GeneralSolarSystemPropData
    {
        [Tooltip("Whether the vessel should spawn in this system even if it wasn't used to warp to it. This will automatically power on the vessel.")]
        public bool AlwaysPresent;
        [Tooltip("Whether to always spawn the player on the vessel, even if it wasn't used to warp to the system.")]
        public bool SpawnOnVessel;
        [Tooltip("Whether the vessel should have physics enabled. Defaults to false if parentBody is set, and true otherwise.")]
        public bool HasPhysics;
        [Tooltip("Whether the vessel should have a zero-gravity volume around it. Defaults to false if parentBody is set, and true otherwise.")]
        public bool HasZeroGravityVolume;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {

        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(VesselPropAsset))]
    public class VesselPropAsset : GeneralSolarSystemPropAsset<VesselPropData> { }
    public class VesselPropComponent : GeneralSolarSystemPropComponent<VesselPropData> { }
}
