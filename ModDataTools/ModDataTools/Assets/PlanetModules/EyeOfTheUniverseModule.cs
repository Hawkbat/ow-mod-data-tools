using ModDataTools.Assets.Props;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Assets.PlanetModules
{
    public class EyeOfTheUniverseModule : PlanetModule
    {
        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            var eyeTravelers = AssetRepository.GetProps<EyeTravelerPropData>(planet);
            if (eyeTravelers.Any())
                writer.WriteProperty("eyeTravelers", eyeTravelers);
            var instrumentZones = AssetRepository.GetProps<InstrumentZonePropData>(planet);
            if (instrumentZones.Any())
                writer.WriteProperty("instrumentZones", instrumentZones);
            var quantumInstruments = AssetRepository.GetProps<QuantumInstrumentPropData>(planet);
            if (quantumInstruments.Any())
                writer.WriteProperty("quantumInstruments", quantumInstruments);
        }

        public override void Validate(PlanetAsset planet, IAssetValidator validator)
        {
            foreach (var prop in AssetRepository.GetProps<EyeTravelerPropData>(planet))
                validator.Validate(prop);
            foreach (var prop in AssetRepository.GetProps<InstrumentZonePropData>(planet))
                validator.Validate(prop);
            foreach (var prop in AssetRepository.GetProps<QuantumInstrumentPropData>(planet))
                validator.Validate(prop);
        }

        public override bool ShouldWrite(PlanetAsset planet) =>
            base.ShouldWrite(planet) ||
            AssetRepository.GetProps<EyeTravelerPropData>(planet).Any() ||
            AssetRepository.GetProps<InstrumentZonePropData>(planet).Any() ||
            AssetRepository.GetProps<QuantumInstrumentPropData>(planet).Any();
    }
}
