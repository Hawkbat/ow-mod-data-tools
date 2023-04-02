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
    public class WarpReceiverComputerPropData : GeneralPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {

        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(WarpReceiverComputerPropAsset))]
    public class WarpReceiverComputerPropAsset : GeneralPropAsset<WarpReceiverComputerPropData> { }
    public class WarpReceiverComputerPropComponent : GeneralPropComponent<WarpReceiverComputerPropData> { }
}
