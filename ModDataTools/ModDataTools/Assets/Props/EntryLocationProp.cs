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
    public class EntryLocationPropData : GeneralPointPropData
    {
        [Tooltip("The entry this location relates to")]
        public EntryAsset Entry;
        [Tooltip("Whether this location is cloaked")]
        public bool Cloaked;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("id", Entry.FullID);
            if (Cloaked)
                writer.WriteProperty("cloaked", Cloaked);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(EntryLocationPropAsset))]
    public class EntryLocationPropAsset : GeneralPointPropAsset<EntryLocationPropData> { }
    public class EntryLocationPropComponent : GeneralPointPropComponent<EntryLocationPropData> { }
}
