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
                .Where(p => !p.Data.IsProxyDetail);
            var dialogues = AssetRepository.GetProps<DialoguePropData>(planet);
            var entryLocations = AssetRepository.GetProps<EntryLocationPropData>(planet);
            var geysers = AssetRepository.GetProps<GeyserPropData>(planet);
            var translatorTexts = AssetRepository.GetProps<TranslatorTextPropData>(planet);
            var proxyDetails = AssetRepository.GetProps<DetailPropData>(planet)
                .Where(p => p.Data.IsProxyDetail);
            var rafts = AssetRepository.GetProps<RaftPropData>(planet);
            var scatters = AssetRepository.GetProps<ScatterPropData>(planet);
            var slideShows = AssetRepository.GetProps<SlideShowPropData>(planet);
            var quantumGroups = AssetRepository.GetProps<QuantumGroupPropData>(planet);
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
            if (scatters.Any())
                writer.WriteProperty("scatter", scatters);
            if (slideShows.Any())
                writer.WriteProperty("slideShows", slideShows);
            if (quantumGroups.Any())
                writer.WriteProperty("quantumGroups", quantumGroups);
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
        }

        public IEnumerable<PropContext> GetProps(PlanetAsset planet)
        {
            foreach (var prop in AssetRepository.GetProps<DetailPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<DialoguePropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<EntryLocationPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<GeyserPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<TranslatorTextPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<RaftPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<ScatterPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<SlideShowPropData>(planet)) yield return prop;
            foreach (var prop in AssetRepository.GetProps<QuantumGroupPropData>(planet)) yield return prop;
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
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            foreach (var prop in GetProps(planet))
                foreach (var resource in prop.GetProp().GetData().GetResources(prop))
                    yield return resource;
        }

        public override void Localize(PlanetAsset planet, Localization l10n)
        {
            foreach (var prop in GetProps(planet))
                prop.GetProp().GetData().Localize(prop, l10n);
        }

        public override void Validate(PlanetAsset planet, IAssetValidator validator)
        {
            foreach (var prop in GetProps(planet))
                prop.GetProp().GetData().Validate(prop, planet, validator);
        }

        public override bool ShouldWrite(PlanetAsset planet) => GetProps(planet).Any();
    }
}
