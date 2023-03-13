using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    [CreateAssetMenu]
    public class Achievement : DataAsset, IValidateableAsset, IJsonAsset
    {
        [Tooltip("The mod this asset belongs to")]
        public ModManifest Mod;
        [Header("Data")]
        [Tooltip("The icon to display for this achievement.")]
        public Texture2D Icon;
        [Tooltip("Should the name and description of the achievement be hidden until it is unlocked. Good for hiding spoilers!")]
        public bool Secret;
        [Tooltip("A list of facts that must be discovered before this achievement is unlocked.")]
        public List<FactBase> Facts;
        //[Tooltip("A list of signals that must be discovered before this achievement is unlocked.")]
        //public List<Signal> Signals;
        [Tooltip("A list of conditions that must be true before this achievement is unlocked. Conditions can be set via dialogue.")]
        public List<Condition> Conditions;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Mod) yield return Mod;
        }

        public override string GetIDPrefix()
        {
            if (Mod) return Mod.GetFullID() + "_";
            return base.GetIDPrefix();
        }

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("ID", GetFullID());
            writer.WriteProperty("secret", Secret);
            if (Facts != null && Facts.Any())
                writer.WriteProperty("factIDs", Facts.Select(f => f.GetFullID()));
            //if (Signals != null && Signals.Any())
            //    writer.WriteProperty("signalIDs", Signals.Select(s => s.GetID()));
            if (Conditions != null && Conditions.Any())
                writer.WriteProperty("conditionIDs", Conditions.Select(c => c.GetFullID()));
            writer.WriteEndObject();
        }
        public string ToJsonString() => ExportUtility.ToJsonString(this);

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!Icon)
                validator.Warn(this, $"Missing {nameof(Icon)}");
            if ((Facts == null || !Facts.Any()) && /* (Signals == null || !Signals.Any()) && */ (Conditions == null || !Conditions.Any()))
                validator.Warn(this, $"No unlock criteria defined");
        }
    }
}
