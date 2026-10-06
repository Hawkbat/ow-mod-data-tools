using ModDataTools.Assets.Resources;
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
    public class AlarmTotemPropData : GeneralPropData
    {
        [Tooltip("The maximum distance of the alarm's vision cone.")]
        public float SightDistance = 45f;
        [Tooltip("The width of the alarm's vision cone in degrees.")]
        public float SightAngle = 60f;
        [Tooltip("Scales the visible vision cone in the simulation view (does not affect the actual vision cone detection).")]
        public NullishVector3 StretchVisionCone;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (SightDistance != 45f)
                writer.WriteProperty("sightDistance", SightDistance);
            if (SightAngle != 60f)
                writer.WriteProperty("sightAngle", SightAngle);
            writer.WriteProperty("stretchVisionCone", StretchVisionCone);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(AlarmTotemPropAsset))]
    public class AlarmTotemPropAsset : GeneralPropAsset<AlarmTotemPropData> { }
    public class AlarmTotemPropComponent : GeneralPropComponent<AlarmTotemPropData> { }
}
