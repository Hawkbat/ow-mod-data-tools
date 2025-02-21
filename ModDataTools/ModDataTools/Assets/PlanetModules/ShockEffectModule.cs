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
    public class ShockEffectModule : PlanetModule
    {
        [Tooltip("Override the calculated radius of the shock effect")]
        public NullishSingle Radius;
        [Tooltip("The replacement mesh for the planet's supernova shock effect")]
        public Mesh Mesh;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (IsEnabled)
            {
                writer.WriteProperty("radius", Radius);
                if (Mesh)
                {
                    writer.WriteProperty("assetBundle", AssetRepository.GetAssetBundlePath(Mesh));
                    writer.WriteProperty("meshPath", AssetRepository.GetAssetPath(Mesh));
                }
            }
            else
            {
                writer.WriteProperty("hasSupernovaShockEffect", IsEnabled);
            }
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            if (IsEnabled)
            {
                if (Mesh)
                    yield return new MeshResource(Mesh, string.Empty);
            }
        }

        public override bool ShouldWrite(PlanetAsset planet) => true;
    }
}
