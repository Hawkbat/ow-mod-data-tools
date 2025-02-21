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
    public class ProcGenModule : PlanetModule
    {
        [Tooltip("Scale height of the proc gen.")]
        public float Scale;
        [Tooltip("Can pick a preset material with a texture from the base game. Does not work with color or any textures.")]
        public MaterialType Material;
        [Tooltip("Ground color, only applied if no texture or material is chosen.")]
        [ConditionalField(nameof(Material), Values = [MaterialType.Default])]
        public Color Color = Color.white;
        [Tooltip("Can use a custom texture. Does not work with material or color.")]
        [ConditionalField(nameof(Material), Values = [MaterialType.Default])]
        public Texture2D Texture;
        [Tooltip("The texture used for the terrain's smoothness and metallic, which are controlled by the texture's alpha and red channels respectively. Optional. Typically black with variable transparency, when metallic isn't wanted.")]
        [ConditionalField(nameof(Material), Values = [MaterialType.Default])]
        public Texture2D SmoothnessMap;
        [Tooltip("How 'glossy' the surface is, where 0 is diffuse, and 1 is like a mirror. Multiplies with the alpha of the smoothness map if using one.")]
        [Range(0f, 1f)]
        [ConditionalField(nameof(Material), Values = [MaterialType.Default])]
        public float Smoothness;
        [Tooltip("How metallic the surface is, from 0 to 1. Multiplies with the red of the smoothness map if using one.")]
        [Range(0f, 1f)]
        [ConditionalField(nameof(Material), Values = [MaterialType.Default])]
        public float Metallic;
        [Tooltip("The texture used for the terrain's normal (aka bump) map. Optional.")]
        [ConditionalField(nameof(Material), Values = [MaterialType.Default])]
        public Texture2D NormalMap;
        [Tooltip("Strength of the normal map. Usually 0-1, but can go above, or negative to invert the map.")]
        [ConditionalField(nameof(Material), Values = [MaterialType.Default])]
        public float NormalStrength = 1f;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (Scale != 0f)
                writer.WriteProperty("scale", Scale);
            if (Material == MaterialType.Default)
            {
                if (!Texture && Color != Color.white)
                    writer.WriteProperty("color", (Color32)Color);
                if (Texture)
                    writer.WriteProperty("texture", planet.GetResourcePath(Texture));
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
            }
            else
            {
                writer.WriteProperty("material", Material.ToString());
            }
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            if (Texture)
                yield return new ImageResource(Texture, planet);
            if (SmoothnessMap)
                yield return new ImageResource(SmoothnessMap, planet);
            if (NormalMap)
                yield return new ImageResource(NormalMap, planet);
        }

        public enum MaterialType
        {
            Default = 0,
            Ice = 1,
            Quantum = 2,
            Rock = 3,
        }
    }
}
