using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Resources
{
    [Serializable]
    public class MeshResource : AssetResource
    {
        public Mesh Mesh;

        public MeshResource() { }
        public MeshResource(Mesh mesh, StarSystemAsset starSystem) : base(mesh, starSystem) { Mesh = mesh; }
        public MeshResource(Mesh mesh, PlanetAsset planet) : base(mesh, planet) { Mesh = mesh; }
        public MeshResource(Mesh mesh, string outputPath) : base(mesh, outputPath) { Mesh = mesh; }

        public override UnityEngine.Object GetResource() => Mesh;
    }
}
