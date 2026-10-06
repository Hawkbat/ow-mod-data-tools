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
    public class InstrumentZonePropData : DetailPropData
    {

    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(InstrumentZonePropAsset))]
    public class InstrumentZonePropAsset : DetailPropAsset<InstrumentZonePropData>
    {
        [Tooltip("The Eye Traveler associated with this instrument zone.")]
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

    public class InstrumentZonePropComponent : DetailPropComponent<InstrumentZonePropData>
    {
        [Tooltip("The Eye Traveler associated with this instrument zone.")]
        public EyeTravelerPropAsset EyeTravelerAsset;
        [Tooltip("The Eye Traveler associated with this instrument zone.")]
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
