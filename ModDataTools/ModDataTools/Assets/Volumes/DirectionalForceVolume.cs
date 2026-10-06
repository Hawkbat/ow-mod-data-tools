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
    public class DirectionalForceVolumeData : GeneralForceVolumeData
    {
        [Tooltip("The direction of the force applied by this volume. Defaults to up (0, 1, 0).")]
        public NullishVector3 Normal;
        [Tooltip("Whether this force volume affects alignment.")]
        public bool AffectsAlignment = true;
        [Tooltip("Whether the force applied by this volume takes the centripetal force of the volume's parent body into account.")]
        public bool OffsetCentripetalForce;
        [Tooltip("Whether to play the gravity crystal audio when the player is in this volume.")]
        public bool PlayGravityCrystalAudio;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("normal", Normal);
            if (!AffectsAlignment)
                writer.WriteProperty("affectsAlignment", AffectsAlignment);
            if (OffsetCentripetalForce)
                writer.WriteProperty("offsetCentripetalForce", OffsetCentripetalForce);
            if (PlayGravityCrystalAudio)
                writer.WriteProperty("playGravityCrystalAudio", PlayGravityCrystalAudio);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(DirectionalForceVolumeAsset))]
    public class DirectionalForceVolumeAsset : GeneralPriorityVolumeAsset<DirectionalForceVolumeData> { }
    public class DirectionalForceVolumeComponent : GeneralPriorityVolumeComponent<DirectionalForceVolumeData> { }
}
