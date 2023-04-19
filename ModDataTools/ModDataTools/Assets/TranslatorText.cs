using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using UnityEngine;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(TranslatorTextAsset))]
    public class TranslatorTextAsset : DataAsset, IValidateableAsset, IXmlSerializable
    {
        [Tooltip("The planet this text is associated with")]
        public PlanetAsset Planet;
        [Header("Export")]
        [Tooltip("Whether to export a text .xml file")]
        public bool ExportXmlFile = true;
        [Tooltip("The text .xml file to use as-is instead of generating one")]
        public TextAsset OverrideXmlFile;
        [Header("Data")]
        [Tooltip("Facts to reveal after translating text")]
        public List<RevealFact> RevealFacts = new();
        [Tooltip("The random seed used to pick what the text arcs will look like.")]
        public int Seed;
        [Header("Children")]
        [Tooltip("The text blocks of this text object")]
        [HideInInspector]
        public List<TranslatorTextBlockAsset> TextBlocks = new();

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Planet) yield return Planet;
        }

        public override IEnumerable<DataAsset> GetNestedAssets() => TextBlocks;

        public override string GetChildIDPrefix() => $"{FullID}_";

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (OverrideXmlFile) return;
            if (TextBlocks.Any())
            {
                foreach (TranslatorTextBlockAsset block in TextBlocks)
                {
                    if (block.Parent && !TextBlocks.Any(b => b == block.Parent))
                        validator.Error(this, $"Block is targeting a non-existent parent block");
                    block.Validate(validator);
                }
            }
            if (RevealFacts.Any())
            {
                foreach (RevealFact reveal in RevealFacts)
                {
                    if (!reveal.TextBlocks.Any())
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
            foreach (TranslatorTextBlockAsset textBlock in TextBlocks)
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
                writer.WriteElementString("FactID", reveal.Fact.FullID);
                writer.WriteElementString("Condition", string.Join(",", reveal.TextBlocks));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        public string GetXmlOutputPath() => $"text/{Planet.StarSystem.FullID}/{Planet.FullID}/{FullID}.xml";

        public override void Localize(Localization l10n)
        {
            foreach (var block in TextBlocks)
                block.Localize(l10n);
        }

        public override IEnumerable<AssetResource> GetResources()
        {
            if (ExportXmlFile)
            {
                if (OverrideXmlFile)
                    yield return new TextResource(OverrideXmlFile, GetXmlOutputPath());
                else
                    yield return new TextResource(ExportUtility.ToXmlString(this), GetXmlOutputPath());
            }
            foreach (var block in TextBlocks)
                foreach (var resource in block.GetResources())
                    yield return resource;
        }

        public enum TextType
        {
            Wall = 0,
            Scroll = 1,
            Computer = 2,
            Cairn = 3,
            Recorder = 4,
            PreCrashRecorder = 5,
            PreCrashComputer = 6,
            Trailmarker = 7,
            CairnVariant = 8,
            Whiteboard = 9,
        }

        public enum Location
        {
            Unspecified = 0,
            A = 1,
            B = 2,
        }

        [Serializable]
        public class RevealFact
        {
            [Tooltip("The location ('A' or 'B' for remote text walls) that the player must be at to reveal this fact")]
            public Location Location;
            [Tooltip("The text blocks that must all be read to reveal the fact")]
            public List<TranslatorTextBlockAsset> TextBlocks;
            [Tooltip("The fact to reveal")]
            public FactAsset Fact;
        }
    }
}
