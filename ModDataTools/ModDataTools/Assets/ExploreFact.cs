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
    public class ExploreFactAsset : FactAsset
    {
        public override void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("ExploreFact");
            writer.WriteElementString("ID", FullID);
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
            if (!string.IsNullOrEmpty(AltText))
                l10n.AddShipLog($"{FullID}_ALT", AltText);
        }
    }
}
