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
    public class WaterModule : PlanetModule
    {
        [Tooltip("Size of the water sphere")]
        public float Size;
        [Tooltip("Tint of the water")]
        public NullishColor Tint;
        [Tooltip("Density of the water sphere. The higher the density, the harder it is to go through this fluid.")]
        public float Density = 1.2f;
        [Tooltip("Buoyancy density of the water sphere")]
        public float Buoyancy = 1f;
        [Tooltip("Scale this object over time")]
        public AnimationCurve Curve;


        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("size", Size);
            if (Curve != null && Curve.keys.Any())
                writer.WriteProperty("curve", Curve);
            writer.WriteProperty("density", Density);
            writer.WriteProperty("buoyancy", Buoyancy);
            writer.WriteProperty("tint", Tint);
        }
    }
}
