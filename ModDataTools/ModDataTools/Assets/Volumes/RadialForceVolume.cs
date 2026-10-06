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
    public class RadialForceVolumeData : GeneralForceVolumeData
    {
        [Tooltip("How the force falls off with distance.")]
        public FallOffType FallOff = FallOffType.Linear;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (FallOff != FallOffType.Linear)
                writer.WriteProperty("fallOff", FallOff);
        }

        public enum FallOffType
        {
            Constant = 0,
            Linear = 1,
            InverseSquared = 2,
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(RadialForceVolumeAsset))]
    public class RadialForceVolumeAsset : GeneralPriorityVolumeAsset<RadialForceVolumeData> { }
    public class RadialForceVolumeComponent : GeneralPriorityVolumeComponent<RadialForceVolumeData> { }
}
