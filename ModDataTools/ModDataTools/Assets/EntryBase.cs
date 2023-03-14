using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using ModDataTools.Utilities;

namespace ModDataTools.Assets
{
    public abstract class EntryBase : DataAsset, IValidateableAsset, IXmlAsset
    {
        [Tooltip("The planet this entry is for")]
        public Planet Planet;
        [Header("Data")]
        [Tooltip("The position of the entry in rumor mode")]
        public Vector2 RumorModePosition;
        [Tooltip("Whether to hide the \"More To Explore\" text on this entry")]
        public bool IgnoreMoreToExplore;
        [Tooltip("Ignore more to explore if a fact is known")]
        [ConditionalField(nameof(IgnoreMoreToExplore))]
        public FactBase IgnoreMoreToExploreCondition;
        [Tooltip("The picture to show for the entry")]
        public Texture2D Photo;
        [Tooltip("An alt picture to show if a condition is met")]
        [ConditionalField(nameof(Photo))]
        public Texture2D AltPhoto;
        [Tooltip("If this fact is revealed, show the Alt picture")]
        [ConditionalField(nameof(AltPhoto))]
        public FactBase AltPhotoCondition;
        [Header("Children")]
        [Tooltip("The rumor facts associated with this entry")]
        [HideInInspector]
        public List<RumorFact> RumorFacts = new();
        [Tooltip("The explore facts associated with this entry")]
        [HideInInspector]
        public List<ExploreFact> ExploreFacts = new();

        public Vector2 EditorPosition
        {
            get => new(RumorModePosition.x, -RumorModePosition.y);
            set => RumorModePosition = new Vector2(value.x, -value.y);
        }

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Planet) yield return Planet;
        }

        public override IEnumerable<DataAsset> GetNestedAssets()
        {
            foreach (var fact in RumorFacts) yield return fact;
            foreach (var fact in ExploreFacts) yield return fact;
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!Planet)
                validator.Error(this, $"Missing {nameof(Planet)}");
            if (!Photo)
                validator.Warn(this, $"Missing {nameof(Photo)}");
        }

        public abstract Curiosity GetCuriosity();

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Entry");
            writer.WriteElementString("ID", GetFullID());
            writer.WriteElementString("Name", GetFullName());
            writer.WriteElementString("Curiosity", GetCuriosity().GetFullID());
            if (this is Curiosity)
                writer.WriteEmptyElement("IsCuriosity");
            if (IgnoreMoreToExplore && IgnoreMoreToExploreCondition)
                writer.WriteElementString("IgnoreMoreToExploreCondition", IgnoreMoreToExploreCondition.GetFullID());
            else if (IgnoreMoreToExplore)
                writer.WriteEmptyElement("IgnoreMoreToExplore");
            if (AltPhotoCondition)
                writer.WriteElementString("AltPhotoCondition", AltPhotoCondition.GetFullID());

            foreach (var fact in RumorFacts)
                fact.ToXml(writer);
            foreach (var fact in ExploreFacts)
                fact.ToXml(writer);

            var childEntries = AssetRepository.GetAllAssets<Entry>().Where(e => e.Parent == this);
            foreach (var childEntry in childEntries)
                childEntry.ToXml(writer);

            writer.WriteEndElement();
        }
        public string ToXmlString() => ExportUtility.ToXmlString(this);
    }
}
