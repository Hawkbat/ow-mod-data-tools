using ModDataTools.Assets.Resources;
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
    public class SignalPropData : GeneralPointPropData
    {
        [Tooltip("The AudioClip to use")]
        public AudioClip Audio;
        [Tooltip("The AudioType to use, if not using a custom audio clip")]
        [ConditionalField(nameof(Audio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType AudioType;
        [Tooltip("The custom frequency of the signal.")]
        public FrequencyAsset Frequency;
        [Tooltip("The frequency of the signal, if not using a custom value.")]
        [ConditionalField(nameof(Frequency), (FrequencyAsset)null)]
        public SignalFrequency SignalFrequency;
        [Tooltip("How close the player must get to the signal to detect it. This is when you get the \"Unknown Signal Detected\" notification.")]
        public float DetectionRadius;
        [Tooltip("How close the player must get to the signal to identify it. This is when you learn its name.")]
        public float IdentificationRadius = 10f;
        [Tooltip("Radius of the sphere giving off the signal.")]
        public float SourceRadius = 1f;
        [Tooltip("Only set to true if you are putting this signal inside a cloaking field.")]
        public bool InsideCloak;
        [Tooltip("Set to false if the player can hear the signal without equipping the signal-scope.")]
        public bool OnlyAudibleToScope = true;
        [Tooltip("A ship log fact to reveal when the signal is identified.")]
        public FactAsset RevealFact;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Audio)
                writer.WriteProperty("audio", context.Planet.GetResourcePath(Audio));
            else if (AudioType != AudioType.None)
                writer.WriteProperty("audio", AudioType, false);
            writer.WriteProperty("detectionRadius", DetectionRadius);
            if (Frequency)
                writer.WriteProperty("frequency", Frequency.FullName);
            else
                writer.WriteProperty("frequency", SignalFrequency, false);
            if (IdentificationRadius != 10f)
                writer.WriteProperty("identificationRadius", IdentificationRadius);
            if (InsideCloak)
                writer.WriteProperty("insideCloak", InsideCloak);
            if (RevealFact)
                writer.WriteProperty("reveals", RevealFact.FullID);
            if (SourceRadius != 1f)
                writer.WriteProperty("sourceRadius", SourceRadius);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            if (Audio)
                yield return new AudioResource(Audio, context.Planet);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(SignalPropAsset))]
    public class SignalPropAsset : GeneralPointPropAsset<SignalPropData>
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("name", FullName);
            base.WriteJsonProps(context, writer);
        }
    }
    public class SignalPropComponent : GeneralPointPropComponent<SignalPropData>
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("name", gameObject.name);
            base.WriteJsonProps(context, writer);
        }
    }
}
