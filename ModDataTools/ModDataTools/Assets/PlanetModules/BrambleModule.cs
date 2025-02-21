using ModDataTools.Assets.Props;
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
    public class BrambleModule : PlanetModule
    {
        [Tooltip("The dimension the vines will be copied from. Only a handful are available due to batched colliders.")]
        public VinePrefabType VinePrefab = VinePrefabType.Hub;
        [Tooltip("The internal radius (in meters) of the dimension. The default is 750 for the Hub, Escape Pod, and Angler Nest dimensions, and 500 for the others.")]
        public NullishSingle Radius;
        [Tooltip("The color of the fog inside this dimension. The default is (84, 83, 73).")]
        public NullishColor FogTint;
        [Tooltip("The density of the fog inside this dimension. The default is 6.")]
        public NullishSingle FogDensity;
        [Tooltip("The node that the player is taken to when exiting this dimension.")]
        public BrambleNodePropAsset LinkedNode;
        [Tooltip("The allowed entrances into this dimension. Use Unity Explorer to check the SphericalFogWarpExits to determine which ones to disable.")]
        [BitField(6)]
        public int AllowedEntrances = 0b111111;

        public enum VinePrefabType
        {
            None = 0,
            Hub = 1,
            Cluster = 2,
            SmalllNest = 3,
            ExitOnly = 4,
        }

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (IsEnabled)
            {
                writer.WritePropertyName("dimension");
                writer.WriteStartObject();
                writer.WriteProperty("fogTint", FogTint);
                writer.WriteProperty("fogDensity", FogDensity);
                if (LinkedNode)
                    writer.WriteProperty("linksTo", LinkedNode.FullID);
                writer.WriteProperty("radius", Radius);
                if (VinePrefab != VinePrefabType.Hub)
                    writer.WriteProperty("vinePrefab", VinePrefab);
                if (!BitFieldUtility.GetValues(AllowedEntrances, 6).All(v => v))
                {
                    writer.WritePropertyName("allowedEntrances");
                    writer.WriteStartArray();
                    for (int i = 0; i < 6; i++)
                        if (BitFieldUtility.GetValue(AllowedEntrances, i))
                            writer.WriteValue(i);
                    writer.WriteEndArray();
                }
                writer.WriteEndObject();
            }
            var nodes = AssetRepository.GetProps<BrambleNodePropData>(planet);
            if (nodes.Any())
                writer.WriteProperty("nodes", nodes);
        }

        public override void Validate(PlanetAsset planet, IAssetValidator validator)
        {
            foreach (var node in AssetRepository.GetProps<BrambleNodePropData>(planet))
                node.Data.Validate(node, planet, validator);
        }

        public override bool ShouldWrite(PlanetAsset planet)
            => base.ShouldWrite(planet) || AssetRepository.GetProps<BrambleNodePropData>(planet).Any();
    }
}
