using ModDataTools.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Assets.Resources
{
    [Serializable]
    public abstract class AssetResource
    {
        public readonly string AssetPath;
        public readonly string AssetBundle;
        public readonly string OutputPath;

        public AssetResource() { }
        public AssetResource(UnityEngine.Object resource, StarSystemAsset starSystem) : this(resource, starSystem.GetResourcePath(resource)) { }
        public AssetResource(UnityEngine.Object resource, PlanetAsset planet) : this(resource, planet.GetResourcePath(resource)) { }
        public AssetResource(UnityEngine.Object resource, string outputPath)
        {
            AssetPath = AssetRepository.GetAssetPath(resource);
            AssetBundle = AssetRepository.GetAssetBundle(resource);
            OutputPath = outputPath;
        }

        public abstract UnityEngine.Object GetResource();
    }
}
