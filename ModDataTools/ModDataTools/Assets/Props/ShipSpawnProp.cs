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
    public class ShipSpawnPropData : SpawnPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {

        }
    }
    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(ShipSpawnPropAsset))]
    public class ShipSpawnPropAsset : SpawnPropAsset<ShipSpawnPropData> { }
    public class ShipSpawnPropComponent : SpawnPropComponent<ShipSpawnPropData> { }
}
