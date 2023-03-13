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
    public class TranslatorTextBlock : DataAsset, IXmlAsset
    {
        [Tooltip("The translator text this text block belongs to")]
        [ReadOnlyField]
        public TranslatorText TranslatorText;
        [Header("Data")]
        [Tooltip("The parent of this text block")]
        public TranslatorTextBlock Parent;
        [Tooltip("Whether this text block belongs to location 'A' or 'B' (for remote text walls)")]
        public TranslatorText.Location Location;
        [Tooltip("The text to show for this option")]
        public string Text;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (TranslatorText) yield return TranslatorText;
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("TextBlock");
            writer.WriteElementString("ID", GetFullID());
            if (Parent)
                writer.WriteElementString("Parent", Parent.GetFullID());
            if (Location == TranslatorText.Location.A)
                writer.WriteEmptyElement("LocationA");
            else if (Location == TranslatorText.Location.B)
                writer.WriteEmptyElement("LocationB");
            writer.WriteElementString("Text", Text);
            writer.WriteEndElement();
        }
        public string ToXmlString() => ExportUtility.ToXmlString(this);

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (Parent && Parent.TranslatorText != TranslatorText)
                validator.Error(this, $"Parent block does not belong to the same translator text");
        }
    }
}
