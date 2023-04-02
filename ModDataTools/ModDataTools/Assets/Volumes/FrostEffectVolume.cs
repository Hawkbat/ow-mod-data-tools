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
    public class FrostEffectVolumeData : GeneralPriorityVolumeData
    {
        [Tooltip("The rate at which the frost effect will get stronger")]
        public float FrostRate = 0.5f;
        [Tooltip("The maximum strength of frost this volume can give")]
        [Range(0f, 1f)]
        public float MaxFrost = 0.91f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (FrostRate != 0.5f)
                writer.WriteProperty("frostRate", FrostRate);
            if (MaxFrost != 0.91f)
                writer.WriteProperty("maxFrost", MaxFrost);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(FrostEffectVolumeAsset))]
    public class FrostEffectVolumeAsset : GeneralPriorityVolumeAsset<FrostEffectVolumeData> { }
    public class FrostEffectVolumeComponent : GeneralPriorityVolumeComponent<FrostEffectVolumeData> { }
}
