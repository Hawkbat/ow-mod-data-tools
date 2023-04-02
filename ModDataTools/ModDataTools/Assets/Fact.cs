using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModDataTools.Utilities;
using System.Xml;

namespace ModDataTools.Assets
{
    public abstract class FactAsset : DataAsset, IValidateableAsset, IXmlSerializable
    {
        [Tooltip("The entry this fact belongs to")]
        [ReadOnlyField]
        public EntryAsset Entry;
        [Header("Data")]
        [Tooltip("Whether to hide the \"More to explore\" for this fact")]
        public bool IgnoreMoreToExplore;
        [Tooltip("The text content for this fact")]
        [TextArea]
        public string Text;
        [Tooltip("The condition that needs to be fulfilled to have the alt text be displayed")]
        public FactAsset AltTextCondition;
        [Tooltip("The text to display if the condition is met")]
        [TextArea]
        [ConditionalField(nameof(AltTextCondition))]
        public string AltText;
        [Tooltip("Reveal this fact when the game starts")]
        public bool InitiallyRevealed;

        public abstract void ToXml(XmlWriter writer);

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Entry) yield return Entry;
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!Entry)
                validator.Error(this, $"Missing {nameof(Entry)}");
            if (string.IsNullOrEmpty(Text))
                validator.Warn(this, $"Missing {nameof(Text)}");
        }
    }
}
