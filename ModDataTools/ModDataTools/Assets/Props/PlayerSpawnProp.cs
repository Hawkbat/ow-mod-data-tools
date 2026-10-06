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
        [Tooltip("Spawns you in the pilot seat of your ship. This will ignore the spawn point position. Be sure to provide a valid ship spawn point as well!")]
        public bool StartInShip;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (StartWithSuit)
                writer.WriteProperty("startWithSuit", StartWithSuit);
            if (StartInShip)
                writer.WriteProperty("startInShip", StartInShip);
        }
    }
    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(PlayerSpawnPropAsset))]
    public class PlayerSpawnPropAsset : SpawnPropAsset<PlayerSpawnPropData> { }
    public class PlayerSpawnPropComponent : SpawnPropComponent<PlayerSpawnPropData> { }
}
