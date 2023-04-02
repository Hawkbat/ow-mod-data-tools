using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Resources
{
    [Serializable]
    public class TextResource : AssetResource
    {
        public TextAsset Text;

        public TextResource() { }
        public TextResource(TextAsset text, StarSystemAsset starSystem) : base(text, starSystem) { Text = text; }
        public TextResource(TextAsset text, PlanetAsset planet) : base(text, planet) { Text = text; }
        public TextResource(TextAsset text, string outputPath) : base(text, outputPath) { Text = text; }
        public TextResource(string text, StarSystemAsset starSystem) : this(new TextAsset(text), starSystem) { }
        public TextResource(string text, PlanetAsset planet) : this(new TextAsset(text), planet) { }
        public TextResource(string text, string outputPath) : this(new TextAsset(text), outputPath) { }

        public override UnityEngine.Object GetResource() => Text;
    }
}
