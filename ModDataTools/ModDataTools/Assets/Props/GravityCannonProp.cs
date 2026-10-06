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
    public class GravityCannonPropData : GeneralPropData
    {
        [Tooltip("Ship log fact revealed when retrieving the shuttle to this pad.")]
        public FactAsset RetrieveReveal;
        [Tooltip("Ship log fact revealed when launching from this pad.")]
        public FactAsset LaunchReveal;
        [Tooltip("Hide the lattice cage around the platform.")]
        public bool Detailed = true;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (RetrieveReveal)
                writer.WriteProperty("retrieveReveal", RetrieveReveal.FullID);
            if (LaunchReveal)
                writer.WriteProperty("launchReveal", LaunchReveal.FullID);
            if (!Detailed)
                writer.WriteProperty("detailed", Detailed);
        }

        public static void ValidateShuttle(PropContext context, IAssetValidator validator, IProp shuttle)
        {
            if (shuttle == null)
                validator.Error(context.Planet, $"Gravity cannon '{context.GetProp().PropName}' has no shuttle set");
        }
    }

    [Serializable]
    public class GravityCannonControlsPropData : GeneralPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {

        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(GravityCannonPropAsset))]
    public class GravityCannonPropAsset : GeneralPropAsset<GravityCannonPropData>
    {
        [Tooltip("The shuttle that pairs with this gravity cannon")]
        public ShuttlePropAsset Shuttle;
        [Tooltip("Will create a modern Nomai computer linked to this gravity cannon.")]
        public NomaiComputerPropAsset Computer;
        [Tooltip("Position of the interface used to launch the shuttle")]
        public GravityCannonControlsPropAsset Controls;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Shuttle)
                writer.WriteProperty("shuttleID", Shuttle.PropID);
            if (Computer)
                writer.WriteProperty("computer", context.MakeSibling(Computer));
            if (Controls)
                writer.WriteProperty("controls", context.MakeSibling(Controls));
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            GravityCannonPropData.ValidateShuttle(new PropContext<GravityCannonPropData> { Planet = Planet, Prop = this }, validator, Shuttle ? Shuttle : null);
        }
    }

    public class GravityCannonPropComponent : GeneralPropComponent<GravityCannonPropData>
    {
        [Tooltip("The shuttle that pairs with this gravity cannon")]
        public ShuttlePropAsset ShuttleAsset;
        [Tooltip("The shuttle that pairs with this gravity cannon")]
        public ShuttlePropComponent Shuttle;
        [Tooltip("Will create a modern Nomai computer linked to this gravity cannon.")]
        public NomaiComputerPropAsset ComputerAsset;
        [Tooltip("Will create a modern Nomai computer linked to this gravity cannon.")]
        public NomaiComputerPropComponent Computer;
        [Tooltip("Position of the interface used to launch the shuttle")]
        public GravityCannonControlsPropAsset ControlsAsset;
        [Tooltip("Position of the interface used to launch the shuttle")]
        public GravityCannonControlsPropComponent Controls;

        IProp GetShuttle() => ShuttleAsset ? (IProp)ShuttleAsset : Shuttle ? Shuttle : null;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (GetShuttle() != null)
                writer.WriteProperty("shuttleID", GetShuttle().PropID);
            if (ComputerAsset)
                writer.WriteProperty("computer", context.MakeSibling(ComputerAsset));
            else if (Computer)
                writer.WriteProperty("computer", context.MakeSibling(Computer));
            if (ControlsAsset)
                writer.WriteProperty("controls", context.MakeSibling(ControlsAsset));
            else if (Controls)
                writer.WriteProperty("controls", context.MakeSibling(Controls));
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            GravityCannonPropData.ValidateShuttle(context, validator, GetShuttle());
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(GravityCannonControlsPropAsset))]
    public class GravityCannonControlsPropAsset : GeneralPropAsset<GravityCannonControlsPropData> { }
    public class GravityCannonControlsPropComponent : GeneralPropComponent<GravityCannonControlsPropData> { }
}
