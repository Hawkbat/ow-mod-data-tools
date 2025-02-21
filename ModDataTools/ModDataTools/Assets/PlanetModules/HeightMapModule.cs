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
        [Tooltip("Resolution of the heightmap. Higher values means more detail but also more memory/cpu/gpu usage. This value will be 1:1 with the heightmap texture width, but only at the equator.")]
        [Range(4f, 2000f)]
        public int Resolution = 204;
        [Tooltip("The texture used for the terrain colors.")]
        public Texture2D TextureMap;
        [Tooltip("The texture used for the terrain's smoothness and metallic, which are controlled by the texture's alpha and red channels respectively. Optional. Typically black with variable transparency, when metallic isn't wanted.")]
        public Texture2D SmoothnessMap;
        [Tooltip("How 'glossy' the surface is, where 0 is diffuse, and 1 is like a mirror. Multiplies with the alpha of the smoothness map if using one.")]
        [Range(0f, 1f)]
        public float Smoothness;
        [Tooltip("How metallic the surface is, from 0 to 1. Multiplies with the red of the smoothness map if using one.")]
        [Range(0f, 1f)]
        public float Metallic;
        [Tooltip("The texture used for the terrain's normal (aka bump) map. Optional.")]
        public Texture2D NormalMap;
        [Tooltip("Strength of the normal map. Usually 0-1, but can go above, or negative to invert the map.")]
        [ConditionalField(nameof(NormalMap))]
        public float NormalStrength = 1f;
        [Tooltip("The texture used for emission. Optional.")]
        public Texture2D EmissionMap;
        [Tooltip("Color multiplier of the emission texture.")]
        [ConditionalField(nameof(EmissionMap))]
        public Color EmissionColor = Color.white;
        [Tooltip("Whether to use tiles for the terrain. If false, the main texture map will be used as-is. If true, the blend map will be used to combine up to 5 tiles together.")]
        public bool UseTiles;
        [Tooltip("The texture used for blending up to 5 tiles together, using the red, green, blue, and alpha channels, plus a lack of all 4 for a fifth \"base\" tile. Optional, even if using tiles (defaults to white, therefore either base or all other channels will be active).")]
        [ConditionalField(nameof(UseTiles))]
        public Texture2D TileBlendMap;
        [Tooltip("An optional set of textures that can tile and combine with the main maps. This tile will appear when all other tile channels are absent in the blend map, or when no other tiles are defined. Note that tiles will not be active from afar, so it is recommended to make the main textures control the general appearance, and make the tiles handle up close details.")]
        [ConditionalField(nameof(UseTiles))]
        public HeightMapTileConfig BaseTile;
        [Tooltip("An optional set of textures that can tile and combine with the main maps. The distribution of this tile is controlled by red channel of the blend map. Note that tiles will not be active from afar, so it is recommended to make the main maps control the general appearance more than the tiles.")]
        [ConditionalField(nameof(UseTiles))]
        public HeightMapTileConfig RedTile;
        [Tooltip("An optional set of textures that can tile and combine with the main maps. The distribution of this tile is controlled by green channel of the blend map. Note that tiles will not be active from afar, so it is recommended to make the main maps control the general appearance more than the tiles.")]
        [ConditionalField(nameof(UseTiles))]
        public HeightMapTileConfig GreenTile;
        [Tooltip("An optional set of textures that can tile and combine with the main maps. The distribution of this tile is controlled by blue channel of the blend map. Note that tiles will not be active from afar, so it is recommended to make the main maps control the general appearance more than the tiles.")]
        [ConditionalField(nameof(UseTiles))]
        public HeightMapTileConfig BlueTile;
        [Tooltip("An optional set of textures that can tile and combine with the main maps. The distribution of this tile is controlled by alpha channel of the blend map. Note that tiles will not be active from afar, so it is recommended to make the main maps control the general appearance more than the tiles.")]
        [ConditionalField(nameof(UseTiles))]
        public HeightMapTileConfig AlphaTile;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (HeightMap)
                writer.WriteProperty("heightMap", planet.GetResourcePath(HeightMap));
            writer.WriteProperty("maxHeight", MaxHeight);
            writer.WriteProperty("minHeight", MinHeight);
            if (Stretch != Vector3.one)
                writer.WriteProperty("stretch", Stretch);
            if (Resolution != 204)
                writer.WriteProperty("resolution", Resolution);
            if (TextureMap)
                writer.WriteProperty("textureMap", planet.GetResourcePath(TextureMap));
            if (SmoothnessMap)
                writer.WriteProperty("smoothnessMap", planet.GetResourcePath(SmoothnessMap));
            if (Smoothness != 0f)
                writer.WriteProperty("smoothness", Smoothness);
            if (Metallic != 0f)
                writer.WriteProperty("metallic", Metallic);
            if (NormalMap)
                writer.WriteProperty("normalMap", planet.GetResourcePath(NormalMap));
            if (NormalStrength != 1f)
                writer.WriteProperty("normalStrength", NormalStrength);
            if (EmissionMap)
                writer.WriteProperty("emissionMap", planet.GetResourcePath(EmissionMap));
            if (EmissionColor != Color.white)
                writer.WriteProperty("emissionColor", (Color32)EmissionColor);
            if (UseTiles)
            {
                if (TileBlendMap)
                    writer.WriteProperty("tileBlendMap", planet.GetResourcePath(TileBlendMap));
                writer.WritePropertyName("baseTile");
                BaseTile.ToJson(planet, writer);
                writer.WritePropertyName("redTile");
                RedTile.ToJson(planet, writer);
                writer.WritePropertyName("greenTile");
                GreenTile.ToJson(planet, writer);
                writer.WritePropertyName("blueTile");
                BlueTile.ToJson(planet, writer);
                writer.WritePropertyName("alphaTile");
                AlphaTile.ToJson(planet, writer);
            }
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            if (HeightMap)
                yield return new ImageResource(HeightMap, planet);
            if (TextureMap)
                yield return new ImageResource(TextureMap, planet);
            if (SmoothnessMap)
                yield return new ImageResource(SmoothnessMap, planet);
            if (NormalMap)
                yield return new ImageResource(NormalMap, planet);
            if (EmissionMap)
                yield return new ImageResource(EmissionMap, planet);
            if (UseTiles)
            {
                if (TileBlendMap)
                    yield return new ImageResource(TileBlendMap, planet);
                foreach (var resource in BaseTile.GetResources(planet))
                    yield return resource;
                foreach (var resource in RedTile.GetResources(planet))
                    yield return resource;
                foreach (var resource in GreenTile.GetResources(planet))
                    yield return resource;
                foreach (var resource in BlueTile.GetResources(planet))
                    yield return resource;
                foreach (var resource in AlphaTile.GetResources(planet))
                    yield return resource;
            }
        }

        [Serializable]
        public class HeightMapTileConfig
        {
            [Tooltip("The size, in meters, of each tile.")]
            public float Size = 1f;
            [Tooltip("A color texture. Optional. Note that this tile texture will be multiplied with the main texture map. This means that white will multiply by 2, black by 0, and grey by 1. Thus, a texture that stays near (128, 128, 128) will blend nicely with the main texture map below. Colors other than greyscale can be used, but they might multiply strangely.")]
            public Texture2D TextureTile;
            [Tooltip("A texture for smoothness and metallic, which are controlled by the texture's alpha and red channels respectively. Optional. Note that this tile texture will be multiplied with the main smoothness map and/or values. This means that black/red will multiply by 2, transparent by 0, and half transparent by 1. Thus, a texture that stays near half alpha/red will blend nicely with the main smoothness map below.")]
            public Texture2D SmoothnessTile;
            [Tooltip("A texture for normal (aka bump) map. Optional. Blends additively with the main normal map.")]
            public Texture2D NormalTile;
            [Tooltip("Strength of the normal map. Usually 0-1, but can go above, or negative to invert the map.")]
            [ConditionalField(nameof(NormalTile))]
            public float NormalStrength = 1f;

            public void ToJson(PlanetAsset planet, JsonTextWriter writer)
            {
                writer.WriteStartObject();
                if (Size != 1f)
                    writer.WriteProperty("size", Size);
                if (TextureTile)
                    writer.WriteProperty("textureTile", planet.GetResourcePath(TextureTile));
                if (SmoothnessTile)
                    writer.WriteProperty("smoothnessTile", planet.GetResourcePath(SmoothnessTile));
                if (NormalTile)
                    writer.WriteProperty("normalTile", planet.GetResourcePath(NormalTile));
                if (NormalStrength != 1f)
                    writer.WriteProperty("normalStrength", NormalStrength);
                writer.WriteEndObject();
            }

            public IEnumerable<AssetResource> GetResources(PlanetAsset planet)
            {
                if (TextureTile)
                    yield return new ImageResource(TextureTile, planet);
                if (SmoothnessTile)
                    yield return new ImageResource(SmoothnessTile, planet);
                if (NormalTile)
                    yield return new ImageResource(NormalTile, planet);
            }
        }
    }
}
