using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using ModDataTools.Utilities;
using ModDataTools.Assets.PlanetModules;
using ModDataTools.Assets.Resources;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(PlanetAsset))]
    public class PlanetAsset : DataAsset, IValidateableAsset, IXmlSerializable, IJsonSerializable
    {
        [Tooltip("The solar system this planet belongs in")]
        public StarSystemAsset StarSystem;
        [Tooltip("A prefix appended to all entry and fact IDs belonging to this planet")]
        public string ChildIDPrefix;
        [Header("Export")]
        [Tooltip("Whether to export a New Horizons config file")]
        public bool ExportConfigFile = true;
        [Tooltip("The New Horizons planet config .json file to use as-is instead of generating one")]
        [ConditionalField(nameof(ExportConfigFile))]
        public TextAsset OverrideConfigFile;
        [Tooltip("Whether to export a ship log .xml file")]
        public bool ExportShipLogFile = true;
        [Tooltip("The ship log .xml file to use as-is instead of generating one")]
        [ConditionalField(nameof(ExportShipLogFile))]
        public TextAsset OverrideShipLogFile;
        [Header("Data")]
        [Tooltip("The configurable fields used to generate the New Horizons config .json file")]
        [ConditionalField(nameof(ExportConfigFile))]
        public NewHorizonsConfig NewHorizons;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (StarSystem) yield return StarSystem;
        }

        public override string GetChildIDPrefix()
        {
            if (!string.IsNullOrEmpty(ChildIDPrefix))
                return base.GetChildIDPrefix() + ChildIDPrefix + "_";
            return base.GetChildIDPrefix();
        }

        public override void Localize(Localization l10n)
        {
            l10n.AddUI(FullID, FullName);
            foreach (var module in NewHorizons.GetPlanetModules().Where(m => m.ShouldWrite(this)))
                module.Localize(this, l10n);
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!StarSystem)
                validator.Error(this, $"{nameof(StarSystem)} is not set. (For planets in the vanilla star system, create a non-exported star system with an {nameof(OverrideFullID)} of 'SolarSystem'.)");
            foreach (var module in NewHorizons.GetPlanetModules())
                module.Validate(this, validator);
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("AstroObjectEntry");
            writer.WriteSchemaAttributes("https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/shiplog_schema.xsd");
            writer.WriteElementString("ID", FullID);
            var entries = AssetRepository.GetAllAssets<EntryAsset>().Where(e => e.Planet == this && !e.Parent);
            foreach (var entry in entries)
            {
                entry.ToXml(writer);
            }
            writer.WriteEndElement();
        }

        public void ToJson(JsonTextWriter writer)
        {
            var nh = NewHorizons;

            writer.WriteStartObject();
            writer.WriteProperty("$schema", "https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/body_schema.json");
            writer.WriteProperty("name", FullID);
            writer.WriteProperty("starSystem", StarSystem.FullID);
            if (nh.Destroy)
            {
                writer.WriteProperty("destroy", nh.Destroy);
                writer.WriteEndObject();
                return;
            }
            if (nh.IsQuantumState)
                writer.WriteProperty("isQuantumState", nh.IsQuantumState);
            if (nh.IsStellarRemnant)
                writer.WriteProperty("isStellarRemnant", nh.IsStellarRemnant);
            if (!nh.CanShowOnTitle)
                writer.WriteProperty("canShowOnTitle", nh.CanShowOnTitle);
            if (!nh.TrackForSolarSystemRadius)
                writer.WriteProperty("trackForSolarSystemRadius", nh.TrackForSolarSystemRadius);
            if (nh.RemoveChildren.Any())
                writer.WriteProperty("removeChildren", nh.RemoveChildren);
            if (!nh.CheckForExisting)
                writer.WriteProperty("checkForExisting", nh.CheckForExisting);
            writer.WriteProperty("Base", nh.Base, this);
            writer.WriteProperty("AmbientLights", nh.AmbientLights, this);
            writer.WriteProperty("AsteroidBelt", nh.AsteroidBelt, this);
            writer.WriteProperty("Atmosphere", nh.Atmosphere, this);
            writer.WriteProperty("Bramble", nh.Bramble, this);
            writer.WriteProperty("Cloak", nh.Cloak, this);
            writer.WriteProperty("Dream", nh.Dream, this);
            writer.WriteProperty("EyeOfTheUniverse", nh.EyeOfTheUniverse, this);
            writer.WriteProperty("FocalPoint", nh.FocalPoint, this);
            writer.WriteProperty("Funnel", nh.Funnel, this);
            writer.WriteProperty("HeightMap", nh.HeightMap, this);
            writer.WriteProperty("Lava", nh.Lava, this);
            writer.WriteProperty("MapMarker", nh.MapMarker, this);
            writer.WriteProperty("Orbit", nh.Orbit, this);
            writer.WriteProperty("ProcGen", nh.ProcGen, this);
            writer.WriteProperty("Props", nh.Props, this);
            writer.WriteProperty("ReferenceFrame", nh.ReferenceFrame, this);
            writer.WriteProperty("Rings", nh.Rings, this);
            writer.WriteProperty("Sand", nh.Sand, this);
            writer.WriteProperty("ShipLog", nh.ShipLog, this);
            writer.WriteProperty("ShockEffect", nh.ShockEffect, this);
            writer.WriteProperty("Spawn", nh.Spawn, this);
            writer.WriteProperty("Star", nh.Star, this);
            writer.WriteProperty("Water", nh.Water, this);
            writer.WriteProperty("ParticleFields", nh.ParticleFields, this);
            writer.WriteProperty("Volumes", nh.Volumes, this);
            writer.WriteProperty("CometTail", nh.CometTail, this);
            writer.WriteEndObject();
        }

        public string GetConfigFilePath() => $"planets/{StarSystem.FullID}/{FullID}.json";
        public string GetShipLogFilePath() => $"shiplogs/{StarSystem.FullID}/{FullID}.xml";
        public string GetShipLogPhotoPath() => $"shiplogs/{StarSystem.FullID}/{FullID}/sprites/";

        public override IEnumerable<AssetResource> GetResources()
        {
            if (ExportConfigFile)
            {
                if (OverrideConfigFile)
                    yield return new TextResource(OverrideConfigFile, GetConfigFilePath());
                else
                    yield return new TextResource(ExportUtility.ToJsonString(this), GetConfigFilePath());
            }
            if (ExportShipLogFile)
            {
                if (OverrideShipLogFile)
                    yield return new TextResource(OverrideShipLogFile, GetShipLogFilePath());
                else
                    yield return new TextResource(ExportUtility.ToXmlString(this), GetShipLogFilePath());
            }
            foreach (var module in NewHorizons.GetPlanetModules().Where(m => m.ShouldWrite(this)))
            {
                foreach (var resource in module.GetResources(this))
                    yield return resource;
            }
        }

        public string GetResourcePath(UnityEngine.Object resource) => $"planets/{StarSystem.FullID}/{FullID}/{AssetRepository.GetAssetFileName(resource)}";

        public string GetSectorPath()
        {
            var planetBodyName = FullID.Replace(" ", "").Replace("_", "").Replace("'", "").ToUpper() switch
            {
                "ATTLEROCK" => "Moon",
                "HOLLOWSLANTERN" => "VolcanicMoon",
                "ASHTWIN" => "TowerTwin",
                "EMBERTWIN" => "CaveTwin",
                "INTERLOPER" => "Comet",
                "MAPSATELLITE" => "HearthianMapSatellite",
                "EYEOFTHEUNIVERSE" or "EYE" => "EyeOfTheUniverse",
                _ => FullID,
            };
            return planetBodyName switch
            {
                "TimberHearth" => "Sector_TH",
                "CaveTwin" => "Sector_CaveTwin",
                "RingWorld" => "Sector_RingWorld",
                "BrittleHollow" => "Sector_BH",
                "OrbitalProbeCannon" => "Sector_OrbitalProbeCannon",
                "WhiteholeStation" => "Sector_WhiteholeStation",
                "SunStation" => "Sector_SunStation",
                "QuantumIsland" => "Sector_QuantumIsland",
                "GiantsDeep" => "Sector_GD",
                "Comet" => "Sector_CO",
                "TowerTwin" => "Sector_TowerTwin",
                "DreamWorld" => "Sector_DreamWorld",
                "VolcanicMoon" => "Sector_VM",
                "DB_VesselDimension" => "Sector_VesselDimension",
                "StatueIsland" => "Sector_StatueIsland",
                "ConstructionYardIsland" => "Sector_ConstructionYard",
                "Ship" => "ShipSector",
                "Moon" => "Sector_THM",
                "DB_ExitOnlyDimension" => "Sector_ExitOnlyDimension",
                "CannonMuzzle" => "Sector_CannonDebrisTip",
                "DB_AnglerNestDimension" => "Sector_AnglerNestDimension",
                "Satellite" => "",
                "B_Elsinore" => "Sector_ElsinoreDimension",
                "DB_ClusterDimension" => "Sector_ClusterDimension",
                "GabbroShip" => "Sector_GabbroShip",
                "CannonBarrel" => "Sector_CannonDebrisMid",
                "GabbroIsland" => "Sector_GabbroIsland",
                "DB_PioneerDimension" => "Sector_PioneerDimension",
                "BrambleIsland" => "Sector_BrambleIsland",
                "BackerSatellite" => "Sector_BackerSatellite",
                "HearthianMapSatellite" => "Sector_HearthianMapSatellite",
                "DarkBramble" => "Sector_DB",
                "QuantumMoon" => "Sector_QuantumMoon",
                "DB_EscapePodDimension" => "Sector_EscapePodDimension",
                "Sector_EscapePodBody" => "Sector_EscapePodBody",
                "DB_HubDimension" => "Sector_HubDimension",
                "WhiteHole" => "Sector_WhiteHole",
                "FocalBody" => "Sector_HGT",
                "DB_SmallNest" => "Sector_SmallNestDimension",
                "Sun" => "Sector_SUN",
                "EyeOfTheUniverse" => "Sector_EyeOfTheUniverse",
                _ => "Sector",
            };
        }

        [Serializable]
        public class NewHorizonsConfig
        {
            [Tooltip("Set to true if you want to delete this planet")]
            public bool Destroy;
            [Tooltip("Does this config describe a quantum state of a custom planet defined in another file?")]
            public bool IsQuantumState;
            [Tooltip("Does this config describe a stellar remnant of a custom star defined in another file?")]
            public bool IsStellarRemnant;
            [Tooltip("Should this planet ever be shown on the title screen?")]
            public bool CanShowOnTitle = true;
            [Tooltip("Do we track the position of this body when calculating the solar system radius? `true` if you want the map zoom speed, map panning distance/speed, map camera farclip plane, and autopilot-returning-to-solar-system to adjust to this planet's orbit")]
            public bool TrackForSolarSystemRadius = true;
            [Tooltip("A list of paths to child GameObjects to destroy on this planet")]
            public List<string> RemoveChildren = new();
            [Tooltip("Optimization. Turn this off if you know you're generating a new body and aren't worried about other addons editing it.")]
            public bool CheckForExisting = true;
            [Tooltip("Base Properties of this body")]
            public BaseModule Base = new BaseModule() { IsEnabled = true };
            [Tooltip("Add ambient lights to this body")]
            public List<AmbientLightModule> AmbientLights = new();
            [Tooltip("Generate asteroids around this body")]
            public AsteroidBeltModule AsteroidBelt = new();
            [Tooltip("Describes this body's atmosphere")]
            public AtmosphereModule Atmosphere = new();
            [Tooltip("Make this planet a bramble dimension")]
            public BrambleModule Bramble = new();
            [Tooltip("Add a cloaking field for this planet")]
            public CloakModule Cloak = new();
            [Tooltip("Make this planet part of the dream world")]
            public DreamModule Dream = new();
            [Tooltip("Add features exclusive to the Eye of the Universe scene")]
            public EyeOfTheUniverseModule EyeOfTheUniverse = new();
            [Tooltip("Make this body into a focal point (barycenter)")]
            public FocalPointModule FocalPoint = new();
            [Tooltip("Add funnel from this planet to another")]
            public FunnelModule Funnel = new();
            [Tooltip("Generate the surface of this planet using a heightmap")]
            public HeightMapModule HeightMap = new();
            [Tooltip("Add lava on this planet")]
            public LavaModule Lava = new();
            [Tooltip("Map marker properties of this body")]
            public MapMarkerModule MapMarker = new();
            [Tooltip("Describes this Body's orbit (or lack there of)")]
            public OrbitModule Orbit = new OrbitModule() { IsEnabled = true };
            [Tooltip("Procedurally generated mesh for this planet's surface")]
            public ProcGenModule ProcGen = new();
            [Tooltip("Spawn various objects on this body")]
            [HideInInspector]
            public PropsModule Props = new();
            [Tooltip("Reference frame properties of this body")]
            public ReferenceFrameModule ReferenceFrame = new ReferenceFrameModule() { IsEnabled = true };
            [Tooltip("Add rings around the planet")]
            public List<RingsModule> Rings = new();
            [Tooltip("Add sand on this planet")]
            public SandModule Sand = new();
            [Tooltip("Describe how the planet looks in map mode")]
            public ShipLogModule ShipLog = new();
            [Tooltip("Does this planet have a shock effect when the nearest star goes supernova? Should be disabled for stars, focal points, and stellar remnants.")]
            public ShockEffectModule ShockEffect = new();
            [Tooltip("Spawn the player at this planet")]
            [HideInInspector]
            public SpawnModule Spawn = new();
            [Tooltip("Make this body a star")]
            public StarModule Star = new();
            [Tooltip("Add water on this planet")]
            public WaterModule Water = new();
            [Tooltip("Add particle effects in a field around the planet. Also known as Vection Fields.")]
            public List<ParticleFieldModule> ParticleFields = new();
            [Tooltip("Add various volumes on this body")]
            [HideInInspector]
            public VolumesModule Volumes = new();
            [Tooltip("Add a comet tail to this planet, like the Interloper")]
            public CometTailModule CometTail = new();

            public IEnumerable<PlanetModule> GetPlanetModules()
            {
                yield return Base;
                foreach (var ambientLight in AmbientLights)
                    yield return ambientLight;
                yield return AsteroidBelt;
                yield return Atmosphere;
                yield return Bramble;
                yield return Cloak;
                yield return Dream;
                yield return EyeOfTheUniverse;
                yield return FocalPoint;
                yield return Funnel;
                yield return HeightMap;
                yield return Lava;
                yield return MapMarker;
                yield return Orbit;
                yield return ProcGen;
                yield return Props;
                yield return ReferenceFrame;
                foreach (var ring in Rings)
                    yield return ring;
                yield return Sand;
                yield return ShipLog;
                yield return ShockEffect;
                yield return Spawn;
                yield return Star;
                yield return Water;
                foreach (var field in ParticleFields)
                    yield return field;
                yield return Volumes;
                yield return CometTail;
            }
        }
    }
}
