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
    public class ShuttlePropData : GeneralPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("id", context.GetProp().PropID);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(ShuttlePropAsset))]
    public class ShuttlePropAsset : GeneralPropAsset<ShuttlePropData> { }
    public class ShuttlePropComponent : GeneralPropComponent<ShuttlePropData> { }
}
