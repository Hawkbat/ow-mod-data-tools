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

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (SlideShow.RevealFacts.Any())
                writer.WriteProperty("reveals", SlideShow.RevealFacts.Select(f => f.FullID));
            if (SlideShow.PlayWithFacts.Any())
                writer.WriteProperty("playWithShipLogFacts", SlideShow.PlayWithFacts.Select(f => f.FullID));
            writer.WriteProperty("slides", SlideShow.Slides);
            writer.WriteProperty("type", Type);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(SlideShowPropAsset))]
    public class SlideShowPropAsset : GeneralPropAsset<SlideShowPropData> { }
    public class SlideShowPropComponent : GeneralPropComponent<SlideShowPropData> { }
}
