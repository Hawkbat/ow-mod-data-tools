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
    public class SpeedTrapVolumeData : GeneralVolumeData
    {
        [Tooltip("The speed the volume will slow you down to when you enter it.")]
        public float SpeedLimit = 10f;
        [Tooltip("How fast it will slow down the player to the speed limit.")]
        public float Acceleration = 3f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (SpeedLimit != 10f)
                writer.WriteProperty("speedLimit", SpeedLimit);
            if (Acceleration != 3f)
                writer.WriteProperty("acceleration", Acceleration);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(SpeedTrapVolumeAsset))]
    public class SpeedTrapVolumeAsset : GeneralVolumeAsset<SpeedTrapVolumeData> { }
    public class SpeedTrapVolumeComponent : GeneralVolumeComponent<SpeedTrapVolumeData> { }
}
