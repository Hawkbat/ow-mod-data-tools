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
    public class LavaModule : PlanetModule
    {
        [Tooltip("Size of the lava sphere")]
        public float Size;
        [Tooltip("Tint of the lava")]
        public NullishColor Tint;
        [Tooltip("Scale this object over time")]
        public AnimationCurve Curve;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("size", Size);
            if (Curve != null && Curve.keys.Any())
                writer.WriteProperty("curve", Curve);
            writer.WriteProperty("tint", Tint);
        }
    }
}
