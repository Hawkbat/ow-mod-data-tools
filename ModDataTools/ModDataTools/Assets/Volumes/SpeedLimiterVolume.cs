using ModDataTools.Assets.Props;
using ModDataTools.Assets.Resources;
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
    public class SpeedLimiterVolumeData : GeneralVolumeData
    {
        [Tooltip("The speed the volume will slow you down to when you enter it.")]
        public float MaxSpeed = 10f;
        [Tooltip("The distance from the outside of the volume that the limiter slows you down to max speed at.")]
        public float StoppingDistance = 100f;
        [Tooltip("The maximum angle (in degrees) between the direction the incoming object is moving relative to the volume's center and the line from the object toward the center of the volume, within which the speed limiter will activate.")]
        public float MaxEntryAngle = 60f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (MaxSpeed != 10f)
                writer.WriteProperty("maxSpeed", MaxSpeed);
            if (StoppingDistance != 100f)
                writer.WriteProperty("stoppingDistance", StoppingDistance);
            if (MaxEntryAngle != 60f)
                writer.WriteProperty("maxEntryAngle", MaxEntryAngle);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(SpeedLimiterVolumeAsset))]
    public class SpeedLimiterVolumeAsset : GeneralVolumeAsset<SpeedLimiterVolumeData> { }
    public class SpeedLimiterVolumeComponent : GeneralVolumeComponent<SpeedLimiterVolumeData> { }
}
