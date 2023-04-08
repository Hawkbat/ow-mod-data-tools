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
    public class AtmosphereModule : PlanetModule
    {
        [Tooltip("Scale height of the atmosphere")]
        public float Size;
        [Tooltip("Colour of atmospheric shader on the planet.")]
        public NullishColor AtmosphereTint;
        [Tooltip("How intense should the sun appear in the sky. Also affects general atmosphere brightness. Default value of 1 matches Timber Hearth.")]
        public NullishSingle AtmosphereSunIntensity;
        [Tooltip("Describes the clouds in the atmosphere")]
        public CloudsSubModule Clouds;
        [Tooltip("Whether the planet has fog")]
        public bool HasFog;
        [Tooltip("How dense the fog is.")]
        [ConditionalField(nameof(HasFog))]
        public float FogDensity;
        [Tooltip("Radius of fog sphere, independent of the atmosphere. This has to be set for there to be fog.")]
        [ConditionalField(nameof(HasFog))]
        public float FogSize;
        [Tooltip("Colour of fog on the planet.")]
        [ConditionalField(nameof(HasFog))]
        public NullishColor FogTint;
        [Tooltip("Lets you survive on the planet without a suit.")]
        public bool HasOxygen;
        [Tooltip("Does this planet have trees? This will change the notification from \"Oxygen tank refilled\" to \"Trees detected, oxygen tank refilled\".")]
        public bool HasTrees;
        [Tooltip("Does this planet have rain?")]
        public bool HasRain;
        [Tooltip("Does this planet have snow?")]
        public bool HasSnow;
        [Tooltip("Whether we use an atmospheric shader on the planet. Doesn't affect clouds, fog, rain, snow, oxygen, etc. Purely visual.")]
        public bool UseAtmosphereShader;
        [Tooltip("Whether this atmosphere will have flames appear when your ship goes a certain speed.")]
        public bool HasShockLayer = true;
        [Tooltip("Minimum speed that your ship can go in the atmosphere where flames will appear.")]
        [ConditionalField(nameof(HasShockLayer))]
        public float MinShockSpeed = 100f;
        [Tooltip("Maximum speed that your ship can go in the atmosphere where flames will appear at their brightest.")]
        [ConditionalField(nameof(HasShockLayer))]
        public float MaxShockSpeed = 300f;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("atmosphereTint", AtmosphereTint);
            writer.WriteProperty("atmosphereSunIntensity", AtmosphereSunIntensity);
            writer.WriteProperty("clouds", Clouds, planet);
            if (HasFog)
            {
                writer.WriteProperty("fogDensity", FogDensity);
                writer.WriteProperty("fogSize", FogSize);
                writer.WriteProperty("fogTint", FogTint);
            }
            if (HasOxygen)
                writer.WriteProperty("hasOxygen", HasOxygen);
            if (HasTrees)
                writer.WriteProperty("hasTrees", HasTrees);
            if (HasRain)
                writer.WriteProperty("hasRain", HasRain);
            if (HasSnow)
                writer.WriteProperty("hasSnow", HasSnow);
            writer.WriteProperty("size", Size);
            if (UseAtmosphereShader)
                writer.WriteProperty("useAtmosphereShader", UseAtmosphereShader);
            writer.WriteProperty("hasShockLayer", HasShockLayer);
            if (HasShockLayer)
            {
                writer.WriteProperty("minShockSpeed", MinShockSpeed);
                writer.WriteProperty("maxShockSpeed", MaxShockSpeed);
            }
        }

        [Serializable]
        public class CloudsSubModule : PlanetModule
        {
            [Tooltip("Radius from the center to the inner layer of the clouds.")]
            public float InnerCloudRadius;
            [Tooltip("Radius from the center to the outer layer of the clouds.")]
            public float OuterCloudRadius;
            [Tooltip("Should these clouds be based on Giant's Deep's banded clouds, or the Quantum Moon's non-banded clouds?")]
            public CloudsPrefabType CloudsPrefab;
            [Tooltip("The cloud texture, if the planet has clouds.")]
            public Texture2D Texture;
            [Tooltip("The cloud cap texture, if the planet has clouds.")]
            public Texture2D Cap;
            [Tooltip("The cloud ramp texture, if the planet has clouds. If you don't put anything here it will be auto-generated.")]
            public Texture2D Ramp;
            [Tooltip("Colour of the inner cloud layer.")]
            public NullishColor Tint;
            [Tooltip("If the top layer shouldn't have shadows. Set to true if you're making a brown dwarf for example.")]
            public bool Unlit;
            [Tooltip("How fast the clouds will rotate relative to the planet in degrees per second.")]
            public float RotationSpeed;
            [Tooltip("Fluid type for sounds/effects when colliding with this cloud.")]
            public CloudsFluidType FluidType = CloudsFluidType.Cloud;
            [Tooltip("Add lightning to this planet like on Giant's Deep.")]
            public bool HasLightning;
            [Tooltip("Colour gradient of the lightning, time is in seconds.")]
            [ConditionalField(nameof(HasLightning))]
            public Gradient LightningGradient;

            public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
            {
                writer.WriteProperty("cloudsPrefab", CloudsPrefab);
                if (Cap)
                    writer.WriteProperty("capPath", planet.GetResourcePath(Cap));
                if (FluidType != CloudsFluidType.Cloud)
                    writer.WriteProperty("fluidType", FluidType);
                if (HasLightning)
                    writer.WriteProperty("hasLightning", HasLightning);
                writer.WriteProperty("innerCloudRadius", InnerCloudRadius);
                if (HasLightning)
                    writer.WriteProperty("lightningGradient", LightningGradient);
                writer.WriteProperty("outerCloudRadius", OuterCloudRadius);
                if (Ramp)
                    writer.WriteProperty("rampPath", planet.GetResourcePath(Ramp));
                if (Texture)
                    writer.WriteProperty("texturePath", planet.GetResourcePath(Texture));
                writer.WriteProperty("tint", Tint);
                if (Unlit)
                    writer.WriteProperty("unlit", Unlit);
                if (RotationSpeed != 0f)
                    writer.WriteProperty("rotationSpeed", RotationSpeed);
            }

            public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
            {
                if (Texture)
                    yield return new ImageResource(Texture, planet);
                if (Cap)
                    yield return new ImageResource(Cap, planet);
                if (Ramp)
                    yield return new ImageResource(Ramp, planet);
            }

            public enum CloudsPrefabType
            {
                GiantsDeep = 0,
                QuantumMoon = 1,
                Basic = 2,
                Transparent = 3,
            }

            public enum CloudsFluidType
            {
                None = 0,
                Water = 1,
                Cloud = 2,
                Sand = 3,
                Plasma = 4,
            }
        }
    }
}
