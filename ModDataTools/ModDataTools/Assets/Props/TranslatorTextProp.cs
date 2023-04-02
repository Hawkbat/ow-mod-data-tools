using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class TranslatorTextPropData : GeneralPropData
    {
        [Tooltip("The translator text asset to use")]
        public TranslatorTextAsset TranslatorText;
        [Tooltip("The type of object this is")]
        public TranslatorTextAsset.TextType Type;
        [Tooltip("The location of this object.")]
        public TranslatorTextAsset.Location Location;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("arcInfo", TranslatorText.TextBlocks.Select(b => b.Arc));
            writer.WriteProperty("seed", TranslatorText.Seed);
            writer.WriteProperty("type", Type);
            if (Location != TranslatorTextAsset.Location.Unspecified)
                writer.WriteProperty("location", Location);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(TranslatorTextPropAsset))]
    public class TranslatorTextPropAsset : GeneralPropAsset<TranslatorTextPropData>
    {
        [Tooltip("Only for wall text. Aligns wall text to face towards the given direction, with 'up' oriented relative to its current rotation or alignment.")]
        public Vector3 Normal;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("normal", Normal);
        }
    }
    public class TranslatorTextPropComponent : GeneralPropComponent<TranslatorTextPropData>
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("normal", transform.forward);
        }
    }
}
