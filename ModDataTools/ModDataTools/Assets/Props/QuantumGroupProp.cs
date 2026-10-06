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
    public interface IQuantumGroupMember
    {
        public IProp GetQuantumGroup();
    }

    [Serializable]
    public class QuantumGroupPropData : GeneralPropData
    {
        [Tooltip("What type of group this is: a list of sockets that the group's details move between, a list of states that a single quantum object switches between, or a set of details that are alternated between during flashes of lightning")]
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
            var details = GetMembers<DetailPropData>(context);
            if (details.Any())
                writer.WriteProperty("details", details);
            if (Type == QuantumGroupType.Sockets)
            {
                writer.WriteProperty("sockets", GetMembers<QuantumSocketPropData>(context));
            }
            else if (Type == QuantumGroupType.States)
            {
                if (HasEmptyState)
                    writer.WriteProperty("hasEmptyState", HasEmptyState);
                if (Sequential)
                {
                    writer.WriteProperty("sequential", Sequential);
                    if (!Loop)
                        writer.WriteProperty("loop", Loop);
                }
            }
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            var detailCount = GetMembers<DetailPropData>(context).Count();
            if (detailCount == 0)
                validator.Error(context.Planet, $"Quantum group '{context.GetProp().PropName}' has no details");
            if (Type == QuantumGroupType.Sockets && GetMembers<QuantumSocketPropData>(context).Count() < detailCount)
                validator.Error(context.Planet, $"Quantum group '{context.GetProp().PropName}' has fewer sockets than details");
        }

        public static IEnumerable<string> GetQuantumObjectPaths(PropContext context, IProp group)
        {
            var groupContext = AssetRepository.GetPropContext<QuantumGroupPropData>(context.Planet, group);
            if (groupContext == null)
                return Enumerable.Empty<string>();
            if (groupContext.Data.Type == QuantumGroupType.Sockets)
                return GetMembers<DetailPropData>(groupContext).Select(d => d.Prop.GetPlanetPath(d));
            return new[] { group.GetPlanetPath(groupContext) };
        }

        public static IEnumerable<PropContext<T>> GetMembers<T>(PropContext context) where T : PropData
        {
            var group = context.GetProp();
            return AssetRepository.GetProps<T>(context.Planet)
                .Where(p => p.Prop is IQuantumGroupMember member && member.GetQuantumGroup() == group);
        }

        public enum QuantumGroupType
        {
            Sockets = 0,
            States = 1,
            Lightning = 2,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(QuantumGroupPropAsset))]
    public class QuantumGroupPropAsset : GeneralPropAsset<QuantumGroupPropData>
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Data.Type == QuantumGroupPropData.QuantumGroupType.Lightning)
            {
                base.WriteJsonProps(context, writer);
                return;
            }
            writer.WriteProperty("rename", FullID);
            Data.WriteJsonProps(context, writer);
        }

        public override string GetPlanetPath(PropContext context)
        {
            if (Data.Type == QuantumGroupPropData.QuantumGroupType.Lightning)
                return base.GetPlanetPath(context);
            return context.Planet.GetSectorPath() + "/" + FullID;
        }
    }

    public class QuantumGroupPropComponent : GeneralPropComponent<QuantumGroupPropData>
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            var data = (QuantumGroupPropData)GetData();
            if (data.Type == QuantumGroupPropData.QuantumGroupType.Lightning)
            {
                base.WriteJsonProps(context, writer);
                return;
            }
            writer.WriteProperty("rename", PropName);
            data.WriteJsonProps(context, writer);
        }

        public override string GetPlanetPath(PropContext context)
        {
            if (((QuantumGroupPropData)GetData()).Type == QuantumGroupPropData.QuantumGroupType.Lightning)
                return base.GetPlanetPath(context);
            return context.Planet.GetSectorPath() + "/" + PropName;
        }
    }
}
