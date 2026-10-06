using ModDataTools.Assets.Props;
using ModDataTools.Assets.Resources;
using ModDataTools.Assets.Volumes;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Assets.PlanetModules
{
    [Serializable]
    public class VolumesModule : PlanetModule
    {
        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            var audioVolumes = AssetRepository.GetProps<AudioVolumeData>(planet);
            var conditionTriggerVolumes = AssetRepository.GetProps<ConditionTriggerVolumeData>(planet);
            var cylindricalForceVolumes = AssetRepository.GetProps<CylindricalForceVolumeData>(planet);
            var directionalForceVolumes = AssetRepository.GetProps<DirectionalForceVolumeData>(planet);
            var gravityVolumes = AssetRepository.GetProps<GravityVolumeData>(planet);
            var polarForceVolumes = AssetRepository.GetProps<PolarForceVolumeData>(planet);
            var radialForceVolumes = AssetRepository.GetProps<RadialForceVolumeData>(planet);
            var interactionVolumes = AssetRepository.GetProps<InteractionVolumeData>(planet);
            var playerRecoveryVolumes = AssetRepository.GetProps<PlayerRecoveryVolumeData>(planet);
            var quantumToggleVolumes = AssetRepository.GetProps<QuantumToggleVolumeData>(planet);
            var repairVolumes = AssetRepository.GetProps<RepairVolumeData>(planet);
            var speedLimiterVolumes = AssetRepository.GetProps<SpeedLimiterVolumeData>(planet);
            var dirtEffectVolumes = AssetRepository.GetProps<DirtEffectVolumeData>(planet);
            var dayNightAudioVolumes = AssetRepository.GetProps<DayNightAudioVolumeData>(planet);
            var destructionVolumes = AssetRepository.GetProps<DestructionVolumeData>(planet);
            var fluidVolumes = AssetRepository.GetProps<FluidVolumeData>(planet);
            var hazardVolumes = AssetRepository.GetProps<HazardVolumeData>(planet);
            var interferenceVolumes = AssetRepository.GetProps<InterferenceVolumeData>(planet);
            var insulatingVolumes = AssetRepository.GetProps<InsulatingVolumeData>(planet);
            var lightSourceVolumes = AssetRepository.GetProps<LightSourceVolumeData>(planet);
            var mapRestrictionVolumes = AssetRepository.GetProps<MapRestrictionVolumeData>(planet);
            var notificationVolumes = AssetRepository.GetProps<NotificationVolumeData>(planet);
            var oxygenVolumes = AssetRepository.GetProps<OxygenVolumeData>(planet);
            var probeDestructionVolumes = AssetRepository.GetProps<ProbeDestructionVolumeData>(planet);
            var probeSafetyVolumes = AssetRepository.GetProps<ProbeSafetyVolumeData>(planet);
            var referenceFrameBlockerVolumes = AssetRepository.GetProps<ReferenceFrameBlockerVolumeData>(planet);
            var revealVolumes = AssetRepository.GetProps<RevealVolumeData>(planet);
            var reverbVolumes = AssetRepository.GetProps<ReverbVolumeData>(planet);
            var antiTravelMusicRulesetVolumes = AssetRepository.GetProps<AntiTravelMusicRulesetVolumeData>(planet);
            var playerImpactRulesetVolumes = AssetRepository.GetProps<PlayerImpactRulesetVolumeData>(planet);
            var probeRulesetVolumes = AssetRepository.GetProps<ProbeRulesetVolumeData>(planet);
            var thrustRulesetVolumes = AssetRepository.GetProps<ThrustRulesetVolumeData>(planet);
            var speedTrapVolumes = AssetRepository.GetProps<SpeedTrapVolumeData>(planet);
            var frostEffectVolumes = AssetRepository.GetProps<FrostEffectVolumeData>(planet);
            var rainEffectVolumes = AssetRepository.GetProps<RainEffectVolumeData>(planet);
            var zeroGravityVolumes = AssetRepository.GetProps<ZeroGravityVolumeData>(planet);
            var solarSystemVolumes = AssetRepository.GetProps<SolarSystemVolumeData>(planet);
            var creditsVolumes = AssetRepository.GetProps<CreditsVolumeData>(planet);

            if (audioVolumes.Any())
                writer.WriteProperty("audioVolumes", audioVolumes);
            if (conditionTriggerVolumes.Any())
                writer.WriteProperty("conditionTriggerVolumes", conditionTriggerVolumes);
            if (dayNightAudioVolumes.Any())
                writer.WriteProperty("dayNightAudioVolumes", dayNightAudioVolumes);
            if (destructionVolumes.Any())
                writer.WriteProperty("destructionVolumes", destructionVolumes);
            if (fluidVolumes.Any())
                writer.WriteProperty("fluidVolumes", fluidVolumes);
            if (cylindricalForceVolumes.Any() || directionalForceVolumes.Any() || gravityVolumes.Any() || polarForceVolumes.Any() || radialForceVolumes.Any())
            {
                writer.WritePropertyName("forces");
                writer.WriteStartObject();
                if (cylindricalForceVolumes.Any())
                    writer.WriteProperty("cylindricalVolumes", cylindricalForceVolumes);
                if (directionalForceVolumes.Any())
                    writer.WriteProperty("directionalVolumes", directionalForceVolumes);
                if (gravityVolumes.Any())
                    writer.WriteProperty("gravityVolumes", gravityVolumes);
                if (polarForceVolumes.Any())
                    writer.WriteProperty("polarVolumes", polarForceVolumes);
                if (radialForceVolumes.Any())
                    writer.WriteProperty("radialVolumes", radialForceVolumes);
                writer.WriteEndObject();
            }
            if (hazardVolumes.Any())
                writer.WriteProperty("hazardVolumes", hazardVolumes);
            if (interferenceVolumes.Any())
                writer.WriteProperty("interferenceVolumes", interferenceVolumes);
            if (interactionVolumes.Any())
                writer.WriteProperty("interactionVolumes", interactionVolumes);
            if (playerRecoveryVolumes.Any())
                writer.WriteProperty("playerRecoveryVolumes", playerRecoveryVolumes);
            if (insulatingVolumes.Any())
                writer.WriteProperty("insulatingVolumes", insulatingVolumes);
            if (lightSourceVolumes.Any())
                writer.WriteProperty("lightSourceVolumes", lightSourceVolumes);
            if (mapRestrictionVolumes.Any())
                writer.WriteProperty("mapRestrictionVolumes", mapRestrictionVolumes);
            if (notificationVolumes.Any())
                writer.WriteProperty("notificationVolumes", notificationVolumes);
            if (quantumToggleVolumes.Any())
                writer.WriteProperty("quantumToggleVolumes", quantumToggleVolumes);
            if (oxygenVolumes.Any())
                writer.WriteProperty("oxygenVolumes", oxygenVolumes);
            if (probeDestructionVolumes.Any() || probeSafetyVolumes.Any())
            {
                writer.WritePropertyName("probe");
                writer.WriteStartObject();
                if (probeDestructionVolumes.Any())
                    writer.WriteProperty("probeDestructionVolumes", probeDestructionVolumes);
                if (probeSafetyVolumes.Any())
                    writer.WriteProperty("probeSafetyVolumes", probeSafetyVolumes);
                writer.WriteEndObject();
            }
            if (referenceFrameBlockerVolumes.Any())
                writer.WriteProperty("referenceFrameBlockerVolumes", referenceFrameBlockerVolumes);
            if (repairVolumes.Any())
                writer.WriteProperty("repairVolumes", repairVolumes);
            if (revealVolumes.Any())
                writer.WriteProperty("revealVolumes", revealVolumes);
            if (reverbVolumes.Any())
                writer.WriteProperty("reverbVolumes", reverbVolumes);
            if (antiTravelMusicRulesetVolumes.Any() || playerImpactRulesetVolumes.Any() || probeRulesetVolumes.Any() || thrustRulesetVolumes.Any())
            {
                writer.WritePropertyName("rulesets");
                writer.WriteStartObject();
                if (antiTravelMusicRulesetVolumes.Any())
                    writer.WriteProperty("antiTravelMusicRulesetVolumes", antiTravelMusicRulesetVolumes);
                if (playerImpactRulesetVolumes.Any())
                    writer.WriteProperty("playerImpactRulesetVolumes", playerImpactRulesetVolumes);
                if (probeRulesetVolumes.Any())
                    writer.WriteProperty("probeRulesetVolumes", probeRulesetVolumes);
                if (thrustRulesetVolumes.Any())
                    writer.WriteProperty("thrustRulesetVolumes", thrustRulesetVolumes);
                writer.WriteEndObject();
            }
            if (speedTrapVolumes.Any())
                writer.WriteProperty("speedTrapVolumes", speedTrapVolumes);
            if (speedLimiterVolumes.Any())
                writer.WriteProperty("speedLimiterVolumes", speedLimiterVolumes);
            if (frostEffectVolumes.Any() || rainEffectVolumes.Any() || dirtEffectVolumes.Any())
            {
                writer.WritePropertyName("visorEffects");
                writer.WriteStartObject();
                if (frostEffectVolumes.Any())
                    writer.WriteProperty("frostEffectVolumes", frostEffectVolumes);
                if (rainEffectVolumes.Any())
                    writer.WriteProperty("rainEffectVolumes", rainEffectVolumes);
                if (dirtEffectVolumes.Any())
                    writer.WriteProperty("dirtEffectVolumes", dirtEffectVolumes);
                writer.WriteEndObject();
            }
            if (zeroGravityVolumes.Any())
                writer.WriteProperty("zeroGravityVolumes", zeroGravityVolumes);
            if (solarSystemVolumes.Any())
                writer.WriteProperty("starSystemVolumes", solarSystemVolumes);
            if (creditsVolumes.Any())
                writer.WriteProperty("creditsVolume", creditsVolumes);
        }

        public IEnumerable<PropContext> GetProps(PlanetAsset planet)
        {
            foreach (var prop in AssetRepository.GetProps<AudioVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<DayNightAudioVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<DestructionVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<FluidVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<HazardVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<InterferenceVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<InsulatingVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<LightSourceVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<MapRestrictionVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<NotificationVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<OxygenVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ProbeDestructionVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ProbeSafetyVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ReferenceFrameBlockerVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RevealVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ReverbVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<AntiTravelMusicRulesetVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<PlayerImpactRulesetVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ProbeRulesetVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ThrustRulesetVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<SpeedTrapVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<FrostEffectVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RainEffectVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ZeroGravityVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<SolarSystemVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<CreditsVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ConditionTriggerVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<CylindricalForceVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<DirectionalForceVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<GravityVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<PolarForceVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RadialForceVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<InteractionVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<PlayerRecoveryVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<QuantumToggleVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RepairVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<SpeedLimiterVolumeData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<DirtEffectVolumeData>(planet)) yield return prop;
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            foreach (var resource in base.GetResources(planet))
                yield return resource;
            foreach (var prop in GetProps(planet))
                foreach (var resource in prop.GetProp().GetResources(prop))
                    yield return resource;
        }

        public override void Validate(PlanetAsset planet, IAssetValidator validator)
        {
            base.Validate(planet, validator);
            foreach (var prop in GetProps(planet))
                prop.GetProp().Validate(prop, validator);
        }

        public override void Localize(PlanetAsset planet, Localization l10n)
        {
            base.Localize(planet, l10n);
            foreach (var prop in GetProps(planet))
                prop.GetProp().Localize(prop, l10n);
        }

        public override bool ShouldWrite(PlanetAsset planet) => GetProps(planet).Any();
    }
}
