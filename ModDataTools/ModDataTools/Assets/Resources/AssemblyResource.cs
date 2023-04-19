using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Resources
{
    [Serializable]
    public class AssemblyResource : AssetResource
    {
        public UnityEngine.Object Assembly;

        public AssemblyResource() { }
        public AssemblyResource(UnityEngine.Object assembly, StarSystemAsset starSystem) : base(assembly, starSystem) { Assembly = assembly; }
        public AssemblyResource(UnityEngine.Object assembly, PlanetAsset planet) : base(assembly, planet) { Assembly = assembly; }
        public AssemblyResource(UnityEngine.Object assembly, string outputPath) : base(assembly, outputPath) { Assembly = assembly; }

        public override UnityEngine.Object GetResource() => Assembly;
    }
}
