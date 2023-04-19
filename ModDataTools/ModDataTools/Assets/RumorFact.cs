using ModDataTools.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace ModDataTools.Assets
{
    public class RumorFactAsset : FactAsset
    {
        [Tooltip("The source of this rumor, this draws a line in detective mode")]
        public EntryAsset Source;
        [Tooltip("Displays on the card in detective mode if no ExploreFacts have been revealed on the parent entry")]
        public string RumorName;
        [Tooltip("Priority over other RumorFacts to appear as the entry card's title")]
        [ConditionalField(nameof(RumorName))]
        public int RumorNamePriority;

        public override void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("RumorFact");
            writer.WriteElementString("ID", FullID);
            writer.WriteElementString("SourceID", Source.FullID);
            if (!string.IsNullOrEmpty(RumorName))
            {
                writer.WriteElementString("RumorName", $"{FullID}_NAME");
                if (RumorNamePriority != 0)
                    writer.WriteElementString("RumorNamePriority", RumorNamePriority.ToString());
            }
            if (IgnoreMoreToExplore)
                writer.WriteEmptyElement("IgnoreMoreToExplore");
            writer.WriteElementString("Text", FullID);
            if (!string.IsNullOrEmpty(AltText) || AltTextCondition)
            {
                writer.WriteStartElement("AltText");
                writer.WriteElementString("Text", $"{FullID}_ALT");
                writer.WriteElementString("Condition", AltTextCondition.FullID);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public override void Localize(Localization l10n)
        {
            l10n.AddShipLog(FullID, Text);
            if (!string.IsNullOrEmpty(RumorName))
                l10n.AddShipLog($"{FullID}_NAME", RumorName);
            if (!string.IsNullOrEmpty(AltText))
                l10n.AddShipLog($"{FullID}_ALT", AltText);
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!Source)
                validator.Error(this, $"Missing {nameof(Source)}");
        }
    }
}
