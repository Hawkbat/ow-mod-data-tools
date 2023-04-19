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
    public class QuantumGroupPropData : PropData
    {
        [Tooltip("What type of group this is: does it define a list of states a single quantum object could take or a list of sockets one or more quantum objects could share?")]
        public QuantumGroupType Type;
        [ConditionalField(nameof(Type), QuantumGroupType.States)]
        [Tooltip("If this is true, then the first prop made part of this group will be used to construct a visibility box for an empty game object, which will be considered one of the states.")]
        public bool HasEmptyState;
        [ConditionalField(nameof(Type), QuantumGroupType.States)]
        [Tooltip("If this is true, then the states will be presented in order, rather than in a random order")]
        public bool Sequential;
        [ConditionalField(nameof(Type), QuantumGroupType.States)]
        [Tooltip($"Only applicable if {nameof(Sequential)} is set. If this is false, then after the last state has appeared, the object will no longer change state")]
        public bool Loop = true;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("type", Type);
            if (Type == QuantumGroupType.States)
            {
                writer.WriteProperty("hasEmptyState", HasEmptyState);
                writer.WriteProperty("sequential", Sequential);
                if (Sequential)
                    writer.WriteProperty("loop", Loop);
            }
        }

        public enum QuantumGroupType
        {
            Sockets = 0,
            States = 1,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(QuantumGroupPropAsset))]
    public class QuantumGroupPropAsset : PropDataAsset<QuantumGroupPropData> {

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("id", FullID);
            if (Data.Type == QuantumGroupPropData.QuantumGroupType.Sockets)
            {
                var childSockets = AssetRepository.GetProps<QuantumSocketPropData>(context.Planet)
                .Where(ctx => (ctx.Prop is QuantumSocketPropAsset sa && sa.QuantumGroup == this)
                    || (ctx.Prop is QuantumSocketPropComponent sc && sc.QuantumGroupAsset == this));
                writer.WriteProperty("sockets", childSockets);
            }
            base.WriteJsonProps(context, writer);
        }

        public override string GetPlanetPath(PropContext context) => context.DetailPath + "/" + $"Quantum Sockets - " + FullID;
    }

    public class QuantumGroupPropComponent : PropDataComponent<QuantumGroupPropData> {

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("id", PropID);
            if (Data.Type == QuantumGroupPropData.QuantumGroupType.Sockets)
            {
                var childSockets = AssetRepository.GetProps<QuantumSocketPropData>(context.Planet)
                    .Where(ctx => ctx.Prop is QuantumSocketPropComponent sc && sc.QuantumGroup == this);
                writer.WriteProperty("sockets", childSockets);
            }
            base.WriteJsonProps(context, writer);
        }

        public override string GetPlanetPath(PropContext context) => context.DetailPath + "/" + $"Quantum Sockets - " + PropID;
    }
}
