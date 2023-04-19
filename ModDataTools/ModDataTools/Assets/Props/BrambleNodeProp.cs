using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class BrambleNodePropData : GeneralPropData
    {
        [Tooltip("Set this to true to make this node a seed instead of a node the player can enter.")]
        public bool IsSeed;
        [Tooltip("The planet that hosts the dimension this node links to.")]
        public PlanetAsset LinkedPlanet;
        [Tooltip("The color of the fog inside the node. The default value is (255, 245, 217, 255).")]
        public NullishColor FogTint;
        [Tooltip("The color of the light from the node. Alpha controls brightness. The default value is solid white.")]
        public NullishColor LightTint;
        [Tooltip("Should this node have a point of light from afar? Typically, nodes will have a foglight, while seeds won't, and neither will if not in a dimension.")]
        public bool HasFogLight;
        [Tooltip("The allowed exits from this node. Use Unity Explorer to check the SphericalFogWarpExits to determine which ones to disable.")]
        [BitField(6)]
        public int PossibleExits = 0b111111;
        [Tooltip("If your game hard crashes upon entering bramble, it's most likely because you have indirectly recursive dimensions, i.e. one leads to another that leads back to the first one. Set this to true for one of the nodes in the recursion to fix this, at the cost of it no longer showing markers for the scout, ship, etc.")]
        public bool PreventRecursionCrash;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("isSeed", IsSeed);
            writer.WriteProperty("fogTint", FogTint);
            writer.WriteProperty("lightTint", LightTint);
            writer.WriteProperty("hasFogLight", HasFogLight);
            if (!BitFieldUtility.GetValues(PossibleExits, 6).All(v => v))
            {
                writer.WritePropertyName("allowedEntrances");
                writer.WriteStartArray();
                for (int i = 0; i < 6; i++)
                    if (BitFieldUtility.GetValue(PossibleExits, i))
                        writer.WriteValue(i);
                writer.WriteEndArray();
            }
            if (PreventRecursionCrash)
                writer.WriteProperty("preventRecursionCrash", PreventRecursionCrash);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(BrambleNodePropAsset))]
    public class BrambleNodePropAsset : GeneralPropAsset<BrambleNodePropData> {
        [Tooltip("The physical scale of the node, as a multiplier of the original size. Nodes are 150m across, seeds are 10m across.")]
        public float Scale = 1f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("scale", Scale);
            writer.WriteProperty("name", FullID);
            base.WriteJsonProps(context, writer);
        }
    }
    public class BrambleNodePropComponent : GeneralPropComponent<BrambleNodePropData> {

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("scale", transform.localScale.Average());
            writer.WriteProperty("name", PropID);
            base.WriteJsonProps(context, writer);
        }
    }
}
