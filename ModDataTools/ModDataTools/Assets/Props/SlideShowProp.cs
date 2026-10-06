using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class SlideShowPropData : GeneralPropData
    {
        [Tooltip("The slideshow to spawn.")]
        public SlideShowAsset SlideShow;
        [Tooltip("The type of object this is.")]
        public SlideShowAsset.SlideShowType Type;
        [Tooltip("Model/mesh of the reel. Each model has a different number of slides on it. Whole has 7 slides but a full ring like 8.")]
        [ConditionalField(nameof(Type), SlideShowAsset.SlideShowType.SlideReel)]
        public SlideReelType ReelModel = SlideReelType.SevenSlides;
        [Tooltip("Condition/material of the reel. Antique is the Stranger, Pristine is the Dreamworld, Rusted (exclusive to slide reels) is a burned reel.")]
        [ConditionalField(nameof(Type), SlideShowAsset.SlideShowType.SlideReel, SlideShowAsset.SlideShowType.StandingVisionTorch)]
        public SlideReelCondition ReelCondition = SlideReelCondition.Antique;
        [Tooltip("Exclusive to the slide reel type. Set which slides appear on the slide reel model. Leave empty to default to the first few slides. Takes a list of indices, i.e., to show the first 5 slides in reverse you would put [4, 3, 2, 1, 0]. Index starts at 0.")]
        public List<int> DisplaySlides = new();

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (!SlideShow) return;
            if (SlideShow.RevealFacts.Any())
                writer.WriteProperty("reveals", SlideShow.RevealFacts.Select(f => f.FullID));
            if (SlideShow.PlayWithFacts.Any())
                writer.WriteProperty("playWithShipLogFacts", SlideShow.PlayWithFacts.Select(f => f.FullID));
            if (SlideShow.ConditionsToSet.Any())
                writer.WriteProperty("conditionsToSet", SlideShow.ConditionsToSet.Select(c => c.FullID));
            if (SlideShow.PersistentConditionsToSet.Any())
                writer.WriteProperty("persistentConditionsToSet", SlideShow.PersistentConditionsToSet.Select(c => c.FullID));
            writer.WriteProperty("slides", SlideShow.Slides);
            writer.WriteProperty("type", Type);
            if (Type == SlideShowAsset.SlideShowType.SlideReel)
            {
                if (ReelModel != SlideReelType.SevenSlides)
                    writer.WriteProperty("reelModel", ReelModel);
                if (DisplaySlides.Any())
                    writer.WriteProperty("displaySlides", DisplaySlides);
            }
            if (ReelCondition != SlideReelCondition.Antique && (Type == SlideShowAsset.SlideShowType.SlideReel || Type == SlideShowAsset.SlideShowType.StandingVisionTorch && ReelCondition != SlideReelCondition.Rusted))
                writer.WriteProperty("reelCondition", ReelCondition);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            if (!SlideShow)
                validator.Error(context.Planet, $"Slide show prop '{context.GetProp().PropName}' has no slide show set");
            else if (DisplaySlides.Any(i => i < 0 || i >= SlideShow.Slides.Count))
                validator.Error(context.Planet, $"Slide show prop '{context.GetProp().PropName}' displays slide indices that are out of range");
            if (Type == SlideShowAsset.SlideShowType.StandingVisionTorch && ReelCondition == SlideReelCondition.Rusted)
                validator.Error(context.Planet, $"Slide show prop '{context.GetProp().PropName}' cannot use the {nameof(SlideReelCondition.Rusted)} condition on a standing vision torch");
        }

        public enum SlideReelType
        {
            SixSlides = 0,
            SevenSlides = 1,
            EightSlides = 2,
            Whole = 3,
        }

        public enum SlideReelCondition
        {
            Antique = 0,
            Pristine = 1,
            Rusted = 2,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(SlideShowPropAsset))]
    public class SlideShowPropAsset : GeneralPropAsset<SlideShowPropData> { }
    public class SlideShowPropComponent : GeneralPropComponent<SlideShowPropData> { }
}
