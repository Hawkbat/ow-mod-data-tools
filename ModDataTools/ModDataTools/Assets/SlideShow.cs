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
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(SlideShowAsset))]
    public class SlideShowAsset : DataAsset
    {
        [Tooltip("The planet this slideshow is associated with")]
        public PlanetAsset Planet;
        [Header("Data")]
        [Tooltip("The ship log facts revealed after finishing this slide reel.")]
        public List<FactAsset> RevealFacts = new();
        [Tooltip("Play this slide reel in the ship log menu for these facts.")]
        public List<FactAsset> PlayWithFacts = new();
        [Header("Children")]
        [Tooltip("The list of slides")]
        [HideInInspector]
        public List<SlideShowSlideAsset> Slides = new();

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Planet) yield return Planet;
        }

        public override IEnumerable<DataAsset> GetNestedAssets() => Slides;

        public enum SlideShowType
        {
            SlideReel = 0,
            AutoProjector = 1,
            VisionTorchTarget = 2,
            StandingVisionTorch = 3,
        }
    }
}
