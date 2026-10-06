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
    public class ConditionTriggerVolumeData : GeneralVolumeData
    {
        [Tooltip("The dialogue condition or persistent condition to set when entering the volume.")]
        public ConditionAsset Condition;
        [Tooltip("Whether to unset the condition when exiting the volume.")]
        public bool Reversible;
        [Tooltip("Whether to set the condition when the player enters this volume.")]
        public bool Player = true;
        [Tooltip("Whether to set the condition when the scout probe enters this volume.")]
        public bool Probe;
        [Tooltip("Whether to set the condition when the ship enters this volume.")]
        public bool Ship;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Condition)
            {
                writer.WriteProperty("condition", Condition.FullID);
                if (Condition.Persistent)
                    writer.WriteProperty("persistent", Condition.Persistent);
            }
            if (Reversible)
                writer.WriteProperty("reversible", Reversible);
            if (!Player)
                writer.WriteProperty("player", Player);
            if (Probe)
                writer.WriteProperty("probe", Probe);
            if (Ship)
                writer.WriteProperty("ship", Ship);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            if (!Condition)
                validator.Error(context.Planet, $"Condition trigger volume '{context.GetProp().PropName}' has no condition set");
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(ConditionTriggerVolumeAsset))]
    public class ConditionTriggerVolumeAsset : GeneralVolumeAsset<ConditionTriggerVolumeData> { }
    public class ConditionTriggerVolumeComponent : GeneralVolumeComponent<ConditionTriggerVolumeData> { }
}
