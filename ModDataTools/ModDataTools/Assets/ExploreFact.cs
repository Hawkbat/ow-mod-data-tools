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
            writer.WriteElementString("Text", Text);
            if (!string.IsNullOrEmpty(AltText) || AltTextCondition)
            {
                writer.WriteStartElement("AltText");
                writer.WriteElementString("Text", AltText);
                writer.WriteElementString("Condition", AltTextCondition.FullID);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
