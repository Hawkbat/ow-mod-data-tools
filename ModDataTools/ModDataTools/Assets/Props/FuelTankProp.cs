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
    public class FuelTankPropData : GeneralScaleablePropData
    {
        [Tooltip("The type of fuel tank this is.")]
        public FuelTankType Type = FuelTankType.HearthianTank;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Type != FuelTankType.HearthianTank)
                writer.WriteProperty("type", Type);
        }

        public enum FuelTankType
        {
            HearthianTank = 0,
            NomaiTank = 1,
            PreCrashNomaiTank = 2,
            DlcTorch = 3,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(FuelTankPropAsset))]
    public class FuelTankPropAsset : GeneralScaleablePropAsset<FuelTankPropData> { }
    public class FuelTankPropComponent : GeneralScaleablePropComponent<FuelTankPropData> { }
}
