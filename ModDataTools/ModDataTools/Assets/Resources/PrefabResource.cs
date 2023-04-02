using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Resources
{
    [Serializable]
    public class PrefabResource : AssetResource
    {
        public GameObject Prefab;

        public PrefabResource() { }
        public PrefabResource(GameObject prefab, StarSystemAsset starSystem) : base(prefab, starSystem) { Prefab = prefab; }
        public PrefabResource(GameObject prefab, PlanetAsset planet) : base(prefab, planet) { Prefab = prefab; }
        public PrefabResource(GameObject prefab, string outputPath) : base(prefab, outputPath) { Prefab = prefab; }

        public override UnityEngine.Object GetResource() => Prefab;
    }
}
