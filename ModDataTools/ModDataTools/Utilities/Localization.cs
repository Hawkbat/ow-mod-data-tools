using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Utilities
{
    public class Localization : IJsonSerializable
    {
        public readonly string LanguageName;

        readonly Dictionary<string, string> ui = new();
        readonly Dictionary<string, string> shiplog = new();
        readonly Dictionary<string, string> dialogue = new();
        readonly Dictionary<string, AchievementLocalization> achievements = new();

        public Localization(string languageName)
        {
            LanguageName = languageName;
        }

        public void AddUI(string key, string value)
        {
            ui[key] = value;
        }

        public void AddShipLog(string key, string value)
        {
            shiplog[key] = value;
        }

        public void AddDialogue(string key, string value)
        {
            dialogue[key] = value;
        }

        public void AddAchivement(string key, string name, string desc)
        {
            achievements[key] = new AchievementLocalization() {
                Name = name,
                Description = desc
            };
        }

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("$schema", "https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/translation_schema.json");
            if (dialogue.Any())
            {
                writer.WritePropertyName("DialogueDictionary");
                writer.WriteStartObject();
                foreach (var (key, value) in dialogue)
                    writer.WriteProperty(key, value);
                writer.WriteEndObject();
            }
            if (shiplog.Any())
            {
                writer.WritePropertyName("ShipLogDictionary");
                writer.WriteStartObject();
                foreach (var (key, value) in shiplog)
                    writer.WriteProperty(key, value);
                writer.WriteEndObject();
            }
            if (ui.Any())
            {
                writer.WritePropertyName("UIDictionary");
                writer.WriteStartObject();
                foreach (var (key, value) in ui)
                    writer.WriteProperty(key, value);
                writer.WriteEndObject();
            }
            if (achievements.Any())
            {
                writer.WritePropertyName("AchievementTranslations");
                writer.WriteStartObject();
                foreach (var (key, value) in achievements)
                {
                    writer.WritePropertyName(key);
                    writer.WriteStartObject();
                    writer.WriteProperty("Name", value.Name);
                    writer.WriteProperty("Description", value.Description);
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();

            }
            writer.WriteEndObject();
        }

        [Serializable]
        public class AchievementLocalization
        {
            public string Name;
            public string Description;
        }
    }
}
