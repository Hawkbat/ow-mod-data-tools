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
    public class StarModule : PlanetModule
    {
        [Tooltip("Radius of the star. Defaults to 2000m.")]
        public NullishSingle Size;
        [Tooltip("Scale this object over time")]
        public AnimationCurve Curve;
        [Tooltip("The texture to put as the star ramp. Optional.")]
        public Texture2D StarRamp;
        [Tooltip("The texture to put as the star ramp while it is collapsing. Optional.")]
        public Texture2D StarCollapseRamp;
        [Tooltip("Colour of the star.")]
        public NullishColor Tint;
        [Tooltip("Colour of the star at the end of its lifespan.")]
        public NullishColor EndTint;
        [Tooltip("Colour of the light given off. Defaults to yellowish.")]
        public NullishColor LightTint;
        [Tooltip("How far the light from the star can reach. Defaults to 50,000m.")]
        public NullishSingle LightRadius;
        [Tooltip("Relative strength of the light compared to the sun.")]
        public float SolarLuminosity = 1f;
        [Tooltip("How long in minutes this star will last until it supernovas.")]
        public float Lifespan = 22f;
        [Tooltip("The type of death your star will have.")]
        public DeathType StellarDeathType = DeathType.Default;
        [Tooltip("Radius of the supernova. Any planets within this will be destroyed. Defaults to 50,000m.")]
        [ConditionalField(nameof(StellarDeathType), DeathType.None, Invert = true)]
        public NullishSingle SupernovaSize;
        [Tooltip("Speed of the supernova wall in meters per second. Defaults to 1000 m/s.")]
        [ConditionalField(nameof(StellarDeathType), DeathType.None, Invert = true)]
        public NullishSingle SupernovaSpeed;
        [Tooltip("The tint of the supernova this star creates when it dies.")]
        [ConditionalField(nameof(StellarDeathType), DeathType.None, Invert = true)]
        public NullishColor SupernovaTint;
        [Tooltip("The type of stellar remnant your star will leave behind.")]
        public RemnantType StellarRemnantType = RemnantType.Default;
        [Tooltip("Should we add a star controller to this body? If you want clouds to work on a binary brown dwarf system, set this to false.")]
        public bool HasStarController = true;
        [Tooltip("The default sun has its own atmosphere that is different from regular planets. If you want that, set this to true.")]
        public bool HasAtmosphere = true;
        [Tooltip("Size multiplier for solar flares. Defaults to 1.")]
        public NullishSingle SolarFlareScaleFactor;
        [Tooltip("How long a solar flare is visible for. Defaults to 15 seconds.")]
        public NullishSingle SolarFlareLifeLength;
        [Tooltip("Solar flares are emitted randomly. This is the minimum amount of time between solar flares. Defaults to 5 seconds.")]
        public NullishSingle MinTimeBetweenSolarFlares;
        [Tooltip("Solar flares are emitted randomly. This is the maximum amount of time between solar flares. Defaults to 30 seconds.")]
        public NullishSingle MaxTimeBetweenSolarFlares;

        public enum DeathType
        {
            Default = 0,
            None = 1,
            PlanetaryNebula = 2,
            Supernova = 3,
        }

        public enum RemnantType
        {
            Default = 0,
            WhiteDwarf = 1,
            NeutronStar = 2,
            Pulsar = 3,
            BlackHole = 4,
            Custom = 5,
        }

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (Curve != null && Curve.keys.Any())
                writer.WriteProperty("curve", Curve);
            writer.WriteProperty("endTint", EndTint);
            if (Lifespan != 22f)
                writer.WriteProperty("lifespan", Lifespan);
            if (!HasStarController)
                writer.WriteProperty("hasStarController", HasStarController);
            if (!HasAtmosphere)
                writer.WriteProperty("hasAtmosphere", HasAtmosphere);
            writer.WriteProperty("lightTint", LightTint);
            writer.WriteProperty("size", Size);
            if (SolarLuminosity != 1f)
                writer.WriteProperty("solarLuminosity", SolarLuminosity);
            writer.WriteProperty("supernovaSize", SupernovaSize);
            writer.WriteProperty("supernovaSpeed", SupernovaSpeed);
            writer.WriteProperty("supernovaTint", SupernovaTint);
            writer.WriteProperty("tint", Tint);
            if (StarRamp)
                writer.WriteProperty("starRampTexture", planet.GetResourcePath(StarRamp));
            if (StarCollapseRamp)
                writer.WriteProperty("starCollapseRampTexture", planet.GetResourcePath(StarCollapseRamp));
            writer.WriteProperty("lightRadius", LightRadius);
            if (StellarDeathType != DeathType.Default)
                writer.WriteProperty("stellarDeathType", StellarDeathType);
            if (StellarRemnantType != RemnantType.Default)
                writer.WriteProperty("stellarRemnantType", StellarRemnantType);
            if (SolarFlareScaleFactor.HasValue || SolarFlareLifeLength.HasValue || MinTimeBetweenSolarFlares.HasValue || MaxTimeBetweenSolarFlares.HasValue)
            {
                writer.WritePropertyName("solarFlareSettings");
                writer.WriteStartObject();
                writer.WriteProperty("scaleFactor", SolarFlareScaleFactor);
                writer.WriteProperty("lifeLength", SolarFlareLifeLength);
                writer.WriteProperty("minTimeBetweenFlares", MinTimeBetweenSolarFlares);
                writer.WriteProperty("maxTimeBetweenFlares", MaxTimeBetweenSolarFlares);
                writer.WriteEndObject();
            }
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            if (StarRamp)
                yield return new ImageResource(StarRamp, planet);
            if (StarCollapseRamp)
                yield return new ImageResource(StarCollapseRamp, planet);
        }
    }
}
