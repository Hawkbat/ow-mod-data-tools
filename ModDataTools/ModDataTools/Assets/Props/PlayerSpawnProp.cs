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
    public class PlayerSpawnPropData : SpawnPropData
    {
        [Tooltip("If you spawn on a planet with no oxygen, you probably want to set this to true ;;)")]
        public bool StartWithSuit;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (StartWithSuit)
                writer.WriteProperty("startWithSuit", StartWithSuit);
        }
    }
    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(PlayerSpawnPropAsset))]
    public class PlayerSpawnPropAsset : SpawnPropAsset<PlayerSpawnPropData> {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer) { }
    }
    public class PlayerSpawnPropComponent : SpawnPropComponent<PlayerSpawnPropData> {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer) { }
    }
}
