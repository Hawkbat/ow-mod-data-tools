using ModDataTools.Assets.Resources;
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
    public class EyeTravelerPropData : DetailPropData
    {
        [Tooltip("If set, the player must know this ship log fact for this traveler (and their instrument zones and quantum instruments) to appear. The fact does not need to exist in the current star system; the player's save data will be checked directly.")]
        public FactAsset RequiredFact;
        [Tooltip("If set, the player must have this persistent dialogue condition set for this traveler (and their instrument zones and quantum instruments) to appear.")]
        public ConditionAsset RequiredPersistentCondition;
        [Tooltip("The dialogue condition that will trigger the traveler to start playing their instrument. Must be unique for each traveler.")]
        public ConditionAsset StartPlayingCondition;
        [Tooltip("If specified, this dialogue condition must be set for the traveler to participate in the campfire song. Otherwise, the song will be able to start without them.")]
        public ConditionAsset ParticipatingCondition;
        [Tooltip("The audio signal to use for the traveler while playing around the campfire (and also for their paired quantum instrument if another is not specified). The audio clip should be 16 measures at 92 BPM (approximately 42 seconds long).")] 
        public SignalPropData Signal;
        [Tooltip("The audio to use for the traveler during the finale of the campfire song. It should be 8 measures of the main loop at 92 BPM followed by 2 measures of fade-out (approximately 26 seconds long in total). Can be a path to a .wav/.ogg/.mp3 file, or taken from the AudioClip list.")] 
        public AudioConfig FinaleAudio;
        [Tooltip("The dialogue to use for this traveler. If omitted, the first CharacterDialogueTree in the object will be used.")] 
        public DialoguePropData Dialogue;
        [Tooltip("The name of the base game traveler to position this traveler after at the campfire, starting clockwise from Riebeck. Defaults to the end of the list (right before Riebeck).")] 
        public TravelerName AfterTraveler = TravelerName.Auto;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (RequiredFact)
                writer.WriteProperty("requiredFact", RequiredFact.FullID);
            if (RequiredPersistentCondition)
                writer.WriteProperty("requiredPersistentCondition", RequiredPersistentCondition.FullID);
            if (StartPlayingCondition)
                writer.WriteProperty("startPlayingCondition", StartPlayingCondition.FullID);
            if (ParticipatingCondition)
                writer.WriteProperty("participatingCondition", ParticipatingCondition.FullID);
            writer.WritePropertyName("signal");
            writer.WriteStartObject();
            Signal.WriteJsonProps(context, writer);
            writer.WriteEndObject();
            writer.WriteProperty("finaleAudio", FinaleAudio, context.Planet.GetResourcePath);
            writer.WritePropertyName("dialogue");
            writer.WriteStartObject();
            Dialogue.WriteJsonProps(context, writer);
            writer.WriteEndObject();
            if (AfterTraveler != TravelerName.Auto)
                writer.WriteProperty("afterTraveler", AfterTraveler, false);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            Signal.Validate(context, validator);
            Dialogue.Validate(context, validator);
            if (RequiredPersistentCondition && !RequiredPersistentCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(RequiredPersistentCondition)} must be a persistent condition");
            if (!StartPlayingCondition)
                validator.Error(context.Planet, $"Missing {nameof(StartPlayingCondition)}");
            else if (StartPlayingCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(StartPlayingCondition)} must not be a persistent condition");
            if (ParticipatingCondition && ParticipatingCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(ParticipatingCondition)} must not be a persistent condition");
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            foreach (var resource in base.GetResources(context))
                yield return resource;
            foreach (var resource in Signal.GetResources(context))
                yield return resource;
            foreach (var resource in Dialogue.GetResources(context))
                yield return resource;
            foreach (var resource in FinaleAudio.GetResources(context.Planet.GetResourcePath))
                yield return resource;
        }

        public enum TravelerName
        {
            Riebeck,
            Chert,
            Esker,
            Felspar,
            Gabbro,
            Solanum,
            Prisoner,
            Auto,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(EyeTravelerPropAsset))]
    public class EyeTravelerPropAsset : DetailPropAsset<EyeTravelerPropData>
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("id", FullID);
        }
    }

    public class EyeTravelerPropComponent : DetailPropComponent<EyeTravelerPropData>
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("id", PropID);
        }
    }
}
