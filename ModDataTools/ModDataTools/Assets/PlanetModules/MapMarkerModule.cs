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
    public class MapMarkerModule : PlanetModule
    {
        [Tooltip("Lowest distance away from the body that the marker can be shown. This is automatically set to 0 for all bodies except focal points where it is 5,000.")]
        public NullishSingle MinDisplayDistanceOverride;
        [Tooltip("Highest distance away from the body that the marker can be shown. For planets and focal points the automatic value is 50,000. Moons and planets in focal points are 5,000. Stars are 1E+10 (10,000,000,000).")]
        public NullishSingle MaxDisplayDistanceOverride;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("enabled", IsEnabled);
            if (!IsEnabled) return;

            writer.WriteProperty("minDisplayDistanceOverride", MinDisplayDistanceOverride);
            writer.WriteProperty("maxDisplayDistanceOverride", MaxDisplayDistanceOverride);
        }

        public override bool ShouldWrite(PlanetAsset planet) => true;
    }
}
