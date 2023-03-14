using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using ModDataTools.Utilities;
using Delaunay;

namespace ModDataTools.Assets
{

    public class DialogueNode : DataAsset, IValidateableAsset, IXmlAsset
    {
        [Tooltip("The dialogue this node belongs to")]
        [ReadOnlyField]
        public Dialogue Dialogue;
        [Header("Data")]
        [Tooltip("The condition that needs to be met in order for the dialogue to begin at this node.")]
        public List<Condition> EntryConditions = new();
        [Tooltip("When used with multiple Dialogues, the node will choose a random one to show")]
        public bool Randomize;
        [Tooltip("Pages of dialogue to show to the player")]
        public List<string> Pages = new();
        [Tooltip("A list of options to show to the player once the character is done talking")]
        public List<Option> Options = new();
        [Tooltip("Facts to reveal when the player goes through this dialogue node")]
        public List<FactBase> RevealFacts = new();
        [Tooltip("Sets new conditions that will only last for the current loop or (if persistent) indefinitely in the current save, unless cancelled or deleted")]
        public List<Condition> SetConditions = new();
        [Tooltip("Ship log facts that must be revealed in order to proceed to the target node")]
        public List<FactBase> RequiredTargetFacts = new();
        [Tooltip("The dialogue node to go to after this node. Mutually exclusive with using dialogue options")]
        public DialogueNode Target;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Dialogue) yield return Dialogue;
        }

        public override string GetIDPrefix() => string.Empty;

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (Target && Target.Dialogue != Dialogue)
                validator.Error(this, $"Target node does not belong to the same dialogue");
            if (Target && Options.Any())
                validator.Error(this, $"Both a target node and dialogue options are set; they are mutually exclusive");
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("DialogueNode");
            writer.WriteElementString("Name", GetFullID());
            if (Dialogue && Dialogue.DefaultNode == this)
                writer.WriteElementString("EntryCondition", "DEFAULT");
            foreach (var condition in EntryConditions)
                writer.WriteElementString("EntryCondition", condition.GetFullID());
            if (Randomize)
                writer.WriteEmptyElement("Randomize");
            writer.WriteStartElement("Dialogue");
            foreach (var page in Pages)
                writer.WriteElementString("Page", page);
            writer.WriteEndElement();
            if (Options.Any())
            {
                writer.WriteStartElement("DialogueOptionsList");
                foreach (var option in Options)
                    option.ToXml(writer);
                writer.WriteEndElement();
            }
            if (RevealFacts.Any())
            {
                writer.WriteStartElement("RevealFacts");
                foreach (var fact in RevealFacts)
                    writer.WriteElementString("FactID", fact.GetFullID());
                writer.WriteEndElement();
            }
            foreach (var condition in SetConditions.Where(c => !c.Persistent))
                writer.WriteElementString("SetCondition", condition.GetFullID());
            foreach (var condition in SetConditions.Where(c => c.Persistent))
                writer.WriteElementString("SetPersistentCondition", condition.GetFullID());
            foreach (var fact in RequiredTargetFacts)
                writer.WriteElementString("DialogueTargetShipLogCondition", fact.GetFullID());
            if (Target)
                writer.WriteElementString("DialogueTarget", Target.GetFullID());
            writer.WriteEndElement();
        }
        public string ToXmlString() => ExportUtility.ToXmlString(this);

        [Serializable]
        public class Option : IXmlAsset
        {
            [Tooltip("The text to show for this option")]
            public string Text;
            [Tooltip("Require a condition or persistent condition to be met to show this option")]
            public List<Condition> RequiredConditions = new();
            [Tooltip("Hide this option if a condition or persistent condition has been met")]
            public List<Condition> CancelledConditions = new();
            [Tooltip("Require ship log facts to be known to show this option")]
            public List<FactBase> RequiredFacts = new();
            [Tooltip("Set these conditions when this option is chosen")]
            public List<Condition> ConditionsToSet = new();
            [Tooltip("Cancel these conditions when this option is chosen")]
            public List<Condition> ConditionsToCancel = new();
            [Tooltip("The dialogue node to go to when this option is selected")]
            public DialogueNode Target;

            public void ToXml(XmlWriter writer)
            {
                writer.WriteStartElement("DialogueOption");
                foreach (var condition in RequiredConditions.Where(c => c.Persistent))
                    writer.WriteElementString("RequiredPersistentCondition", condition.GetFullID());
                foreach (var condition in CancelledConditions.Where(c => c.Persistent))
                    writer.WriteElementString("CancelledPersistentCondition", condition.GetFullID());
                foreach (var condition in RequiredConditions.Where(c => !c.Persistent))
                    writer.WriteElementString("RequiredCondition", condition.GetFullID());
                foreach (var condition in CancelledConditions.Where(c => !c.Persistent))
                    writer.WriteElementString("CancelledCondition", condition.GetFullID());
                foreach (var fact in RequiredFacts)
                    writer.WriteElementString("RequiredLogCondition", fact.GetFullID());
                writer.WriteElementString("Text", Text);
                foreach (var condition in ConditionsToSet)
                    writer.WriteElementString("ConditionToSet", condition.GetFullID());
                foreach (var condition in ConditionsToCancel)
                    writer.WriteElementString("ConditionToCancel", condition.GetFullID());
                if (Target)
                    writer.WriteElementString("DialogueTarget", Target.GetFullID());
                writer.WriteEndElement();
            }
            public string ToXmlString() => ExportUtility.ToXmlString(this);
        }
    }
}
