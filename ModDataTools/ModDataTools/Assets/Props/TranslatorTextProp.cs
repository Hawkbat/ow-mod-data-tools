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
        [Tooltip("Turns this computer off when this dialogue condition is set, and back on when it is unset, or the other way around if the computer starts off.")]
        [ConditionalField(nameof(Type), TranslatorTextAsset.TextType.Computer, TranslatorTextAsset.TextType.PreCrashComputer)]
        public ConditionAsset ComputerCondition;
        [Tooltip("Makes this computer turned off by default so the player cannot read the text.")]
        [ConditionalField(nameof(Type), TranslatorTextAsset.TextType.Computer, TranslatorTextAsset.TextType.PreCrashComputer)]
        public bool ComputerStartsOff;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (TranslatorText)
            {
                writer.WriteProperty("xmlFile", TranslatorText.GetXmlOutputPath());
                writer.WriteProperty("arcInfo", TranslatorText.TextBlocks);
                writer.WriteProperty("seed", TranslatorText.Seed);
            }
            writer.WriteProperty("type", Type);
            if (Location != TranslatorTextAsset.Location.Unspecified)
                writer.WriteProperty("location", Location);
            if (Type == TranslatorTextAsset.TextType.Computer || Type == TranslatorTextAsset.TextType.PreCrashComputer)
            {
                if (ComputerCondition)
                    writer.WriteProperty("computerCondition", ComputerCondition.FullID);
                if (ComputerStartsOff)
                    writer.WriteProperty("computerStartsOff", ComputerStartsOff);
            }
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            if (ComputerCondition && ComputerCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(ComputerCondition)} must not be a persistent condition");
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
            if (Normal != Vector3.zero)
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
