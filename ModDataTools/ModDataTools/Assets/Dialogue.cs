using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModDataTools.Utilities;
using System.Xml;
using ModDataTools.Assets.Resources;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(DialogueAsset))]
    public class DialogueAsset : DataAsset, IValidateableAsset, IXmlSerializable
    {
        [Tooltip("The planet this dialogue is associated with")]
        public PlanetAsset Planet;
        [Header("Export")]
        [Tooltip("Whether to export a dialogue .xml file")]
        public bool ExportXmlFile = true;
        [Tooltip("The dialogue .xml file to use as-is instead of generating one")]
        public TextAsset OverrideXmlFile;
        [Header("Data")]
        [Tooltip("How the dialogue is presented in the interact prompt")]
        public DialogueType Type;
        [Tooltip("The name of the character, used for the interaction prompt")]
        [ConditionalField(nameof(Type), DialogueType.Character)]
        public string CharacterName;
        [Tooltip("The dialogue node that will be used if no other conditions are met")]
        public DialogueNodeAsset DefaultNode;
        [Header("Children")]
        [Tooltip("The flattened list of dialogue nodes")]
        [HideInInspector]
        public List<DialogueNodeAsset> Nodes = new();

        public enum DialogueType
        {
            Unknown = 0,
            Character = 1,
            Sign = 2,
            Recording = 3,
        }

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Planet) yield return Planet;
        }

        public override IEnumerable<DataAsset> GetNestedAssets() => Nodes;

        public override string GetChildIDPrefix() => $"{FullID}_";

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (OverrideXmlFile) return;
            if (!DefaultNode)
                validator.Error(this, $"Default node not set");
            foreach (var node in Nodes)
            {
                node.Validate(validator);
                if (node.Target && !Nodes.Any(n => n == node.Target))
                    validator.Error(this, $"Node '{node.FullID}' is targeting a non-existent node");
            }
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("DialogueTree");
            writer.WriteSchemaAttributes("https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/dialogue_schema.xsd");
            if (Type == DialogueType.Sign)
                writer.WriteElementString("NameField", "SIGN");
            else if (Type == DialogueType.Recording)
                writer.WriteElementString("NameField", "RECORDING");
            else if (Type == DialogueType.Character)
                writer.WriteElementString("NameField", FullID);
            else
                writer.WriteElementString("NameField", FullID);
            foreach (var node in Nodes)
                node.ToXml(writer);
            writer.WriteEndElement();
        }
        public string GetXmlOutputPath() =>
            $"dialogue/{Planet.StarSystem.FullID}/{Planet.FullID}/{FullID}.xml";

        public override void Localize(Localization l10n)
        {
            if (Type == DialogueType.Character)
                l10n.AddDialogue(FullID, CharacterName);
            else if (Type == DialogueType.Unknown)
                l10n.AddDialogue(FullID, FullName);
            foreach (var node in Nodes)
                node.Localize(l10n);
        }

        public override IEnumerable<AssetResource> GetResources()
        {
            if (ExportXmlFile)
            {
                if (OverrideXmlFile)
                    yield return new TextResource(OverrideXmlFile, GetXmlOutputPath());
                else
                    yield return new TextResource(ExportUtility.ToXmlString(this), GetXmlOutputPath());
            }
            foreach (var node in Nodes)
                foreach (var resource in node.GetResources())
                    yield return resource;
        }
    }
}
