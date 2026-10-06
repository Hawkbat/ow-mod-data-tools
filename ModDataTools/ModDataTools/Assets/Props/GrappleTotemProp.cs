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
    public class GrappleTotemPropData : GeneralPropData
    {
        [Tooltip("The minimum distance that the player must be from the grapple totem for it to activate.")]
        public float MinDistance = 10f;
        [Tooltip("The distance from the grapple totem that the player will stop at when it activates.")]
        public float ArrivalDistance = 4f;
        [Tooltip("The maximum angle in degrees allowed between the grapple totem's face and the player's lantern in order to activate the totem.")]
        public float MaxAngle = 45f;
        [Tooltip("The maximum distance allowed between the grapple totem's face and the player's lantern in order to activate the totem.")]
        public float MaxDistance = 29f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (MinDistance != 10f)
                writer.WriteProperty("minDistance", MinDistance);
            if (ArrivalDistance != 4f)
                writer.WriteProperty("arrivalDistance", ArrivalDistance);
            if (MaxAngle != 45f)
                writer.WriteProperty("maxAngle", MaxAngle);
            if (MaxDistance != 29f)
                writer.WriteProperty("maxDistance", MaxDistance);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(GrappleTotemPropAsset))]
    public class GrappleTotemPropAsset : GeneralPropAsset<GrappleTotemPropData> { }
    public class GrappleTotemPropComponent : GeneralPropComponent<GrappleTotemPropData> { }
}
