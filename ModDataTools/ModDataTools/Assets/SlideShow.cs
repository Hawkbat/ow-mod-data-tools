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
        [Tooltip("The ship log facts that make the reel play when they are displayed in the computer (by selecting entries or arrows). You should probably include facts from the revealed facts here. If you only specify a rumor fact, then it would only play in its ship log entry if this has revealed only rumor facts because an entry with revealed explore facts doesn't display rumor facts.")]
        public List<FactAsset> PlayWithFacts = new();
        [Tooltip("The dialogue conditions to set after finishing this slide reel.")]
        public List<ConditionAsset> ConditionsToSet = new();
        [Tooltip("The persistent conditions to set after finishing this slide reel.")]
        public List<ConditionAsset> PersistentConditionsToSet = new();
        [Header("Children")]
        [Tooltip("The list of slides")]
        [HideInInspector]
        public List<SlideShowSlideAsset> Slides = new();

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Planet) yield return Planet;
        }

        public override IEnumerable<DataAsset> GetNestedAssets() => Slides;

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            foreach (var condition in ConditionsToSet.Where(c => c && c.Persistent))
                validator.Error(this, $"Condition '{condition.FullID}' in {nameof(ConditionsToSet)} must not be persistent");
            foreach (var condition in PersistentConditionsToSet.Where(c => c && !c.Persistent))
                validator.Error(this, $"Condition '{condition.FullID}' in {nameof(PersistentConditionsToSet)} must be persistent");
        }

        public enum SlideShowType
        {
            SlideReel = 0,
            AutoProjector = 1,
            VisionTorchTarget = 2,
            StandingVisionTorch = 3,
        }
    }
}
