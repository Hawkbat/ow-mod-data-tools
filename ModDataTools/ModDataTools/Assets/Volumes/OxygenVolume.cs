using ModDataTools.Assets.Props;
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
    public class OxygenVolumeData : GeneralVolumeData
    {
        [Tooltip("Does this volume contain trees? This will change the notification from \"Oxygen tank refilled\" to \"Trees detected, oxygen tank refilled\".")]
        public bool TreeVolume;
        [Tooltip("Whether to play the oxygen tank refill sound or just fill quietly.")]
        public bool PlayRefillAudio = true;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (TreeVolume)
                writer.WriteProperty("treeVolume", TreeVolume);
            if (!PlayRefillAudio)
                writer.WriteProperty("playRefillAudio", PlayRefillAudio);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(OxygenVolumeAsset))]
    public class OxygenVolumeAsset : GeneralVolumeAsset<OxygenVolumeData> { }
    public class OxygenVolumeComponent : GeneralVolumeComponent<OxygenVolumeData> { }
}
