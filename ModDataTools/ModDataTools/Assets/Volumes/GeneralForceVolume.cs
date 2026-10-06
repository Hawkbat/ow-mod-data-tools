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
    public abstract class GeneralForceVolumeData : GeneralPriorityVolumeData
    {
        [Tooltip("The force applied by this volume. Can be negative to reverse the direction.")]
        public float Force;
        [Tooltip("The priority of this force volume for the purposes of alignment. Volumes of higher priority will override volumes of lower priority. Volumes of the same priority will stack like normal. Ex: A player in a gravity volume with priority 0, and zero-gravity volume with priority 1, will feel zero gravity. Default value here is 1 instead of 0 so it automatically overrides planet gravity, which is 0 by default.")]
        public int AlignmentPriority = 1;
        [Tooltip("Whether this force volume is inheritable. The most recently activated inheritable force volume will stack with other force volumes even if their priorities differ.")]
        public bool Inheritable;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("force", Force);
            if (AlignmentPriority != 1)
                writer.WriteProperty("alignmentPriority", AlignmentPriority);
            if (Inheritable)
                writer.WriteProperty("inheritable", Inheritable);
        }
    }
}
