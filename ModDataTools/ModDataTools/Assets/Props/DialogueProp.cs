using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class DialoguePropData : GeneralPointPropData
    {
        [Tooltip("The dialogue tree to use")]
        public DialogueAsset Dialogue;
        [Tooltip("Radius of the spherical collision volume where you get the \"talk to\" prompt when looking at. If you use a remoteTriggerPosition, you can set this to 0 to make the dialogue only trigger remotely.")]
        public float Radius;
        [Tooltip("Distance from radius the prompt appears")]
        public float Range = 2f;
        [Tooltip("If a pathToAnimController is supplied, if you are within this distance the character will look at you. If it is set to 0, they will only look at you when spoken to.")]
        public float LookAtRadius;
        [Tooltip("Prevents the dialogue from being created after a specific persistent condition is set. Useful for remote dialogue triggers that you want to have happen only once.")]
        public ConditionAsset BlockAfterPersistentCondition;
        [Tooltip("What type of flashlight toggle to do when dialogue is interacted with")]
        public FlashlightToggleType FlashlightToggle;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("xmlFile", Dialogue.GetXmlOutputPath());
            writer.WriteProperty("radius", Radius);
            writer.WriteProperty("range", Range);
            writer.WriteProperty("lookAtRadius", LookAtRadius);
            if (BlockAfterPersistentCondition)
                writer.WriteProperty("blockAfterPersistentCondition", BlockAfterPersistentCondition);
            if (FlashlightToggle != FlashlightToggleType.None)
                writer.WriteProperty("flashlightToggle", FlashlightToggle);
        }

        public enum FlashlightToggleType
        {
            None = 0,
            TurnOff = 1,
            TurnOffThenOn = 2,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(DialoguePropAsset))]
    public class DialoguePropAsset : GeneralPointPropAsset<DialoguePropData>
    {
        public DialogueTriggerPropAsset RemoteTrigger;
        public string PathToAnimController;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (!string.IsNullOrEmpty(PathToAnimController))
                writer.WriteProperty("pathToAnimController", PathToAnimController);
            if (RemoteTrigger)
                writer.WriteProperty("remoteTrigger", context.MakeSibling(RemoteTrigger));
        }
    }

    public class DialoguePropComponent : GeneralPointPropComponent<DialoguePropData>
    {
        public DialogueTriggerPropAsset RemoteTriggerAsset;
        public DialogueTriggerComponent RemoteTrigger;
        public Transform AnimController;
        [ConditionalField(nameof(AnimController), (Transform)null)]
        public string PathToAnimController;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (AnimController)
                writer.WriteProperty("pathToAnimController", context.DetailPath + "/" + UnityUtility.GetTransformPath(AnimController, true));
            else if (!string.IsNullOrEmpty(PathToAnimController))
                writer.WriteProperty("pathToAnimController", PathToAnimController);
            if (RemoteTriggerAsset)
                writer.WriteProperty("remoteTrigger", context.MakeSibling(RemoteTriggerAsset));
            else if (RemoteTrigger)
                writer.WriteProperty("remoteTrigger", context.MakeSibling(RemoteTrigger));
        }
    }
}
