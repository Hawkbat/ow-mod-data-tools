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
    public class LightSourceVolumeData : GeneralVolumeData
    {

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(LightSourceVolumeAsset))]
    public class LightSourceVolumeAsset : GeneralVolumeAsset<LightSourceVolumeData> { }
    public class LightSourceVolumeComponent : GeneralVolumeComponent<LightSourceVolumeData> { }
}
