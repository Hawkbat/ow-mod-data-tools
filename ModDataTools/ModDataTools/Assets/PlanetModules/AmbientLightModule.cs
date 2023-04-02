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
    public class AmbientLightModule : PlanetModule
    {
        [Tooltip("The lower radius where the light is brightest, fading in from outerRadius. Defaults to surfaceSize.")]
        public NullishSingle InnerRadius;
        [Tooltip("The range of the light. Defaults to surfaceSize * 2.")]
        public NullishSingle OuterRadius;
        [Tooltip("The brightness of the light. For reference, Timber Hearth is 1.4, and Giant's Deep is 0.8.")]
        public NullishSingle Intensity;
        [Tooltip("The tint of the light")]
        public NullishColor Tint;
        [Tooltip("If true, the light will work as a shell between inner and outer radius.")]
        public bool IsShell;
        [Tooltip("The position of the light")]
        public Vector3 Position;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("outerRadius", OuterRadius);
            writer.WriteProperty("innerRadius", InnerRadius);
            writer.WriteProperty("intensity", Intensity);
            writer.WriteProperty("tint", Tint);
            if (IsShell)
                writer.WriteProperty("isShell", IsShell);
            writer.WriteProperty("position", Position);
        }
    }
}
