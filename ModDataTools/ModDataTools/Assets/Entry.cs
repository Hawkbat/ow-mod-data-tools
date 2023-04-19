using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using ModDataTools.Utilities;
using ModDataTools.Assets.Resources;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(EntryAsset))]
    public class EntryAsset : DataAsset, IValidateableAsset, IXmlSerializable
    {
        [Tooltip("The planet this entry is for")]
        public PlanetAsset Planet;
        [Header("Data")]
        [Tooltip("The position of the entry in rumor mode")]
        public Vector2 RumorModePosition;
        [Tooltip("Whether to hide the \"More To Explore\" text on this entry")]
        public bool IgnoreMoreToExplore;
        [Tooltip("Ignore more to explore if a fact is known")]
        [ConditionalField(nameof(IgnoreMoreToExplore))]
        public FactAsset IgnoreMoreToExploreCondition;
        [Tooltip("The picture to show for the entry")]
        public Texture2D Photo;
        [Tooltip("An alt picture to show if a condition is met")]
        [ConditionalField(nameof(Photo))]
        public Texture2D AltPhoto;
        [Tooltip("If this fact is revealed, show the Alt picture")]
        [ConditionalField(nameof(AltPhoto))]
        public FactAsset AltPhotoCondition;

        [Tooltip("Whether this entry is a curiosity")]
        public bool IsCuriosity;

        [Tooltip("The curiosity this entry belongs to")]
        [ConditionalField(nameof(IsCuriosity), false)]
        public EntryAsset Curiosity;
        [Tooltip("The entry this entry is a child of")]
        [ConditionalField(nameof(IsCuriosity), false)]
        public EntryAsset Parent;

        [Tooltip("The color of the curiosity and associated entries when highlighted")]
        [ConditionalField(nameof(IsCuriosity))]
        public Color Color = Color.white;
        [Tooltip("If set, will override the default behavior of making the non-highlighted color a slightly darker version of the primary color")]
        [ConditionalField(nameof(IsCuriosity))]
        public NullishColor OverrideNormalColor;

        [Header("Children")]
        [Tooltip("The rumor facts associated with this entry")]
        [HideInInspector]
        public List<RumorFactAsset> RumorFacts = new();
        [Tooltip("The explore facts associated with this entry")]
        [HideInInspector]
        public List<ExploreFactAsset> ExploreFacts = new();

        public Vector2 EditorPosition
        {
            get => new(RumorModePosition.x, -RumorModePosition.y);
            set => RumorModePosition = new Vector2(value.x, -value.y);
        }

        public Color NormalColor
        {
            get
            {
                if (!IsCuriosity && Curiosity) return Curiosity.NormalColor;
                if (OverrideNormalColor.HasValue) return OverrideNormalColor.Value;
                Color.RGBToHSV(Color, out float h, out float s, out float v);
                return Color.HSVToRGB(h, s, v * 0.7f);
            }
        }

        public Color HighlightColor => IsCuriosity || !Curiosity ? Color : Curiosity.Color;

        public EntryAsset GetCuriosity() => IsCuriosity ? this : Curiosity;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Planet) yield return Planet;
        }

        public override IEnumerable<DataAsset> GetNestedAssets()
        {
            foreach (var fact in RumorFacts) yield return fact;
            foreach (var fact in ExploreFacts) yield return fact;
        }

        public override void Localize(Localization l10n)
        {
            l10n.AddShipLog(FullID, FullName);
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!Planet)
                validator.Error(this, $"Missing {nameof(Planet)}");
            if (!Photo)
                validator.Warn(this, $"Missing {nameof(Photo)}");
            if (!IsCuriosity && !Curiosity)
                validator.Warn(this, $"Missing {nameof(Curiosity)}");
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Entry");
            writer.WriteElementString("ID", FullID);
            writer.WriteElementString("Name", FullID);
            if (GetCuriosity())
                writer.WriteElementString("Curiosity", GetCuriosity().FullID);
            if (IsCuriosity)
                writer.WriteEmptyElement("IsCuriosity");
            if (IgnoreMoreToExplore && IgnoreMoreToExploreCondition)
                writer.WriteElementString("IgnoreMoreToExploreCondition", IgnoreMoreToExploreCondition.FullID);
            else if (IgnoreMoreToExplore)
                writer.WriteEmptyElement("IgnoreMoreToExplore");
            if (AltPhotoCondition)
                writer.WriteElementString("AltPhotoCondition", AltPhotoCondition.FullID);

            foreach (var fact in RumorFacts)
                fact.ToXml(writer);
            foreach (var fact in ExploreFacts)
                fact.ToXml(writer);

            var childEntries = AssetRepository.GetAllAssets<EntryAsset>().Where(e => e.Parent == this);
            foreach (var childEntry in childEntries)
                childEntry.ToXml(writer);

            writer.WriteEndElement();
        }

        public override IEnumerable<AssetResource> GetResources()
        {
            if (Photo)
                yield return new ImageResource(Photo, $"{Planet.GetShipLogPhotoPath()}/{FullID}.png");
            if (AltPhoto)
                yield return new ImageResource(Photo, $"{Planet.GetShipLogPhotoPath()}/{FullID}_ALT.png");
        }
    }
}
