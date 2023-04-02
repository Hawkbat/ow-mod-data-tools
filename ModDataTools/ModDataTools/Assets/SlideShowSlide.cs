using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    public class SlideShowSlideAsset : DataAsset, IJsonSerializable
    {
        [Tooltip("The slideshow this slide belongs to")]
        [ReadOnlyField]
        public SlideShowAsset SlideShow;
        [Tooltip("The image file for this slide.")]
        public Texture2D Image;
        [Tooltip("The AudioClip that will continuously play while watching these slides")]
        public AudioClip BackdropAudio;
        [Tooltip("The AudioType that will continuously play while watching these slides, if not using a custom audio clip")]
        [ConditionalField(nameof(BackdropAudio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType BackdropAudioType;
        [Tooltip("The time to fade into the backdrop audio")]
        public float BackdropFadeTime;
        [Tooltip("The AudioClip for a one-shot sound when opening the slide.")]
        public AudioClip BeatAudio;
        [Tooltip("The AudioType for a one-shot sound when opening the slide, if not using a custom audio clip")]
        [ConditionalField(nameof(BeatAudio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType BeatAudioType;
        [Tooltip("The time delay until the one-shot audio")]
        public float BeatDelay;
        [Tooltip("Ambient light intensity when viewing this slide.")]
        public float AmbientLightIntensity;
        [Tooltip("Ambient light colour when viewing this slide.")]
        [ConditionalField(nameof(AmbientLightIntensity))]
        public Color AmbientLightColor;
        [Tooltip("Ambient light range when viewing this slide.")]
        [ConditionalField(nameof(AmbientLightIntensity))]
        public float AmbientLightRange;
        [Tooltip("Spotlight intensity modifier when viewing this slide.")]
        public float SpotIntensityMod;
        [Tooltip("Before viewing this slide, there will be a black frame for this many seconds.")]
        public float BlackFrameDuration;
        [Tooltip("Play-time duration for auto-projector slides.")]
        public float PlayTimeDuration;
        [Tooltip("Ship log fact revealed when viewing this slide")]
        public FactAsset RevealFact;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (SlideShow) yield return SlideShow;
        }

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            if (AmbientLightIntensity > 0f)
            {
                writer.WriteProperty("ambientLightColor", (Color32)AmbientLightColor);
                writer.WriteProperty("ambientLightIntensity", AmbientLightIntensity);
                writer.WriteProperty("ambientLightRange", AmbientLightRange);
                writer.WriteProperty("spotIntensityMod", SpotIntensityMod);
            }
            if (BackdropAudio)
                writer.WriteProperty("backdropAudio", $"slides/{SlideShow.Planet.StarSystem.FullName}/{SlideShow.Planet.FullName}/{AssetRepository.GetAssetFileName(BackdropAudio)}");
            else if (BackdropAudioType != AudioType.None)
                writer.WriteProperty("backdropAudio", BackdropAudioType, false);
            if (BackdropFadeTime != 0f && (BackdropAudio || BackdropAudioType != AudioType.None))
                writer.WriteProperty("backdropFadeTime", BackdropFadeTime);
            if (BeatAudio)
                writer.WriteProperty("beatAudio", $"slides/{SlideShow.Planet.StarSystem.FullName}/{SlideShow.Planet.FullName}/{AssetRepository.GetAssetFileName(BeatAudio)}");
            else if (BeatAudioType != AudioType.None)
                writer.WriteProperty("beatAudio", BeatAudioType, false);
            if (BeatDelay != 0f && (BeatAudio || BeatAudioType != AudioType.None))
                writer.WriteProperty("beatDelay", BeatDelay);
            if (BlackFrameDuration != 0f)
                writer.WriteProperty("blackFrameDuration", BlackFrameDuration);
            if (PlayTimeDuration != 0f)
                writer.WriteProperty("playTimeDuration", PlayTimeDuration);
            writer.WriteProperty("imagePath", $"slides/{SlideShow.Planet.StarSystem.FullName}/{SlideShow.Planet.FullName}/{AssetRepository.GetAssetFileName(Image)}");
            if (RevealFact)
                writer.WriteProperty("reveal", RevealFact.FullID);
            writer.WriteEndObject();
        }

        public override IEnumerable<AssetResource> GetResources()
        {
            if (Image)
                yield return new ImageResource(Image, $"slides/{SlideShow.Planet.StarSystem.FullName}/{SlideShow.Planet.FullName}/{AssetRepository.GetAssetFileName(Image)}");
            if (BackdropAudio)
                yield return new AudioResource(BackdropAudio, $"slides/{SlideShow.Planet.StarSystem.FullName}/{SlideShow.Planet.FullName}/{AssetRepository.GetAssetFileName(BackdropAudio)}");
            if (BeatAudio)
                yield return new AudioResource(BeatAudio, $"slides/{SlideShow.Planet.StarSystem.FullName}/{SlideShow.Planet.FullName}/{AssetRepository.GetAssetFileName(BeatAudio)}");
        }
    }
}
