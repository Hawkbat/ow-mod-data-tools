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
    public class PolarForceVolumeData : GeneralForceVolumeData
    {
        [Tooltip("Enables tangential mode. The force applied by this volume will be perpendicular to the normal and the direction to the other body.")]
        public bool Tangential;
        [Tooltip("The force applied by this volume will be perpendicular to this direction and the direction to the other body. Defaults to up (0, 1, 0).")]
        [ConditionalField(nameof(Tangential))]
        public NullishVector3 Normal;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Tangential)
            {
                writer.WriteProperty("tangential", Tangential);
                writer.WriteProperty("normal", Normal);
            }
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(PolarForceVolumeAsset))]
    public class PolarForceVolumeAsset : GeneralPriorityVolumeAsset<PolarForceVolumeData> { }
    public class PolarForceVolumeComponent : GeneralPriorityVolumeComponent<PolarForceVolumeData> { }
}
