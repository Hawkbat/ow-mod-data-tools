using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Resources
{
    [Serializable]
    public class ImageResource : AssetResource
    {
        public Texture2D Image;

        public ImageResource() { }
        public ImageResource(Texture2D image, StarSystemAsset starSystem) : base(image, starSystem) { Image = image; }
        public ImageResource(Texture2D image, PlanetAsset planet) : base(image, planet) { Image = image; }
        public ImageResource(Texture2D image, string outputPath) : base(image, outputPath) { Image = image; }

        public override UnityEngine.Object GetResource() => Image;
    }
}
