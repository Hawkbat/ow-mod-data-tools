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
        [Tooltip("The audio to use during the day")]
        public AudioConfig DayAudio;
        [Tooltip("The audio to use during the night")]
        public AudioConfig NightAudio;
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
            writer.WriteProperty("dayAudio", DayAudio, context.Planet.GetResourcePath);
            writer.WriteProperty("nightAudio", NightAudio, context.Planet.GetResourcePath);
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
            foreach (var resource in DayAudio.GetResources(context.Planet.GetResourcePath))
                yield return resource;
            foreach (var resource in NightAudio.GetResources(context.Planet.GetResourcePath))
                yield return resource;
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(DayNightAudioVolumeAsset))]
    public class DayNightAudioVolumeAsset : GeneralPriorityVolumeAsset<DayNightAudioVolumeData> { }
    public class DayNightAudioVolumeComponent : GeneralPriorityVolumeComponent<DayNightAudioVolumeData> { }
}
