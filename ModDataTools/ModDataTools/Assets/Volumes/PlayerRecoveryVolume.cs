using ModDataTools.Assets.Props;
using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Volumes
{
    [Serializable]
    public class PlayerRecoveryVolumeData : GeneralInteractionVolumeData
    {
        [Tooltip("Whether interacting with this volume refuels the player's jetpack.")]
        public bool Refuels = true;
        [Tooltip("Whether interacting with this volume heals the player.")]
        public bool Heals;
        [Tooltip("Whether interacting with this volume cleans dirt off the helmet visor.")]
        public bool CleansVisor;
        [Tooltip("Whether the fuel fire color would be green.")]
        public bool DlcFuel;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (!Refuels)
                writer.WriteProperty("refuels", Refuels);
            if (Heals)
                writer.WriteProperty("heals", Heals);
            if (CleansVisor)
                writer.WriteProperty("cleansVisor", CleansVisor);
            if (DlcFuel)
                writer.WriteProperty("dlcFuel", DlcFuel);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(PlayerRecoveryVolumeAsset))]
    public class PlayerRecoveryVolumeAsset : GeneralVolumeAsset<PlayerRecoveryVolumeData> { }
    public class PlayerRecoveryVolumeComponent : GeneralVolumeComponent<PlayerRecoveryVolumeData> { }
}
