using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace ModDataTools.Assets
{
    public class TranslatorTextBlockAsset : DataAsset, IXmlSerializable
    {
        [Tooltip("The translator text this text block belongs to")]
        [ReadOnlyField]
        public TranslatorTextAsset TranslatorText;
        [Header("Data")]
        [Tooltip("The parent of this text block")]
        public TranslatorTextBlockAsset Parent;
        [Tooltip("Whether this text block belongs to location 'A' or 'B' (for remote text walls)")]
        public TranslatorTextAsset.Location Location;
        [Tooltip("The text to show for this option")]
        public string Text;
        [Tooltip("Nomai wall text arc")]
        public ArcInfo Arc;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (TranslatorText) yield return TranslatorText;
        }

        public override string GetIDPrefix() => string.Empty;

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("TextBlock");
            writer.WriteElementString("ID", FullID);
            if (Parent)
                writer.WriteElementString("Parent", Parent.FullID);
            if (Location == TranslatorTextAsset.Location.A)
                writer.WriteEmptyElement("LocationA");
            else if (Location == TranslatorTextAsset.Location.B)
                writer.WriteEmptyElement("LocationB");
            writer.WriteElementString("Text", Text);
            writer.WriteEndElement();
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (Parent && Parent.TranslatorText != TranslatorText)
                validator.Error(this, $"Parent block does not belong to the same translator text");
        }

        [Serializable]
        public class ArcInfo : IJsonSerializable
        {
            [Tooltip("Whether to skip modifying this spiral's placement, and instead keep the automatically determined placement.")]
            public bool AutoPlacement = true;
            [Tooltip("Whether to flip the spiral from left-curling to right-curling or vice versa.")]
            [ConditionalField(nameof(AutoPlacement), false)]
            public bool Mirror;
            [Tooltip("The local position of this object on the wall.")]
            [ConditionalField(nameof(AutoPlacement), false)]
            public Vector2 Position;
            [Tooltip("The z euler angle for this arc.")]
            [ConditionalField(nameof(AutoPlacement), false)]
            [Range(0f, 360f)]
            public float ZRotation;
            [Tooltip("The type of text to display")]
            public ArcType Type;

            public void ToJson(JsonTextWriter writer)
            {
                writer.WriteStartObject();
                if (!AutoPlacement)
                {
                    writer.WriteProperty("mirror", Mirror);
                    writer.WriteProperty("position", Position);
                    writer.WriteProperty("zRotation", ZRotation);
                }
                if (Type != ArcType.Adult)
                    writer.WriteProperty("type", Type);
                writer.WriteEndObject();
            }

            public enum ArcType
            {
                Adult = 0,
                Child = 1,
                Stranger = 2,
            }
        }
    }
}
