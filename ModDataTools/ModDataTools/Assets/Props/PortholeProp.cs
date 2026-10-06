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
    public class PortholePropData : GeneralPropData
    {
        [Tooltip("Ship log facts to reveal when peeking through the porthole.")]
        public List<FactAsset> RevealFacts = new();
        [Tooltip("The field of view of the porthole camera.")]
        public float FieldOfView = 90f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (RevealFacts.Any(f => f))
                writer.WriteProperty("revealFacts", RevealFacts.Where(f => f).Select(f => f.FullID));
            if (FieldOfView != 90f)
                writer.WriteProperty("fieldOfView", FieldOfView);
        }
    }

    [Serializable]
    public class PortholeTargetPropData : GeneralSolarSystemPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {

        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(PortholePropAsset))]
    public class PortholePropAsset : GeneralPropAsset<PortholePropData>
    {
        [Tooltip("The location of the camera when the player peeks through the porthole. Can be placed on a different planet.")]
        public PortholeTargetPropAsset Target;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Target)
                writer.WriteProperty("target", Target.GetContext(context));
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!Target)
                validator.Error(this, $"Missing {nameof(Target)}");
        }
    }

    public class PortholePropComponent : GeneralPropComponent<PortholePropData>
    {
        [Tooltip("The location of the camera when the player peeks through the porthole. Can be placed on a different planet.")]
        public PortholeTargetPropAsset TargetAsset;
        [Tooltip("The location of the camera when the player peeks through the porthole.")]
        public PortholeTargetPropComponent Target;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (TargetAsset)
                writer.WriteProperty("target", TargetAsset.GetContext(context));
            else if (Target)
                writer.WriteProperty("target", context.MakeSibling(Target));
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            if (!TargetAsset && !Target)
                validator.Error(context.Planet, $"Porthole '{PropName}' has no target set");
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(PortholeTargetPropAsset))]
    public class PortholeTargetPropAsset : GeneralSolarSystemPropAsset<PortholeTargetPropData>
    {
        public PropContext<PortholeTargetPropData> GetContext(PropContext portholeContext)
        {
            if (!Planet)
                return portholeContext.MakeSibling(this);
            return new PropContext<PortholeTargetPropData>
            {
                Planet = Planet,
                DetailPath = Planet.GetSectorPath(),
                Prop = this,
            };
        }
    }

    public class PortholeTargetPropComponent : GeneralSolarSystemPropComponent<PortholeTargetPropData> { }
}
