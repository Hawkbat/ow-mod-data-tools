using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class DreamCandlePropData : GeneralPropData
    {
        [Tooltip("The type of dream candle this is.")]
        public DreamCandleType Type = DreamCandleType.Ground;
        [Tooltip("Whether the candle should start lit or extinguished.")]
        public bool StartLit;
        [Tooltip("A condition to set when the candle is lit.")]
        public DreamLightConditionConfig Condition;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Type != DreamCandleType.Ground)
                writer.WriteProperty("type", Type);
            if (StartLit)
                writer.WriteProperty("startLit", StartLit);
            Condition.WriteJsonProperty(writer, "condition");
        }

        public enum DreamCandleType
        {
            Ground = 0,
            GroundSmall = 1,
            GroundLarge = 2,
            GroundSingle = 3,
            Wall = 4,
            WallLargeFlame = 5,
            WallBigWick = 6,
            Standing = 7,
            Pile = 8,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(DreamCandlePropAsset))]
    public class DreamCandlePropAsset : GeneralPropAsset<DreamCandlePropData> { }
    public class DreamCandlePropComponent : GeneralPropComponent<DreamCandlePropData> { }
}
