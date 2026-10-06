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
    public class ParticleFieldModule : PlanetModule
    {
        [Tooltip("Particle type for this vection field.")]
        public ParticleFieldType Type;
        [Tooltip("What the particle field activates based on.")]
        public FollowTargetType FollowTarget;
        [Tooltip("Density by height curve. Determines how many particles are emitted at different heights. Defaults to a curve based on minimum and maximum heights of various other modules.")]
        public AnimationCurve DensityByHeightCurve;
        [Tooltip("An optional rename of this object.")]
        public string Rename;
        [Tooltip("Overrides the radius of the field around the player or probe. Strongly effects visual density, due to how volume works. Defaults: Rain 20, SnowflakesHeavy 20, SnowflakesLight 10, Embers 30, Clouds 60, Leaves 30, Bubbles 40, Fog 60, CrystalMotes 30, RockMotes 30, IceMotes 30, SandMotes 10, Crawlies 20, Fireflies 30, Plankton 20, Pollen 20, Current 30.")]
        public NullishSingle OverrideFieldRadius;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("type", Type);
            writer.WriteProperty("followTarget", FollowTarget);
            if (DensityByHeightCurve != null && DensityByHeightCurve.keys.Any())
                writer.WriteProperty("densityByHeightCurve", DensityByHeightCurve, "height", "density");
            if (!string.IsNullOrEmpty(Rename))
                writer.WriteProperty("rename", Rename);
            writer.WriteProperty("overrideFieldRadius", OverrideFieldRadius);
        }

        public enum ParticleFieldType
        {
            Rain = 0,
            SnowflakesHeavy = 1,
            SnowflakesLight = 2,
            Embers = 3,
            Clouds = 4,
            Leaves = 5,
            Bubbles = 6,
            Fog = 7,
            CrystalMotes = 8,
            RockMotes = 9,
            IceMotes = 10,
            SandMotes = 11,
            Crawlies = 12,
            Fireflies = 13,
            Plankton = 14,
            Pollen = 15,
            Current = 16,
        }

        public enum FollowTargetType
        {
            Player = 0,
            Probe = 1,
        }
    }
}
