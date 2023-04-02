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
    public class SolarSystemVolumeData : GeneralVolumeData
    {
        [Tooltip("The star system that entering this volume will send you to. Defaults to the Hearthian solar system.")]
        public StarSystemAsset TargetStarSystem;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (TargetStarSystem)
                writer.WriteProperty("targetStarSystem", TargetStarSystem.FullID);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(SolarSystemVolumeAsset))]
    public class SolarSystemVolumeAsset : GeneralVolumeAsset<SolarSystemVolumeData> { }
    public class SolarSystemVolumeComponent : GeneralVolumeComponent<SolarSystemVolumeData> { }
}
