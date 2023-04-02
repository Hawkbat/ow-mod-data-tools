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
    public class SandModule : PlanetModule
    {
        [Tooltip("Size of the sand sphere")]
        public float Size;
        [Tooltip("Tint of the sand")]
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
