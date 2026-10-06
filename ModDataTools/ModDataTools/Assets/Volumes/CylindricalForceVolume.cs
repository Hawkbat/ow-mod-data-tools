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
    public class CylindricalForceVolumeData : GeneralForceVolumeData
    {
        [Tooltip("The direction that the force applied by this volume will be perpendicular to. Defaults to up (0, 1, 0).")]
        public NullishVector3 Normal;
        [Tooltip("Whether to play the gravity crystal audio when the player is in this volume.")]
        public bool PlayGravityCrystalAudio;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("normal", Normal);
            if (PlayGravityCrystalAudio)
                writer.WriteProperty("playGravityCrystalAudio", PlayGravityCrystalAudio);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(CylindricalForceVolumeAsset))]
    public class CylindricalForceVolumeAsset : GeneralPriorityVolumeAsset<CylindricalForceVolumeData> { }
    public class CylindricalForceVolumeComponent : GeneralPriorityVolumeComponent<CylindricalForceVolumeData> { }
}
