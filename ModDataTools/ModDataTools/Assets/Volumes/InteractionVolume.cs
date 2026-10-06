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
    public abstract class GeneralInteractionVolumeData : GeneralVolumeData
    {
        [Tooltip("The range at which the volume can be interacted with.")]
        public float Range = 2f;
        [Tooltip("The max view angle (in degrees) the player can see the volume with to interact with it. This will effectively be a cone extending from the volume's center forwards (along the Z axis) based on the volume's rotation. If not specified, no view angle restriction will be applied.")]
        public NullishSingle MaxViewAngle;
        [Tooltip("Whether the volume can be interacted with while in the ship.")]
        public bool UsableInShip;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Range != 2f)
                writer.WriteProperty("range", Range);
            writer.WriteProperty("maxViewAngle", MaxViewAngle);
            if (UsableInShip)
                writer.WriteProperty("usableInShip", UsableInShip);
        }
    }

    [Serializable]
    public class InteractionVolumeData : GeneralInteractionVolumeData
    {
        [Tooltip("The prompt to display when the volume is interacted with.")]
        public string Prompt;
        [Tooltip("Whether the volume can be interacted with multiple times.")]
        public bool Reusable;
        [Tooltip("The dialogue condition or persistent condition to set when the volume is interacted with.")]
        public ConditionAsset Condition;
        [Tooltip("A sound to play when the volume is interacted with.")]
        public AudioClip Audio;
        [Tooltip("A sound to play when the volume is interacted with, if not using a custom audio clip.")]
        [ConditionalField(nameof(Audio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType AudioType;
        [Tooltip("The name of an animation trigger to set on the animator when the volume is interacted with.")]
        public string AnimationTrigger;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (!string.IsNullOrEmpty(Prompt))
                writer.WriteProperty("prompt", GetPromptKey(context));
            if (Reusable)
                writer.WriteProperty("reusable", Reusable);
            if (Condition)
            {
                writer.WriteProperty("condition", Condition.FullID);
                if (Condition.Persistent)
                    writer.WriteProperty("persistent", Condition.Persistent);
            }
            if (Audio)
                writer.WriteProperty("audio", context.Planet.GetResourcePath(Audio));
            else if (AudioType != AudioType.None)
                writer.WriteProperty("audio", AudioType, false);
            if (!string.IsNullOrEmpty(AnimationTrigger))
                writer.WriteProperty("animationTrigger", AnimationTrigger);
        }

        public string GetPromptKey(PropContext context) => $"{context.GetProp().PropID}_PROMPT";

        public override void Localize(PropContext context, Localization l10n)
        {
            base.Localize(context, l10n);
            if (!string.IsNullOrEmpty(Prompt))
                l10n.AddUI(GetPromptKey(context), Prompt);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            foreach (var resource in base.GetResources(context))
                yield return resource;
            if (Audio)
                yield return new AudioResource(Audio, context.Planet);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(InteractionVolumeAsset))]
    public class InteractionVolumeAsset : GeneralVolumeAsset<InteractionVolumeData>
    {
        [Tooltip("A path to an animator component where an animation will be triggered when the volume is interacted with.")]
        public string PathToAnimator;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (!string.IsNullOrEmpty(PathToAnimator))
                writer.WriteProperty("pathToAnimator", PathToAnimator);
        }
    }

    public class InteractionVolumeComponent : GeneralVolumeComponent<InteractionVolumeData>
    {
        [Tooltip("An animator where an animation will be triggered when the volume is interacted with.")]
        public Animator Animator;
        [Tooltip("A path to an animator component where an animation will be triggered when the volume is interacted with.")]
        [ConditionalField(nameof(Animator), (Animator)null)]
        public string PathToAnimator;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Animator)
                writer.WriteProperty("pathToAnimator", UnityUtility.ResolvePaths(context.DetailPath + "/" + UnityUtility.GetTransformPath(Animator.transform, true)));
            else if (!string.IsNullOrEmpty(PathToAnimator))
                writer.WriteProperty("pathToAnimator", PathToAnimator);
        }
    }
}
