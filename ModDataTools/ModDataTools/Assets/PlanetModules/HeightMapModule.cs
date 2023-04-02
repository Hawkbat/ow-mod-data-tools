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
    public class HeightMapModule : PlanetModule
    {
        [Tooltip("The texture used for the terrain height.")]
        public Texture2D HeightMap;
        [Tooltip("The lowest points on your planet will be at this height.")]
        public float MinHeight;
        [Tooltip("The highest points on your planet will be at this height.")]
        public float MaxHeight;
        [Tooltip("The scale of the terrain.")]
        public Vector3 Stretch = Vector3.one;
        [Tooltip("The texture used for the terrain.")]
        public Texture2D TextureMap;
        [Tooltip("Resolution of the heightmap. Higher values means more detail but also more memory/cpu/gpu usage. This value will be 1:1 with the heightmap texture width, but only at the equator.")]
        [Range(4f, 2000f)]
        public int Resolution = 204;
        [Tooltip("The texture used for emission. Optional.")]
        public Texture2D EmissionMap;
        [Tooltip("Color multiplier of the emission texture.")]
        [ConditionalField(nameof(EmissionMap))]
        public Color EmissionColor = Color.white;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (HeightMap)
                writer.WriteProperty("heightMap", planet.GetResourcePath(HeightMap));
            writer.WriteProperty("maxHeight", MaxHeight);
            writer.WriteProperty("minHeight", MinHeight);
            if (Stretch != Vector3.one)
                writer.WriteProperty("stretch", Stretch);
            if (TextureMap)
                writer.WriteProperty("textureMap", planet.GetResourcePath(TextureMap));
            if (Resolution != 204)
                writer.WriteProperty("resolution", Resolution);
            if (EmissionMap)
                writer.WriteProperty("emissionMap", planet.GetResourcePath(EmissionMap));
            if (EmissionColor != Color.white)
                writer.WriteProperty("emissionColor", (Color32)EmissionColor);
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            if (HeightMap)
                yield return new ImageResource(HeightMap, planet);
            if (TextureMap)
                yield return new ImageResource(TextureMap, planet);
            if (EmissionMap)
                yield return new ImageResource(EmissionMap, planet);
        }
    }
}
