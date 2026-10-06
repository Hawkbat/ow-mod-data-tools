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
        public float Radius = 1f;
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
            if (Dialogue)
                writer.WriteProperty("xmlFile", Dialogue.GetXmlOutputPath());
            writer.WriteProperty("radius", Radius);
            writer.WriteProperty("range", Range);
            writer.WriteProperty("lookAtRadius", LookAtRadius);
            if (BlockAfterPersistentCondition)
                writer.WriteProperty("blockAfterPersistentCondition", BlockAfterPersistentCondition.FullID);
            if (FlashlightToggle != FlashlightToggleType.None)
                writer.WriteProperty("flashlightToggle", FlashlightToggle);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            if (!Dialogue)
                validator.Error(context.Planet, $"Dialogue prop '{context.GetProp().PropName}' has no dialogue set");
        }

        public void ValidateSwappedAttentionPoints(PropContext context, IAssetValidator validator, IEnumerable<DialogueSwappedAttentionPointPropData> points)
        {
            foreach (var point in points)
                if (point.DialogueNode && point.DialogueNode.Dialogue != Dialogue)
                    validator.Error(context.Planet, $"Swapped attention point on '{context.GetProp().PropName}' targets node '{point.DialogueNode.FullID}', which is not part of its dialogue");
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
        [Tooltip("A remote trigger volume that starts this dialogue when entered.")]
        public DialogueTriggerPropAsset RemoteTrigger;
        [Tooltip("If this is on an existing character, the path to the object with the character's animation controller. Lets the character look at the player and play animations, and positions the dialogue relative to the speaker.")]
        public string PathToAnimController;
        [Tooltip("If this dialogue is adding to existing character dialogue, the path to the game object with the dialogue on it.")]
        public string PathToExistingDialogue;
        [Tooltip("The point that the camera looks at when dialogue advances.")]
        public DialogueAttentionPointPropAsset AttentionPoint;
        [Tooltip("Additional points that the camera looks at when dialogue advances through specific dialogue nodes and pages.")]
        public List<DialogueSwappedAttentionPointPropAsset> SwappedAttentionPoints = new();

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (!string.IsNullOrEmpty(PathToAnimController))
                writer.WriteProperty("pathToAnimController", PathToAnimController);
            if (!string.IsNullOrEmpty(PathToExistingDialogue))
                writer.WriteProperty("pathToExistingDialogue", PathToExistingDialogue);
            if (RemoteTrigger)
                writer.WriteProperty("remoteTrigger", context.MakeSibling(RemoteTrigger));
            if (AttentionPoint)
                writer.WriteProperty("attentionPoint", context.MakeSibling(AttentionPoint));
            if (SwappedAttentionPoints.Any(p => p))
                writer.WriteProperty("swappedAttentionPoints", SwappedAttentionPoints.Where(p => p).Select(p => context.MakeSibling(p)));
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            Data.ValidateSwappedAttentionPoints(new PropContext<DialoguePropData> { Planet = Planet, Prop = this }, validator, SwappedAttentionPoints.Where(p => p).Select(p => p.Data));
        }
    }

    public class DialoguePropComponent : GeneralPointPropComponent<DialoguePropData>
    {
        [Tooltip("A remote trigger volume that starts this dialogue when entered.")]
        public DialogueTriggerPropAsset RemoteTriggerAsset;
        [Tooltip("A remote trigger volume that starts this dialogue when entered.")]
        public DialogueTriggerComponent RemoteTrigger;
        [Tooltip("The object with the character's animation controller. Lets the character look at the player and play animations, and positions the dialogue relative to the speaker.")]
        public Transform AnimController;
        [Tooltip("If this is on an existing character, the path to the object with the character's animation controller.")]
        [ConditionalField(nameof(AnimController), (Transform)null)]
        public string PathToAnimController;
        [Tooltip("If this dialogue is adding to existing character dialogue, the game object with the dialogue on it.")]
        public Transform ExistingDialogue;
        [Tooltip("If this dialogue is adding to existing character dialogue, the path to the game object with the dialogue on it.")]
        [ConditionalField(nameof(ExistingDialogue), (Transform)null)]
        public string PathToExistingDialogue;
        [Tooltip("The point that the camera looks at when dialogue advances.")]
        public DialogueAttentionPointPropAsset AttentionPointAsset;
        [Tooltip("The point that the camera looks at when dialogue advances.")]
        public DialogueAttentionPointPropComponent AttentionPoint;
        [Tooltip("Additional points that the camera looks at when dialogue advances through specific dialogue nodes and pages.")]
        public List<DialogueSwappedAttentionPointPropAsset> SwappedAttentionPointAssets = new();
        [Tooltip("Additional points that the camera looks at when dialogue advances through specific dialogue nodes and pages.")]
        public List<DialogueSwappedAttentionPointPropComponent> SwappedAttentionPoints = new();

        IEnumerable<PropContext<DialogueSwappedAttentionPointPropData>> GetSwappedAttentionPoints(PropContext context)
            => SwappedAttentionPointAssets.Where(p => p).Select(p => context.MakeSibling(p))
                .Concat(SwappedAttentionPoints.Where(p => p).Select(p => context.MakeSibling(p)));

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            ((DialoguePropData)GetData()).ValidateSwappedAttentionPoints(context, validator, GetSwappedAttentionPoints(context).Select(p => p.Data));
        }

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (AnimController)
                writer.WriteProperty("pathToAnimController", UnityUtility.ResolvePaths(context.DetailPath + "/" + UnityUtility.GetTransformPath(AnimController, true)));
            else if (!string.IsNullOrEmpty(PathToAnimController))
                writer.WriteProperty("pathToAnimController", PathToAnimController);
            if (ExistingDialogue)
                writer.WriteProperty("pathToExistingDialogue", UnityUtility.ResolvePaths(context.DetailPath + "/" + UnityUtility.GetTransformPath(ExistingDialogue, true)));
            else if (!string.IsNullOrEmpty(PathToExistingDialogue))
                writer.WriteProperty("pathToExistingDialogue", PathToExistingDialogue);
            if (RemoteTriggerAsset)
                writer.WriteProperty("remoteTrigger", context.MakeSibling(RemoteTriggerAsset));
            else if (RemoteTrigger)
                writer.WriteProperty("remoteTrigger", context.MakeSibling(RemoteTrigger));
            if (AttentionPointAsset)
                writer.WriteProperty("attentionPoint", context.MakeSibling(AttentionPointAsset));
            else if (AttentionPoint)
                writer.WriteProperty("attentionPoint", context.MakeSibling(AttentionPoint));
            var swappedAttentionPoints = GetSwappedAttentionPoints(context);
            if (swappedAttentionPoints.Any())
                writer.WriteProperty("swappedAttentionPoints", swappedAttentionPoints);
        }
    }
}
