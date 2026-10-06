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
    public class AudioVolumeData : GeneralPriorityVolumeData
    {
        [Tooltip("The audio to use")]
        public AudioConfig Audio;
        [Tooltip("Which sound clip to pick, if multiple are available for this AudioType")]
        [ConditionalField(nameof(Audio) + "." + nameof(AudioConfig.Clip), (AudioClip)null)]
        public ClipSelectionType ClipSelection;
        [Tooltip("The audio track of this audio volume")]
        public OuterWildsMixerTrackName Track = OuterWildsMixerTrackName.Environment;
        [Tooltip("The loudness of the audio")]
        [Range(0f, 1f)]
        public float Volume = 1f;
        [Tooltip("Whether to loop this audio while in this audio volume or just play it once")]
        public bool Loop = true;
        [Tooltip("How long it will take to fade this sound in and out when entering/exiting this volume.")]
        public float FadeSeconds = 2f;
        [Tooltip("Play the sound instantly without any fading.")]
        public bool NoFadeFromBeginning;
        [Tooltip("Randomize what time the audio starts at.")]
        public bool RandomizePlayhead;
        [Tooltip("Pause the music when exiting the volume.")]
        public bool PauseOnFadeOut;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("audio", Audio, context.Planet.GetResourcePath);
            if (ClipSelection != ClipSelectionType.Random)
                writer.WriteProperty("clipSelection", ClipSelection);
            if (Track != OuterWildsMixerTrackName.Environment)
                writer.WriteProperty("track", Track);
            if (!Loop)
                writer.WriteProperty("loop", Loop);
            if (Volume != 1f)
                writer.WriteProperty("volume", Volume);
            if (FadeSeconds != 2f)
                writer.WriteProperty("fadeSeconds", FadeSeconds);
            if (NoFadeFromBeginning)
                writer.WriteProperty("noFadeFromBeginning", NoFadeFromBeginning);
            if (RandomizePlayhead)
                writer.WriteProperty("randomizePlayhead", RandomizePlayhead);
            if (PauseOnFadeOut)
                writer.WriteProperty("pauseOnFadeOut", PauseOnFadeOut);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            foreach (var resource in Audio.GetResources(context.Planet.GetResourcePath))
                yield return resource;
        }

        public enum ClipSelectionType
        {
            Random = 0,
            Sequential = 1,
            Manual = 2,
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(AudioVolumeAsset))]
    public class AudioVolumeAsset : GeneralPriorityVolumeAsset<AudioVolumeData> { }
    public class AudioVolumeComponent : GeneralPriorityVolumeComponent<AudioVolumeData> { }
}
