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

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("type", Type);
            writer.WriteProperty("followTarget", FollowTarget);
            if (DensityByHeightCurve != null && DensityByHeightCurve.keys.Any())
                writer.WriteProperty("densityByHeightCurve", DensityByHeightCurve, "height", "density");
            if (!string.IsNullOrEmpty(Rename))
                writer.WriteProperty("rename", Rename);
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
