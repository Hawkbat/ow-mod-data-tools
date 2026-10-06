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
    public class RaftPropData : GeneralPropData
    {
        [Tooltip("Acceleration of the raft. Default acceleration is 5.")]
        public float Acceleration = 5f;
        [Tooltip("Uses the raft model from the dreamworld")]
        public bool Pristine;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Acceleration != 5f)
                writer.WriteProperty("acceleration", Acceleration);
            if (Pristine)
                writer.WriteProperty("pristine", Pristine);
        }

        public static void WriteDockPath(PropContext context, JsonTextWriter writer, IProp dock)
        {
            var dockPath = AssetRepository.GetPropPlanetPath<RaftDockPropData>(context.Planet, dock);
            if (dockPath != null)
                writer.WriteProperty("dockPath", dockPath);
        }

        public static void ValidateDock(PropContext context, IAssetValidator validator, IProp dock)
        {
            if (AssetRepository.GetPropContext<RaftDockPropData>(context.Planet, dock) == null)
                validator.Error(context.Planet, $"Raft '{context.GetProp().PropName}' is docked to '{dock.PropName}', which is not on the same planet");
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(RaftPropAsset))]
    public class RaftPropAsset : GeneralPropAsset<RaftPropData>
    {
        [Tooltip("The dock this raft will start attached to.")]
        public RaftDockPropAsset Dock;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Dock)
                RaftPropData.WriteDockPath(context, writer, Dock);
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (Dock && Dock.Planet != Planet)
                validator.Error(this, $"{nameof(Dock)} is not on the same planet");
        }
    }

    public class RaftPropComponent : GeneralPropComponent<RaftPropData>
    {
        [Tooltip("The dock this raft will start attached to.")]
        public RaftDockPropAsset DockAsset;
        [Tooltip("The dock this raft will start attached to.")]
        public RaftDockPropComponent Dock;

        public IProp GetDock() => DockAsset ? (IProp)DockAsset : Dock ? Dock : null;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (GetDock() != null)
                RaftPropData.WriteDockPath(context, writer, GetDock());
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            if (GetDock() != null)
                RaftPropData.ValidateDock(context, validator, GetDock());
        }
    }
}
