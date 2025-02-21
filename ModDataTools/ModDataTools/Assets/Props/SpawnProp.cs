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
    public abstract class SpawnPropData : GeneralPropData
    {
        [Tooltip("Whether this planet's spawn point is the one the player will initially spawn at, if multiple spawn points exist.")]
        public bool IsDefault;
        [Tooltip("If the given ship log fact is revealed, this spawn point will be used. Do not use at the same time as isDefault or makeDefaultIfPersistentCondition. Spawns unlocked with this have highest priority")]
        [ConditionalField(nameof(IsDefault), Invert = true)]
        public FactAsset MakeDefaultIfFactRevealed;
        [Tooltip("If the given persistent condition is true, this spawn point will be used. Do not use at the same time as isDefault or makeDefaultIfFactRevealed. Spawns unlocked with this have second highest priority")]
        [ConditionalField(nameof(IsDefault), Invert = true)]
        public ConditionAsset MakeDefaultIfPersistentConditionSet;
        [Tooltip("Offsets the player/ship by this local vector when spawning. Used to prevent spawning in the floor. Optional: defaults to (0, 4, 0).")]
        public NullishVector3 Offset;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("offset", Offset);
            if (MakeDefaultIfFactRevealed)
                writer.WriteProperty("makeDefaultIfFactRevealed", MakeDefaultIfFactRevealed.FullID);
            if (MakeDefaultIfPersistentConditionSet)
                writer.WriteProperty("makeDefaultIfPersistentCondition", MakeDefaultIfPersistentConditionSet.FullID);
            if (IsDefault)
                writer.WriteProperty("isDefault", IsDefault);
        }
    }
    public abstract class SpawnPropAsset<T> : GeneralPropAsset<T> where T : SpawnPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("id", FullID);
        }
    }
    public abstract class SpawnPropComponent<T> : GeneralPropComponent<T> where T : SpawnPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("id", PropID);
        }
    }
}
