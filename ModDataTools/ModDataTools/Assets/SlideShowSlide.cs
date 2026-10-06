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
        [Tooltip("The audio that will continuously play while watching these slides")]
        public AudioConfig BackdropAudio;
        [Tooltip("The time to fade into the backdrop audio")]
        public float BackdropFadeTime;
        [Tooltip("The audio for a one-shot sound when opening the slide.")]
        public AudioConfig BeatAudio;
        [Tooltip("The time delay until the one-shot audio")]
        public float BeatDelay;
        [Tooltip("Ambient light intensity when viewing this slide.")]
        public float AmbientLightIntensity;
        [Tooltip("Ambient light colour when viewing this slide.")]
        [ConditionalField(nameof(AmbientLightIntensity))]
        public Color AmbientLightColor;
        [Tooltip("Ambient light range when viewing this slide.")]
        [ConditionalField(nameof(AmbientLightIntensity))]
        public float AmbientLightRange = 20f;
        [Tooltip("Spotlight intensity modifier when viewing this slide.")]
        public float SpotIntensityMod;
        [Tooltip("Before viewing this slide, there will be a black frame for this many seconds.")]
        public float BlackFrameDuration;
        [Tooltip("Play-time duration for auto-projector slides.")]
        public float PlayTimeDuration;
        [Tooltip("Ship log fact revealed when viewing this slide")]
        public FactAsset RevealFact;
        [Tooltip("Exclusive to slide reels. Whether this slide should rotate the reel item while inside a projector.")]
        public bool Rotate = true;

        public string GetResourcePath(UnityEngine.Object resource) => $"slides/{SlideShow.Planet.StarSystem.FullID}/{SlideShow.Planet.FullID}/{AssetRepository.GetAssetFileName(resource)}";

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
                if (AmbientLightRange != 20f)
                    writer.WriteProperty("ambientLightRange", AmbientLightRange);
            }
            if (SpotIntensityMod != 0f)
                writer.WriteProperty("spotIntensityMod", SpotIntensityMod);
            writer.WriteProperty("backdropAudio", BackdropAudio, GetResourcePath);
            if (BackdropFadeTime != 0f && BackdropAudio.HasValue)
                writer.WriteProperty("backdropFadeTime", BackdropFadeTime);
            writer.WriteProperty("beatAudio", BeatAudio, GetResourcePath);
            if (BeatDelay != 0f && BeatAudio.HasValue)
                writer.WriteProperty("beatDelay", BeatDelay);
            if (BlackFrameDuration != 0f)
                writer.WriteProperty("blackFrameDuration", BlackFrameDuration);
            if (PlayTimeDuration != 0f)
                writer.WriteProperty("playTimeDuration", PlayTimeDuration);
            writer.WriteProperty("imagePath", GetResourcePath(Image));
            if (RevealFact)
                writer.WriteProperty("reveal", RevealFact.FullID);
            if (!Rotate)
                writer.WriteProperty("rotate", Rotate);
            writer.WriteEndObject();
        }

        public override IEnumerable<AssetResource> GetResources()
        {
            if (Image)
                yield return new ImageResource(Image, GetResourcePath(Image));
            foreach (var resource in BackdropAudio.GetResources(GetResourcePath))
                yield return resource;
            foreach (var resource in BeatAudio.GetResources(GetResourcePath))
                yield return resource;
        }
    }
}
