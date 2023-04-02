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
    public class PlayerSpawnPropData : GeneralPropData
    {
        [Tooltip("Whether this planet's spawn point is the one the player will initially spawn at, if multiple spawn points exist.")]
        public bool IsDefault;
        [Tooltip("If you spawn on a planet with no oxygen, you probably want to set this to true ;;)")]
        public bool StartWithSuit;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (IsDefault)
                writer.WriteProperty("isDefault", IsDefault);
            if (StartWithSuit)
                writer.WriteProperty("startWithSuit", StartWithSuit);
        }
    }
    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(PlayerSpawnPropAsset))]
    public class PlayerSpawnPropAsset : GeneralPropAsset<PlayerSpawnPropData> { }
    public class PlayerSpawnPropComponent : GeneralPropComponent<PlayerSpawnPropData> { }
}
