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
    public class MapRestrictionVolumeData : GeneralVolumeData
    {

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(MapRestrictionVolumeAsset))]
    public class MapRestrictionVolumeAsset : GeneralVolumeAsset<MapRestrictionVolumeData> { }
    public class MapRestrictionVolumeComponent : GeneralVolumeComponent<MapRestrictionVolumeData> { }
}
