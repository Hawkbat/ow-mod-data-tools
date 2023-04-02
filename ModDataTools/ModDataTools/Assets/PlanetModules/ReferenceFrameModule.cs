using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.PlanetModules
{
    [Serializable]
    public class ReferenceFrameModule : PlanetModule
    {
        [Tooltip("Stop the object from being targeted on the map.")]
        public bool HideInMap;
        [Tooltip("Radius of the brackets that show up when you target this. Defaults to the sphere of influence.")]
        public NullishSingle BracketRadius;
        [Tooltip("If it should be targetable even when super close.")]
        public bool TargetWhenClose;
        [Tooltip("The maximum distance that the reference frame can be targeted from. Defaults to 100km and cannot be greater than that.")]
        public NullishSingle MaxTargetDistance;
        [Tooltip("The radius of the sphere around the planet which you can click on to target it. Defaults to twice the sphere of influence.")]
        public NullishSingle TargetColliderRadius;
        [Tooltip("Position of the reference frame relative to the object.")]
        public Vector3 LocalPosition;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (!IsEnabled)
                writer.WriteProperty("enabled", IsEnabled);
            if (HideInMap)
                writer.WriteProperty("hideInMap", HideInMap);
            writer.WriteProperty("bracketRadius", BracketRadius);
            if (TargetWhenClose)
                writer.WriteProperty("targetWhenClose", TargetWhenClose);
            writer.WriteProperty("maxTargetDistance", MaxTargetDistance);
            writer.WriteProperty("targetColliderRadius", TargetColliderRadius);
            if (LocalPosition != Vector3.zero)
                writer.WriteProperty("localPosition", LocalPosition);
        }

        public override bool ShouldWrite(PlanetAsset planet) => true;
    }
}
