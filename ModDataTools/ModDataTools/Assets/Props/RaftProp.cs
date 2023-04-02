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
    public class RaftPropData : GeneralPointPropData
    {
        [Tooltip("Acceleration of the raft. Default acceleration is 5.")]
        public float Acceleration = 5f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("acceleration", Acceleration);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(RaftPropAsset))]
    public class RaftPropAsset : GeneralPointPropAsset<RaftPropData> { }
    public class RaftPropComponent : GeneralPointPropComponent<RaftPropData> { }
}
