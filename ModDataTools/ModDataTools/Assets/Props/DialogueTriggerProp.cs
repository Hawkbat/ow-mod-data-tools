using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class DialogueTriggerPropData : GeneralPointPropData
    {
        [Tooltip("The radius of the remote trigger volume.")]
        public float Radius;
        [Tooltip("If setting up a remote trigger volume, this conditions must be met for it to trigger. Note: This is a dialogue condition, not a persistent condition.")]
        public ConditionAsset PrereqCondition;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("radius", Radius);
            if (PrereqCondition)
                writer.WriteProperty("prereqCondition", PrereqCondition.FullID);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(DialogueTriggerPropAsset))]
    public class DialogueTriggerPropAsset : GeneralPointPropAsset<DialogueTriggerPropData> { }

    public class DialogueTriggerComponent : GeneralPointPropComponent<DialogueTriggerPropData> { }
}
