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
    public class GeneralPriorityVolumeData : GeneralVolumeData
    {
        [Tooltip("The layer of this volume.")]
        public OuterWildsLayer Layer;
        [Tooltip("The priority for this volume's effects to be applied. Ex, a player in a gravity volume with priority 0, and zero-gravity volume with priority 1, will feel zero gravity.")]
        public int Priority = 1;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Layer != OuterWildsLayer.Default)
                writer.WriteProperty("layer", (int)Layer);
            if (Priority != 1)
                writer.WriteProperty("priority", Priority);
        }
    }

    public class GeneralPriorityVolumeAsset<T> : GeneralVolumeAsset<T> where T : GeneralPriorityVolumeData { }
    public class GeneralPriorityVolumeComponent<T> : GeneralVolumeComponent<T> where T : GeneralPriorityVolumeData { }
}
