using ModDataTools.Assets.PlanetModules;
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
    public class GravityVolumeData : GeneralForceVolumeData
    {
        [Tooltip("The upper bounds of the volume's \"surface\". Above this radius, the force applied by this volume will have falloff applied.")]
        public float UpperRadius;
        [Tooltip("The lower bounds of the volume's \"surface\". Above this radius and below the upper radius, the force applied by this volume will be constant.")]
        public float LowerRadius;
        [Tooltip("The volume's force will decrease linearly from the force to the min force as distance decreases from the lower radius to the min radius.")]
        public float MinRadius;
        [Tooltip("The minimum force applied by this volume between the lower radius and the min radius.")]
        public float MinForce;
        [Tooltip("How the force falls off with distance. Most planets use linear but the sun and some moons use inverseSquared.")]
        public BaseModule.GravityFallOffType FallOff = BaseModule.GravityFallOffType.Linear;
        [Tooltip("The radius where objects will be aligned to the volume's force. Defaults to 1.5x the upper radius. Set to 0 to disable alignment.")]
        public NullishSingle AlignmentRadius;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("upperRadius", UpperRadius);
            if (LowerRadius != 0f)
                writer.WriteProperty("lowerRadius", LowerRadius);
            if (MinRadius != 0f)
                writer.WriteProperty("minRadius", MinRadius);
            if (MinForce != 0f)
                writer.WriteProperty("minForce", MinForce);
            if (FallOff != BaseModule.GravityFallOffType.Linear)
                writer.WriteProperty("fallOff", FallOff);
            writer.WriteProperty("alignmentRadius", AlignmentRadius);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(GravityVolumeAsset))]
    public class GravityVolumeAsset : GeneralPriorityVolumeAsset<GravityVolumeData> { }
    public class GravityVolumeComponent : GeneralPriorityVolumeComponent<GravityVolumeData> { }
}
