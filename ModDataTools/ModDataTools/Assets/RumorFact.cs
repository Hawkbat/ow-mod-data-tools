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
    public class RumorFact : FactBase
    {
        [Tooltip("The source of this rumor, this draws a line in detective mode")]
        public EntryBase Source;
        [Tooltip("Displays on the card in detective mode if no ExploreFacts have been revealed on the parent entry")]
        public string RumorName;
        [Tooltip("Priority over other RumorFacts to appear as the entry card's title")]
        [ConditionalField(nameof(RumorName))]
        public int RumorNamePriority;

        public override void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("RumorFact");
            writer.WriteElementString("ID", GetFullID());
            writer.WriteElementString("SourceID", Source.GetFullID());
            if (!string.IsNullOrEmpty(RumorName))
            {
                writer.WriteElementString("RumorName", RumorName);
                if (RumorNamePriority != 0)
                    writer.WriteElementString("RumorNamePriority", RumorNamePriority.ToString());
            }
            if (IgnoreMoreToExplore)
                writer.WriteEmptyElement("IgnoreMoreToExplore");
            writer.WriteElementString("Text", Text);
            if (!string.IsNullOrEmpty(AltText) || AltTextCondition)
            {
                writer.WriteStartElement("AltText");
                writer.WriteElementString("Text", AltText);
                writer.WriteElementString("Condition", AltTextCondition.GetFullID());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!Source)
                validator.Error(this, $"Missing {nameof(Source)}");
        }
    }
}
