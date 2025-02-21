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
    public class DestructionVolumeData : GeneralVolumeData
    {
        [Tooltip("Whether the bodies will shrink when they enter this volume or just disappear instantly.")]
        public bool ShrinkBodies = true;
        [Tooltip("Whether this volume only affects the player, ship, probe/scout, model rocket ship, and nomai shuttle.")]
        public bool OnlyAffectsPlayerRelatedBodies;
        [Tooltip("The type of death the player will have if they enter this volume.")]
        public DeathType DeathType = DeathType.Default;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (!ShrinkBodies)
                writer.WriteProperty("shrinkBodies", ShrinkBodies);
            if (OnlyAffectsPlayerRelatedBodies)
                writer.WriteProperty("onlyAffectsPlayerRelatedBodies", OnlyAffectsPlayerRelatedBodies);
            if (DeathType != DeathType.Default)
                writer.WriteProperty("deathType", DeathType);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(DestructionVolumeAsset))]
    public class DestructionVolumeAsset : GeneralVolumeAsset<DestructionVolumeData> { }
    public class DestructionVolumeComponent : GeneralVolumeComponent<DestructionVolumeData> { }
}
