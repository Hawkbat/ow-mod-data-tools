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
    public class ProcGenModule : PlanetModule
    {
        [Tooltip("The size of the planet mesh")]
        public float Size;
        [Tooltip("The color of the planet mesh")]
        public Color Color = Color.white;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (Color != Color.white)
                writer.WriteProperty("color", (Color32)Color);
            if (Size != 0f)
                writer.WriteProperty("size", Size);
        }
    }
}
