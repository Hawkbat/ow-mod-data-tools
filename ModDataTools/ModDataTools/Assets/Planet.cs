using Newtonsoft.Json;
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
    [CreateAssetMenu]
    public class Planet : DataAsset, IValidateableAsset, IXmlAsset, IJsonAsset
    {
        [Tooltip("The solar system this planet belongs in")]
        public StarSystem SolarSystem;
        [Tooltip("A prefix appended to all entry and fact IDs belonging to this planet")]
        public string ChildIDPrefix;
        [Header("Export")]
        [Tooltip("Whether to export a New Horizons config file")]
        public bool ExportConfigFile = true;
        [Tooltip("The New Horizons planet config .json file to use as-is instead of generating one")]
        [ConditionalField(nameof(ExportConfigFile))]
        public TextAsset OverrideConfigFile;
        [Tooltip("Whether to export a ship log .xml file")]
        public bool ExportShipLogFile = true;
        [Tooltip("The ship log .xml file to use as-is instead of generating one")]
        [ConditionalField(nameof(ExportShipLogFile))]
        public TextAsset OverrideShipLogFile;
        [Header("Data")]
        [Tooltip("The configurable fields used to generate the New Horizons config .json file")]
        [ConditionalField(nameof(ExportConfigFile))]
        public NewHorizonsConfig NewHorizons;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (SolarSystem) yield return SolarSystem;
        }

        public override string GetChildIDPrefix()
        {
            if (!string.IsNullOrEmpty(ChildIDPrefix))
                return base.GetChildIDPrefix() + ChildIDPrefix + "_";
            return base.GetChildIDPrefix();
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("AstroObjectEntry");
            writer.WriteSchemaAttributes("https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/shiplog_schema.xsd");
            writer.WriteElementString("ID", GetFullID());
            var entries = AssetRepository.GetAllAssets<EntryBase>().Where(e => e.Planet == this);
            foreach (var entry in entries)
            {
                entry.ToXml(writer);
            }
            writer.WriteEndElement();
        }
        public string ToXmlString() => ExportUtility.ToXmlString(this);

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("$schema", "https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/body_schema.json");
            writer.WriteProperty("name", GetFullID());
            writer.WriteProperty("starSystem", SolarSystem.GetFullName());
            writer.WritePropertyName("ShipLog");
            writer.WriteStartObject();
            writer.WriteProperty("spriteFolder", "shiplogs/" + SolarSystem.GetFullName() + "/" + GetFullName() + "/sprites/");
            writer.WriteProperty("xmlFile", "shiplogs/" + SolarSystem.GetFullName() + "/" + GetFullName() + ".xml");
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
        public string ToJsonString() => ExportUtility.ToJsonString(this);

        [Serializable]
        public class NewHorizonsConfig
        {

        }
    }
}
