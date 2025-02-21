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
    public class DayNightAudioVolumeData : GeneralPriorityVolumeData
    {
        [Tooltip("The AudioClip to use during the day")]
        public AudioClip DayAudio;
        [Tooltip("The AudioType to use during the day, if not using a custom audio clip")]
        [ConditionalField(nameof(DayAudio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType DayAudioType;
        [Tooltip("The AudioClip to use during the night")]
        public AudioClip NightAudio;
        [Tooltip("The AudioType to use during the night, if not using a custom audio clip")]
        [ConditionalField(nameof(NightAudio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType NightAudioType;
        [Tooltip("The astro object used to determine if it is day or night.")]
        public PlanetAsset Sun;
        [Tooltip("Angle in degrees defining daytime. Inside this window it will be day and outside it will be night.")]
        [Range(0f, 360f)]
        public float DayWindow = 180f;
        [Tooltip("The loudness of the audio")]
        [Range(0f, 1f)]
        public float Volume = 1f;
        [Tooltip("The audio track of this audio volume. Most of the time you'll use environment (the default) for sound effects and music for music.")]
        public OuterWildsMixerTrackName Track = OuterWildsMixerTrackName.Environment;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (DayAudio)
                writer.WriteProperty("dayAudio", context.Planet.GetResourcePath(DayAudio));
            else if (DayAudioType != AudioType.None)
                writer.WriteProperty("dayAudio", DayAudioType, false);
            if (NightAudio)
                writer.WriteProperty("nightAudio", context.Planet.GetResourcePath(NightAudio));
            else if (NightAudioType != AudioType.None)
                writer.WriteProperty("nightAudio", NightAudioType, false);
            if (Sun)
                writer.WriteProperty("sun", Sun.FullID);
            if (DayWindow != 180f)
                writer.WriteProperty("dayWindow", DayWindow);
            if (Volume != 1f)
                writer.WriteProperty("volume", Volume);
            if (Track != OuterWildsMixerTrackName.Environment)
                writer.WriteProperty("track", Track);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            if (DayAudio)
                yield return new AudioResource(DayAudio, context.Planet);
            if (NightAudio)
                yield return new AudioResource(NightAudio, context.Planet);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(DayNightAudioVolumeAsset))]
    public class DayNightAudioVolumeAsset : GeneralPriorityVolumeAsset<DayNightAudioVolumeData> { }
    public class DayNightAudioVolumeComponent : GeneralPriorityVolumeComponent<DayNightAudioVolumeData> { }
}
