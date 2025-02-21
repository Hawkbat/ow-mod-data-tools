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
    public class DreamModule : PlanetModule
    {
        [Tooltip("Setting this value will make this body a dream world style dimension where its contents are only activated while entering it from a dream campfire. Disables the body's map marker.")]
        public bool IsDreamWorld;
        [Tooltip("Whether to generate simulation meshes (the models used in the \"tronworld\" or \"matrix\" view) for most objects on this planet by cloning the existing meshes and applying the simulation materials. Leave this off if you are building your own simulation meshes or using existing objects which have them.")]
        public bool GenerateSimulationMeshes;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (IsDreamWorld)
                writer.WriteProperty("isDreamWorld", IsDreamWorld);
            if (GenerateSimulationMeshes)
                writer.WriteProperty("generateSimulationMeshes", GenerateSimulationMeshes);
        }
    }
}
