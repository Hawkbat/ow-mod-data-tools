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
    public class CometTailModule : PlanetModule
    {
        [Tooltip("Scale this object over time. Time value is in minutes.")]
        public AnimationCurve Curve;
        [Tooltip("Manually sets the local rotation.")]
        public NullishVector3 RotationOverride;
        [Tooltip("Inner radius of the comet tail, defaults to match surfaceSize.")]
        public NullishSingle InnerRadius;
        [Tooltip("The body that the comet tail should always point away from.")]
        public PlanetAsset PrimaryBody;
        [Tooltip("Colour of the dust tail (the shorter part).")]
        public NullishColor DustTint;
        [Tooltip("Colour of the gas tail (the longer part).")]
        public NullishColor GasTint;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (Curve != null && Curve.keys.Any())
                writer.WriteProperty("curve", Curve);
            writer.WriteProperty("rotationOverride", RotationOverride);
            writer.WriteProperty("innerRadius", InnerRadius);
            if (PrimaryBody)
                writer.WriteProperty("primaryBody", PrimaryBody.FullID);
            writer.WriteProperty("dustTint", DustTint);
            writer.WriteProperty("gasTint", GasTint);
        }
    }
}
