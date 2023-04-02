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
    public class VolcanoPropData : GeneralPointPropData
    {
        [Tooltip("The colour of the meteor's lava.")]
        public NullishColor LavaTint;
        [Tooltip("The colour of the meteor's stone.")]
        public NullishColor StoneTint;
        [Tooltip("Minimum time between meteor launches.")]
        public float MinInterval = 5f;
        [Tooltip("Maximum time between meteor launches.")]
        public float MaxInterval = 20f;
        [Tooltip("Minimum random speed at which meteors are launched.")]
        public float MinLaunchSpeed = 50f;
        [Tooltip("Maximum random speed at which meteors are launched.")]
        public float MaxLaunchSpeed = 150f;
        [Tooltip("Scale of the meteors.")]
        public float Scale = 1f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("lavaTint", LavaTint);
            writer.WriteProperty("maxInterval", MaxInterval);
            writer.WriteProperty("maxLaunchSpeed", MaxLaunchSpeed);
            writer.WriteProperty("minInterval", MinInterval);
            writer.WriteProperty("minLaunchSpeed", MinLaunchSpeed);
            writer.WriteProperty("scale", Scale);
            writer.WriteProperty("stoneTint", StoneTint);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(VolcanoPropAsset))]
    public class VolcanoPropAsset : GeneralPointPropAsset<VolcanoPropData> { }
    public class VolcanoPropComponent : GeneralPointPropComponent<VolcanoPropData> { }
}
