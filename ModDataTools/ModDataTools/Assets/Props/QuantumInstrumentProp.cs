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
    public class QuantumInstrumentPropData : DetailPropData
    {
        [Tooltip("A dialogue condition to set when gathering this quantum instrument. Use it in conjunction with `activationCondition` or `deactivationCondition` on other details.")]
        public ConditionAsset GatherCondition;
        [Tooltip("Allows gathering this quantum instrument using the zoomed-in signalscope, like Chert's bongos.")]
        public bool GatherWithScope;
        [Tooltip("The audio signal emitted by this quantum instrument. The fields `name`, `audio`, and `frequency` will be copied from the corresponding Eye Traveler's signal if not specified here.")]
        public SignalPropData Signal;
        [Tooltip("The radius of the added sphere collider that will be used for interaction.")]
        public float InteractRadius = 0.5f;
        [Tooltip("The furthest distance where the player can interact with this quantum instrument.")]
        public float InteractRange = 2f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (GatherCondition)
                writer.WriteProperty("gatherCondition", GatherCondition.FullID);
            if (GatherWithScope)
                writer.WriteProperty("gatherWithScope", GatherWithScope);
            writer.WritePropertyName("signal");
            writer.WriteStartObject();
            Signal.WriteJsonProps(context, writer);
            writer.WriteEndObject();
            if (InteractRadius != 0.5f)
                writer.WriteProperty("interactRadius", InteractRadius);
            if (InteractRange != 2f)
                writer.WriteProperty("interactRange", InteractRange);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            Signal.Validate(context, validator);
            if (GatherCondition && GatherCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(GatherCondition)} must not be a persistent condition");
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            foreach (var resource in base.GetResources(context))
                yield return resource;
            foreach (var resource in Signal.GetResources(context))
                yield return resource;
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(QuantumInstrumentPropAsset))]
    public class QuantumInstrumentPropAsset : DetailPropAsset<QuantumInstrumentPropData>
    {
        [Tooltip("The Eye Traveler associated with this quantum instrument.")]
        public EyeTravelerPropAsset EyeTraveler;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (EyeTraveler)
                writer.WriteProperty("id", EyeTraveler.FullID);
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!EyeTraveler)
                validator.Error(this, "Missing Eye Traveler");
        }
    }

    public class QuantumInstrumentPropComponent : DetailPropComponent<QuantumInstrumentPropData>
    {
        [Tooltip("The Eye Traveler associated with this quantum instrument.")]
        public EyeTravelerPropAsset EyeTravelerAsset;
        [Tooltip("The Eye Traveler associated with this quantum instrument.")]
        public EyeTravelerPropComponent EyeTraveler;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (EyeTravelerAsset)
                writer.WriteProperty("id", EyeTravelerAsset.FullID);
            else if (EyeTraveler)
                writer.WriteProperty("id", EyeTraveler.PropID);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            if (!EyeTravelerAsset && !EyeTraveler)
                validator.Error(context.Planet, "Missing Eye Traveler");
        }
    }
}
