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
    public class AlarmBellPropData : GeneralPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {

        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(AlarmBellPropAsset))]
    public class AlarmBellPropAsset : GeneralPropAsset<AlarmBellPropData> { }
    public class AlarmBellPropComponent : GeneralPropComponent<AlarmBellPropData> { }
}
