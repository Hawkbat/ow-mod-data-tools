using ModDataTools.Assets.Props;
using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Assets.PlanetModules
{
    [Serializable]
    public class PropsModule : PlanetModule
    {
        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            var details = AssetRepository.GetProps<DetailPropData>(planet)
                .Where(p => !p.Data.IsProxyDetail && !(p.Prop is IQuantumGroupMember member && member.GetQuantumGroup() != null));
            var dialogues = AssetRepository.GetProps<DialoguePropData>(planet);
            var entryLocations = AssetRepository.GetProps<EntryLocationPropData>(planet);
            var geysers = AssetRepository.GetProps<GeyserPropData>(planet);
            var translatorTexts = AssetRepository.GetProps<TranslatorTextPropData>(planet);
            var proxyDetails = AssetRepository.GetProps<DetailPropData>(planet)
                .Where(p => p.Data.IsProxyDetail);
            var rafts = AssetRepository.GetProps<RaftPropData>(planet);
            var raftDocks = AssetRepository.GetProps<RaftDockPropData>(planet);
            var scatters = AssetRepository.GetProps<ScatterPropData>(planet);
            var slideShows = AssetRepository.GetProps<SlideShowPropData>(planet);
            var quantumGroups = AssetRepository.GetProps<QuantumGroupPropData>(planet);
            var socketQuantumGroups = quantumGroups.Where(g => g.Data.Type == QuantumGroupPropData.QuantumGroupType.Sockets);
            var stateQuantumGroups = quantumGroups.Where(g => g.Data.Type == QuantumGroupPropData.QuantumGroupType.States);
            var lightningQuantumGroups = quantumGroups.Where(g => g.Data.Type == QuantumGroupPropData.QuantumGroupType.Lightning);
            var tornados = AssetRepository.GetProps<TornadoPropData>(planet);
            var volcanoes = AssetRepository.GetProps<VolcanoPropData>(planet);
            var singularities = AssetRepository.GetProps<SingularityPropData>(planet);
            var signals = AssetRepository.GetProps<SignalPropData>(planet);
            var remotePlatforms = AssetRepository.GetProps<RemotePlatformPropData>(planet);
            var remoteWhiteboards = AssetRepository.GetProps<RemoteWhiteboardPropData>(planet);
            var remoteStones = AssetRepository.GetProps<RemoteStonePropData>(planet);
            var warpReceivers = AssetRepository.GetProps<WarpReceiverPropData>(planet);
            var warpTransmitters = AssetRepository.GetProps<WarpTransmitterPropData>(planet);
            var audioSources = AssetRepository.GetProps<AudioSourcePropData>(planet);
            var gravityCannons = AssetRepository.GetProps<GravityCannonPropData>(planet);
            var shuttles = AssetRepository.GetProps<ShuttlePropData>(planet);
            var campfires = AssetRepository.GetProps<CampfirePropData>(planet);
            var dreamCampfires = AssetRepository.GetProps<DreamCampfirePropData>(planet);
            var alarmBells = AssetRepository.GetProps<AlarmBellPropData>(planet);
            var dreamArrivalPoints = AssetRepository.GetProps<DreamArrivalPointPropData>(planet);
            var grappleTotems = AssetRepository.GetProps<GrappleTotemPropData>(planet);
            var alarmTotems = AssetRepository.GetProps<AlarmTotemPropData>(planet);
            var portholes = AssetRepository.GetProps<PortholePropData>(planet);
            var dreamCandles = AssetRepository.GetProps<DreamCandlePropData>(planet);
            var projectionTotems = AssetRepository.GetProps<ProjectionTotemPropData>(planet);
            var fuelTanks = AssetRepository.GetProps<FuelTankPropData>(planet);

            if (details.Any())
                writer.WriteProperty("details", details);
            if (dialogues.Any())
                writer.WriteProperty("dialogue", dialogues);
            if (entryLocations.Any())
                writer.WriteProperty("entryLocation", entryLocations);
            if (geysers.Any())
                writer.WriteProperty("geysers", geysers);
            if (translatorTexts.Any())
                writer.WriteProperty("translatorText", translatorTexts);
            if (proxyDetails.Any())
                writer.WriteProperty("proxyDetails", proxyDetails);
            if (rafts.Any())
                writer.WriteProperty("rafts", rafts);
            if (raftDocks.Any())
                writer.WriteProperty("raftDocks", raftDocks);
            if (scatters.Any())
                writer.WriteProperty("scatter", scatters);
            if (slideShows.Any())
                writer.WriteProperty("slideShows", slideShows);
            if (socketQuantumGroups.Any())
                writer.WriteProperty("socketQuantumGroups", socketQuantumGroups);
            if (stateQuantumGroups.Any())
                writer.WriteProperty("stateQuantumGroups", stateQuantumGroups);
            if (lightningQuantumGroups.Any())
                writer.WriteProperty("lightningQuantumGroups", lightningQuantumGroups);
            if (tornados.Any())
                writer.WriteProperty("tornados", tornados);
            if (volcanoes.Any())
                writer.WriteProperty("volcanoes", volcanoes);
            if (singularities.Any())
                writer.WriteProperty("singularities", singularities);
            if (signals.Any())
                writer.WriteProperty("signals", signals);
            if (remotePlatforms.Any() || remoteWhiteboards.Any() || remoteStones.Any())
            {
                writer.WritePropertyName("remotes");
                writer.WriteStartArray();
                foreach (var platform in remotePlatforms)
                {
                    var data = platform.Data;
                    writer.WriteStartObject();
                    writer.WriteProperty("id", data.RemoteProjection.FullID);
                    writer.WriteProperty("decalPath", data.RemoteProjection.StarSystem.GetResourcePath(data.RemoteProjection.Decal));
                    writer.WriteProperty("platform", platform);
                    writer.WriteEndObject();
                }
                foreach (var whiteboard in remoteWhiteboards)
                {
                    var data = whiteboard.Data;
                    writer.WriteStartObject();
                    writer.WriteProperty("id", data.RemoteProjection.FullID);
                    writer.WriteProperty("decalPath", data.RemoteProjection.StarSystem.GetResourcePath(data.RemoteProjection.Decal));
                    writer.WriteProperty("whiteboard", whiteboard);
                    writer.WriteEndObject();
                }
                foreach (var stones in remoteStones.GroupBy(s => s.Data.RemoteProjection))
                {
                    writer.WriteStartObject();
                    writer.WriteProperty("id", stones.Key.FullID);
                    writer.WriteProperty("decalPath", stones.Key.StarSystem.GetResourcePath(stones.Key.Decal));
                    writer.WriteProperty("stones", stones);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            if (warpReceivers.Any())
                writer.WriteProperty("warpReceivers", warpReceivers);
            if (warpTransmitters.Any())
                writer.WriteProperty("warpTransmitters", warpTransmitters);
            if (audioSources.Any())
                writer.WriteProperty("audioSources", audioSources);
            if (gravityCannons.Any())
                writer.WriteProperty("gravityCannons", gravityCannons);
            if (shuttles.Any())
                writer.WriteProperty("shuttles", shuttles);
            if (campfires.Any())
                writer.WriteProperty("campfires", campfires);
            if (dreamCampfires.Any())
                writer.WriteProperty("dreamCampfires", dreamCampfires);
            if (alarmBells.Any())
                writer.WriteProperty("alarmBells", alarmBells);
            if (dreamArrivalPoints.Any())
                writer.WriteProperty("dreamArrivalPoints", dreamArrivalPoints);
            if (grappleTotems.Any())
                writer.WriteProperty("grappleTotems", grappleTotems);
            if (alarmTotems.Any())
                writer.WriteProperty("alarmTotems", alarmTotems);
            if (portholes.Any())
                writer.WriteProperty("portholes", portholes);
            if (dreamCandles.Any())
                writer.WriteProperty("dreamCandles", dreamCandles);
            if (projectionTotems.Any())
                writer.WriteProperty("projectionTotems", projectionTotems);
            if (fuelTanks.Any())
                writer.WriteProperty("fuelTanks", fuelTanks);
        }

        public IEnumerable<PropContext> GetProps(PlanetAsset planet)
        {
            foreach (var prop in AssetRepository.GetProps<DetailPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<DialoguePropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<EntryLocationPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<GeyserPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<TranslatorTextPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RaftPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RaftDockPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ScatterPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<SlideShowPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<QuantumGroupPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<QuantumSocketPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<TornadoPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<VolcanoPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<SingularityPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<SignalPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RemotePlatformPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RemoteWhiteboardPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RemoteStonePropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<WarpReceiverPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<WarpTransmitterPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<AudioSourcePropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<GravityCannonPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ShuttlePropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<CampfirePropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<DreamCampfirePropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<AlarmBellPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<DreamArrivalPointPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<GrappleTotemPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<AlarmTotemPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<PortholePropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<DreamCandlePropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ProjectionTotemPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<FuelTankPropData>(planet)) yield return prop;
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            foreach (var prop in GetProps(planet))
                foreach (var resource in prop.GetProp().GetResources(prop))
                    yield return resource;
        }

        public override void Localize(PlanetAsset planet, Localization l10n)
        {
            foreach (var prop in GetProps(planet))
                prop.GetProp().Localize(prop, l10n);
        }

        public override void Validate(PlanetAsset planet, IAssetValidator validator)
        {
            foreach (var prop in GetProps(planet))
                prop.GetProp().Validate(prop, validator);
        }

        public override bool ShouldWrite(PlanetAsset planet) => GetProps(planet).Any();
    }
}
