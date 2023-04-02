using ModDataTools.Assets.Resources;
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
    public class RingsModule : PlanetModule
    {
        [Tooltip("The texture used for the rings.")]
        public Texture2D Texture;
        [Tooltip("Should this ring be unlit?")]
        public bool Unlit;
        [Tooltip("Inner radius of the disk")]
        public float InnerRadius;
        [Tooltip("Outer radius of the disk")]
        public float OuterRadius;
        [Tooltip("Angle between the rings and the equatorial plane of the planet.")]
        public float Inclination;
        [Tooltip("Angle defining the point where the rings rise up from the planet's equatorial plane if inclination is nonzero.")]
        public float LongitudeOfAscendingNode;
        [Tooltip("Allows the rings to rotate.")]
        public float RotationSpeed;
        [Tooltip("Fluid type for sounds/effects when colliding with this ring.")]
        public RingsFluidType FluidType;
        [Tooltip("Scale rings over time. Optional. Value between 0-1, time is in minutes.")]
        public AnimationCurve ScaleCurve;
        [Tooltip("Fade rings in/out over time. Optional. Value between 0-1, time is in minutes.")]
        public AnimationCurve OpacityCurve;
        [Tooltip("An optional rename of this object")]
        public string Rename;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (FluidType != RingsFluidType.None)
                writer.WriteProperty("fluidType", FluidType);
            if (Inclination != 0f)
                writer.WriteProperty("inclination", Inclination);
            if (InnerRadius != 0f)
                writer.WriteProperty("innerRadius", InnerRadius);
            if (LongitudeOfAscendingNode != 0f)
                writer.WriteProperty("longitudeOfAscendingNode", LongitudeOfAscendingNode);
            if (OuterRadius != 0f)
                writer.WriteProperty("outerRadius", OuterRadius);
            if (RotationSpeed != 0f)
                writer.WriteProperty("rotationSpeed", RotationSpeed);
            if (Texture)
                writer.WriteProperty("texture", planet.GetResourcePath(Texture));
            if (Unlit)
                writer.WriteProperty("unlit", Unlit);
            if (ScaleCurve != null && ScaleCurve.keys.Any())
                writer.WriteProperty("scaleCurve", ScaleCurve);
            if (OpacityCurve != null && OpacityCurve.keys.Any())
                writer.WriteProperty("opacityCurve", OpacityCurve);
            if (!string.IsNullOrEmpty(Rename))
                writer.WriteProperty("rename", Rename);
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            if (Texture)
                yield return new ImageResource(Texture, planet);
        }

        public enum RingsFluidType
        {
            None = 0,
            Water = 1,
            Cloud = 2,
            Sand = 3,
            Plasma = 4,
        }
    }
}
