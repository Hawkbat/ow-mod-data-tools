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
    public class RaftDockPropData : GeneralPointPropData
    {

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {

        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(RaftDockPropAsset))]
    public class RaftDockPropAsset : GeneralPointPropAsset<RaftDockPropData> { }
    public class RaftDockPropComponent : GeneralPointPropComponent<RaftDockPropData> { }
}
