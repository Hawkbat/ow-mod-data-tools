using ModDataTools.Utilities;
using ModDataTools.Assets.Resources;
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
    public class TranslatorTextBlockAsset : DataAsset, IXmlSerializable, IJsonSerializable
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

        public string XmlID => (TranslatorText.TextBlocks.IndexOf(this) + 1).ToString();

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (TranslatorText) yield return TranslatorText;
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("TextBlock");
            writer.WriteElementString("ID", XmlID);
            if (Parent)
                writer.WriteElementString("Parent", Parent.XmlID);
            if (Location == TranslatorTextAsset.Location.A)
                writer.WriteEmptyElement("LocationA");
            else if (Location == TranslatorTextAsset.Location.B)
                writer.WriteEmptyElement("LocationB");
            writer.WriteElementString("Text", FullID);
            writer.WriteEndElement();
        }

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            if (!Arc.AutoPlacement)
            {
                writer.WriteProperty("mirror", Arc.Mirror);
                writer.WriteProperty("position", Arc.Position);
                writer.WriteProperty("zRotation", Arc.ZRotation);
            }
            if (Arc.CustomTextImage)
                writer.WriteProperty("customTextImage", TranslatorText.Planet.GetResourcePath(Arc.CustomTextImage));
            else if (Arc.Type != ArcInfo.ArcType.Adult)
                writer.WriteProperty("type", Arc.Type);
            if (Arc.LegiblePersistentCondition)
                writer.WriteProperty("legiblePersistentCondition", Arc.LegiblePersistentCondition.FullID);
            if (!string.IsNullOrEmpty(Arc.CustomLanguageName))
                writer.WriteProperty("customLanguageName", GetCustomLanguageNameKey());
            writer.WriteProperty("overrideUnreadColor", Arc.OverrideUnreadColor);
            if (Arc.OverrideUnreadColor.HasValue)
                writer.WriteProperty("overrideTranslatedColor", Arc.OverrideTranslatedColor);
            writer.WriteEndObject();
        }

        public string GetCustomLanguageNameKey() => $"{FullID}_LANGUAGE";

        public override void Localize(Localization l10n)
        {
            l10n.AddDialogue(FullID, Text);
            if (!string.IsNullOrEmpty(Arc.CustomLanguageName))
                l10n.AddOther(GetCustomLanguageNameKey(), Arc.CustomLanguageName);
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (Parent && Parent.TranslatorText != TranslatorText)
                validator.Error(this, $"Parent block does not belong to the same translator text");
            if (Arc.LegiblePersistentCondition && !Arc.LegiblePersistentCondition.Persistent)
                validator.Error(this, $"{nameof(ArcInfo.LegiblePersistentCondition)} must be a persistent condition");
        }

        public override IEnumerable<AssetResource> GetResources()
        {
            foreach (var resource in base.GetResources())
                yield return resource;
            if (Arc.CustomTextImage && TranslatorText && TranslatorText.Planet)
                yield return new ImageResource(Arc.CustomTextImage, TranslatorText.Planet);
        }

        [Serializable]
        public class ArcInfo
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
            public float ZRotation;
            [Tooltip("The type of text to display")]
            [ConditionalField(nameof(CustomTextImage), (Texture2D)null)]
            public ArcType Type;
            [Tooltip("Allows you to create custom alien language text by overriding the displayed image. This will automatically set the type to Custom.")]
            public Texture2D CustomTextImage;
            [Tooltip("Makes this text require a persistent condition to be known before it can be translated. If you want it to always be untranslatable, use a condition that you will never set.")]
            public ConditionAsset LegiblePersistentCondition;
            [Tooltip("Replaces the \"Nomai\" part in \"Untranslated Nomai writing\". If the type is Custom, this will default to \"unknown\".")]
            public string CustomLanguageName;
            [Tooltip("Overrides the default unread color of the text arc.")]
            public NullishColor OverrideUnreadColor;
            [Tooltip("Overrides the default translated color of the text arc. If the unread color is overridden but this is not, the translated color will be a desaturated version of the unread color.")]
            [ConditionalField(nameof(OverrideUnreadColor), true)]
            public NullishColor OverrideTranslatedColor;

            public enum ArcType
            {
                Adult = 0,
                Child = 1,
                Stranger = 2,
                Teenager = 3,
                Custom = 4,
            }
        }
    }
}
