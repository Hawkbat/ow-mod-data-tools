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
    public class ExploreFact : FactBase
    {
        public override void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("ExploreFact");
            writer.WriteElementString("ID", GetFullID());
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
    }
}
