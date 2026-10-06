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
        [Tooltip("The audio to use")]
        public AudioConfig Audio;
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
        [Tooltip("At this distance the sound is at its loudest.")]
        public float MinDistance;
        [Tooltip("The sound will drop off by this distance. For signals, this only affects when it is heard aloud and not via the signalscope.")]
        public float MaxDistance = 30f;
        [Tooltip("How loud the sound will play")]
        public float Volume = 0.5f;
        [Tooltip("A ship log fact to reveal when the signal is identified.")]
        public FactAsset RevealFact;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("name", context.GetProp().PropID);
            writer.WriteProperty("audio", Audio, context.Planet.GetResourcePath);
            writer.WriteProperty("detectionRadius", DetectionRadius);
            if (Frequency)
                writer.WriteProperty("frequency", Frequency.FullID);
            else
                writer.WriteProperty("frequency", SignalFrequency, false);
            if (IdentificationRadius != 10f)
                writer.WriteProperty("identificationRadius", IdentificationRadius);
            if (InsideCloak)
                writer.WriteProperty("insideCloak", InsideCloak);
            if (!OnlyAudibleToScope)
                writer.WriteProperty("onlyAudibleToScope", OnlyAudibleToScope);
            if (RevealFact)
                writer.WriteProperty("reveals", RevealFact.FullID);
            if (SourceRadius != 1f)
                writer.WriteProperty("sourceRadius", SourceRadius);
            if (MinDistance != 0f)
                writer.WriteProperty("minDistance", MinDistance);
            if (MaxDistance != 30f)
                writer.WriteProperty("maxDistance", MaxDistance);
            if (Volume != 0.5f)
                writer.WriteProperty("volume", Volume);
        }

        public override void Localize(PropContext context, Localization l10n)
        {
            l10n.AddUI(context.GetProp().PropID, context.GetProp().PropName);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            foreach (var resource in Audio.GetResources(context.Planet.GetResourcePath))
                yield return resource;
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(SignalPropAsset))]
    public class SignalPropAsset : GeneralPointPropAsset<SignalPropData> { }
    public class SignalPropComponent : GeneralPointPropComponent<SignalPropData> { }
}
