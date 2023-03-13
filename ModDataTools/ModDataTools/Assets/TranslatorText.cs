using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ModDataTools.Utilities;
using UnityEngine;

namespace ModDataTools.Assets
{
    [CreateAssetMenu]
    public class TranslatorText : DataAsset, IValidateableAsset, IXmlAsset
    {
        [Tooltip("The planet this text is associated with")]
        public Planet Planet;
        [Tooltip("How to present this text object for editing; e.g. Branching for wall text, Linear for recorders")]
        public TextEditType EditType;
        [Header("Export")]
        [Tooltip("Whether to export a text .xml file")]
        public bool ExportXmlFile = true;
        [Tooltip("The text .xml file to use as-is instead of generating one")]
        public TextAsset OverrideXmlFile;
        [Header("Data")]
        [Tooltip("Facts to reveal after translating text")]
        public List<RevealFact> RevealFacts;
        [Header("Children")]
        [Tooltip("The text blocks of this text object")]
        [HideInInspector]
        public List<TranslatorTextBlock> TextBlocks;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Planet) yield return Planet;
        }

        public override IEnumerable<DataAsset> GetNestedAssets() => TextBlocks;

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (TextBlocks != null && TextBlocks.Any())
            {
                foreach (TranslatorTextBlock block in TextBlocks)
                {
                    if (block.Parent && !TextBlocks.Any(b => b == block.Parent))
                        validator.Error(this, $"Block is targeting a non-existent parent block");
                    block.Validate(validator);
                }
            }
            if (RevealFacts != null && RevealFacts.Any())
            {
                foreach (RevealFact reveal in RevealFacts)
                {
                    if (reveal.TextBlocks == null || reveal.TextBlocks.Count == 0)
                        validator.Error(this, $"Translator text fact reveal has no linked text blocks");
                    else if (reveal.TextBlocks.Any(b => !b || !TextBlocks.Contains(b)))
                        validator.Error(this, $"Translator text fact reveal has invalid text block");
                    if (!reveal.Fact)
                        validator.Error(this, $"Translator text fact reveal is missing a fact");
                }
            }
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("NomaiObject");
            writer.WriteSchemaAttributes("https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/text_schema.xsd");
            foreach (TranslatorTextBlock textBlock in TextBlocks)
            {
                textBlock.ToXml(writer);
            }
            foreach (RevealFact reveal in RevealFacts)
            {
                writer.WriteStartElement("ShipLogConditions");
                if (reveal.Location == Location.A)
                    writer.WriteEmptyElement("LocationA");
                else if (reveal.Location == Location.B)
                    writer.WriteEmptyElement("LocationB");
                writer.WriteStartElement("RevealFact");
                writer.WriteElementString("FactID", reveal.Fact.GetFullID());
                writer.WriteElementString("Condition", string.Join(",", reveal.TextBlocks));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        public string ToXmlString() => ExportUtility.ToXmlString(this);

        public enum TextEditType
        {
            Unknown,
            Single,
            Linear,
            Branching,
        }

        public enum Location
        {
            Any,
            A,
            B,
        }

        [Serializable]
        public class RevealFact
        {
            [Tooltip("The location ('A' or 'B' for remote text walls) that the player must be at to reveal this fact")]
            public Location Location;
            [Tooltip("The text blocks that must all be read to reveal the fact")]
            public List<TranslatorTextBlock> TextBlocks;
            [Tooltip("The fact to reveal")]
            public FactBase Fact;
        }
    }
}
