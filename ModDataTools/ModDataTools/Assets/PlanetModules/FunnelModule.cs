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
    public class FunnelModule : PlanetModule
    {
        [Tooltip("The planet the funnel will flow to")]
        public PlanetAsset Target;
        [Tooltip("Type of fluid the funnel transfers")]
        public FluidType Type;
        [Tooltip("Tint of the funnel")]
        public NullishColor Tint;
        [Tooltip("Scale this object over time")]
        public AnimationCurve Curve;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (Curve != null && Curve.keys.Any())
                writer.WriteProperty("curve", Curve);
            if (Target)
                writer.WriteProperty("target", Target.FullID);
            writer.WriteProperty("tint", Tint);
            if (Type != FluidType.Sand)
                writer.WriteProperty("type", Type);
        }

        public enum FluidType
        {
            Sand = 0,
            Water = 1,
            Lava = 2,
            Star = 3,
        }
    }
}
