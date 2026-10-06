using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class DreamLightConditionConfig
    {
        [Tooltip("The dialogue condition or persistent condition to set when the light is lit.")]
        public ConditionAsset Condition;
        [Tooltip("Whether to unset the condition when the light is extinguished again.")]
        [ConditionalField(nameof(Condition))]
        public bool Reversible;
        [Tooltip("Whether to set the condition when the light is extinguished instead. If reversible, the condition will be unset when the light is lit again.")]
        [ConditionalField(nameof(Condition))]
        public bool OnExtinguish;

        public void WriteJsonProperty(JsonTextWriter writer, string name)
        {
            if (!Condition) return;
            writer.WritePropertyName(name);
            writer.WriteStartObject();
            writer.WriteProperty("condition", Condition.FullID);
            if (Condition.Persistent)
                writer.WriteProperty("persistent", Condition.Persistent);
            if (Reversible)
                writer.WriteProperty("reversible", Reversible);
            if (OnExtinguish)
                writer.WriteProperty("onExtinguish", OnExtinguish);
            writer.WriteEndObject();
        }
    }
}
