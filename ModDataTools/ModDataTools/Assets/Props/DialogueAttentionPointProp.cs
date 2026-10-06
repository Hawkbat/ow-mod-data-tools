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
    public class DialogueAttentionPointPropData : GeneralPointPropData
    {
        [Tooltip("An additional offset to apply when the camera looks at this attention point.")]
        public Vector3 Offset;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Offset != Vector3.zero)
                writer.WriteProperty("offset", Offset);
        }
    }

    [Serializable]
    public class DialogueSwappedAttentionPointPropData : DialogueAttentionPointPropData
    {
        [Tooltip("The dialogue node to activate this attention point for. If not set, activates for every node.")]
        public DialogueNodeAsset DialogueNode;
        [Tooltip("The index of the page in the current dialogue node to activate this attention point for, if the node has multiple pages.")]
        public int DialoguePage;
        [Tooltip("The easing factor which determines how 'snappy' the camera is when looking at the attention point.")]
        public float LookEasing = 1f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (DialogueNode)
                writer.WriteProperty("dialogueNode", DialogueNode.FullID);
            if (DialoguePage != 0)
                writer.WriteProperty("dialoguePage", DialoguePage);
            if (LookEasing != 1f)
                writer.WriteProperty("lookEasing", LookEasing);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(DialogueAttentionPointPropAsset))]
    public class DialogueAttentionPointPropAsset : GeneralPointPropAsset<DialogueAttentionPointPropData> { }
    public class DialogueAttentionPointPropComponent : GeneralPointPropComponent<DialogueAttentionPointPropData> { }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(DialogueSwappedAttentionPointPropAsset))]
    public class DialogueSwappedAttentionPointPropAsset : GeneralPointPropAsset<DialogueSwappedAttentionPointPropData> { }
    public class DialogueSwappedAttentionPointPropComponent : GeneralPointPropComponent<DialogueSwappedAttentionPointPropData> { }
}
