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
    public class DirtEffectVolumeData : GeneralPriorityVolumeData
    {
        [Tooltip("The rate at which the dirt effect will accumulate")]
        public float DirtAccumulationRate = 1f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (DirtAccumulationRate != 1f)
                writer.WriteProperty("dirtAccumulationRate", DirtAccumulationRate);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(DirtEffectVolumeAsset))]
    public class DirtEffectVolumeAsset : GeneralPriorityVolumeAsset<DirtEffectVolumeData> { }
    public class DirtEffectVolumeComponent : GeneralPriorityVolumeComponent<DirtEffectVolumeData> { }
}
