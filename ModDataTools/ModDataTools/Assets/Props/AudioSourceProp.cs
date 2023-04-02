using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ModDataTools.Assets.Volumes.AudioVolumeData;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class AudioSourcePropData : GeneralPointPropData
    {
        [Tooltip("The AudioClip to use")]
        public AudioClip Audio;
        [Tooltip("The AudioType to use, if not using a custom audio clip")]
        [ConditionalField(nameof(Audio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType AudioType;
        [Tooltip("At this distance the sound is at its loudest.")]
        public float MinDistance = 0f;
        [Tooltip("The sound will drop off by this distance.")]
        public float MaxDistance = 5f;
        [Tooltip("How loud the sound will play")]
        [Range(0f, 1f)]
        public float Volume = 0.5f;
        [Tooltip("The audio track of this audio volume")]
        public OuterWildsMixerTrackName Track = OuterWildsMixerTrackName.Environment;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Audio)
                writer.WriteProperty("audio", context.Planet.GetResourcePath(Audio));
            else if (AudioType != AudioType.None)
                writer.WriteProperty("audio", AudioType, false);
            if (Track != OuterWildsMixerTrackName.Environment)
                writer.WriteProperty("track", Track);
            if (Volume != 0.5f)
                writer.WriteProperty("volume", Volume);
            if (MinDistance != 0f)
                writer.WriteProperty("minDistance", MinDistance);
            if (MinDistance != 5f)
                writer.WriteProperty("maxDistance", MaxDistance);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            if (Audio)
                yield return new AudioResource(Audio, context.Planet);
        }
    }
    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(AudioSourcePropAsset))]
    public class AudioSourcePropAsset : GeneralPointPropAsset<AudioSourcePropData> { }
    public class AudioSourcePropComponent : GeneralPointPropComponent<AudioSourcePropData> { }
}
