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
    public abstract class PlanetModule
    {
        [HideInInspector]
        public bool IsEnabled;

        public void WriteJsonObject(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteStartObject();
            WriteJsonProps(planet, writer);
            writer.WriteEndObject();
        }

        public abstract void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer);

        public virtual bool ShouldWrite(PlanetAsset planet) => IsEnabled && !planet.NewHorizons.Destroy;

        public virtual IEnumerable<AssetResource> GetResources(PlanetAsset planet)
            => Enumerable.Empty<AssetResource>();
    }
}
