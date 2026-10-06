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
    public class CloakModule : PlanetModule
    {
        [Tooltip("Radius of the cloaking field around the planet. It's a bit finicky so experiment with different values.")]
        public float Radius;
        public NullishSingle InnerCloakRadius;
        public NullishSingle NearCloakRadius;
        public NullishSingle FarCloakRadius;
        [Tooltip("Not sure what this is. For the Stranger it is 2000. Optional (will default to be proportional to the cloak radius).")]
        public NullishSingle CloakScaleDistance;
        [Tooltip("The audio that will play when entering the cloaking field.")]
        public AudioConfig Audio;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("radius", Radius);
            writer.WriteProperty("cloakScaleDist", CloakScaleDistance);
            writer.WriteProperty("innerCloakRadius", InnerCloakRadius);
            writer.WriteProperty("nearCloakRadius", NearCloakRadius);
            writer.WriteProperty("farCloakRadius", FarCloakRadius);
            writer.WriteProperty("audio", Audio, planet.GetResourcePath);
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            foreach (var resource in Audio.GetResources(planet.GetResourcePath))
                yield return resource;
        }
    }
}
