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
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("AstroObjectEntry");
            writer.WriteSchemaAttributes("https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/shiplog_schema.xsd");
            writer.WriteElementString("ID", FullID);
            var entries = AssetRepository.GetAllAssets<EntryAsset>().Where(e => e.Planet == this);
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
            writer.WriteProperty("Base", nh.Base, this);
            writer.WriteProperty("AmbientLights", nh.AmbientLights, this);
            writer.WriteProperty("AsteroidBelt", nh.AsteroidBelt, this);
            writer.WriteProperty("Atmosphere", nh.Atmosphere, this);
            writer.WriteProperty("Bramble", nh.Bramble, this);
            if (!nh.CanShowOnTitle)
                writer.WriteProperty("canShowOnTitle", nh.CanShowOnTitle);
            writer.WriteProperty("Cloak", nh.Cloak, this);
            writer.WriteProperty("FocalPoint", nh.FocalPoint, this);
            writer.WriteProperty("Funnel", nh.Funnel, this);
            writer.WriteProperty("HeightMap", nh.HeightMap, this);
            if (nh.IsQuantumState)
                writer.WriteProperty("isQuantumState", nh.IsQuantumState);
            if (nh.IsStellarRemnant)
                writer.WriteProperty("isStellarRemnant", nh.IsStellarRemnant);
            writer.WriteProperty("Lava", nh.Lava, this);
            writer.WriteProperty("Orbit", nh.Orbit, this);
            writer.WriteProperty("ProcGen", nh.ProcGen, this);
            writer.WriteProperty("Props", nh.Props, this);
            writer.WriteProperty("ReferenceFrame", nh.ReferenceFrame, this);
            if (nh.RemoveChildren.Any())
                writer.WriteProperty("removeChildren", nh.RemoveChildren);
            writer.WriteProperty("Rings", nh.Rings, this);
            writer.WriteProperty("Sand", nh.Sand, this);
            writer.WriteProperty("ShipLog", nh.ShipLog, this);
            writer.WriteProperty("ShockEffect", nh.ShockEffect, this);
            writer.WriteProperty("Spawn", nh.Spawn, this);
            writer.WriteProperty("Star", nh.Star, this);
            writer.WriteProperty("Water", nh.Water, this);
            writer.WriteProperty("Volumes", nh.Volumes, this);

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
            var planetBodyName = FullID;
            switch (FullID.Replace(" ", "").Replace("_", "").Replace("'", "").ToUpper())
            {
                case "ATTLEROCK": planetBodyName = "Moon"; break;
                case "HOLLOWSLANTERN": planetBodyName = "VolcanicMoon"; break;
                case "ASHTWIN": planetBodyName = "TowerTwin"; break;
                case "EMBERTWIN": planetBodyName = "CaveTwin"; break;
                case "INTERLOPER": planetBodyName = "Comet"; break;
                case "MAPSATELLITE": planetBodyName = "HearthianMapSatellite"; break;
                case "EYEOFTHEUNIVERSE":
                case "EYE": planetBodyName = "EyeOfTheUniverse"; break;
            }
            switch (planetBodyName)
            {
                case "TimberHearth": return "Sector_TH";
                case "CaveTwin": return "Sector_CaveTwin";
                case "RingWorld": return "Sector_RingWorld";
                case "BrittleHollow": return "Sector_BH";
                case "OrbitalProbeCannon": return "Sector_OrbitalProbeCannon";
                case "WhiteholeStation": return "Sector_WhiteholeStation";
                case "SunStation": return "Sector_SunStation";
                case "QuantumIsland": return "Sector_QuantumIsland";
                case "GiantsDeep": return "Sector_GD";
                case "Comet": return "Sector_CO";
                case "TowerTwin": return "Sector_TowerTwin";
                case "DreamWorld": return "Sector_DreamWorld";
                case "VolcanicMoon": return "Sector_VM";
                case "DB_VesselDimension": return "Sector_VesselDimension";
                case "StatueIsland": return "Sector_StatueIsland";
                case "ConstructionYardIsland": return "Sector_ConstructionYard";
                case "Ship": return "ShipSector";
                case "Moon": return "Sector_THM";
                case "DB_ExitOnlyDimension": return "Sector_ExitOnlyDimension";
                case "CannonMuzzle": return "Sector_CannonDebrisTip";
                case "DB_AnglerNestDimension": return "Sector_AnglerNestDimension";
                case "Satellite": return "";
                case "B_Elsinore": return "Sector_ElsinoreDimension";
                case "DB_ClusterDimension": return "Sector_ClusterDimension";
                case "GabbroShip": return "Sector_GabbroShip";
                case "CannonBarrel": return "Sector_CannonDebrisMid";
                case "GabbroIsland": return "Sector_GabbroIsland";
                case "DB_PioneerDimension": return "Sector_PioneerDimension";
                case "BrambleIsland": return "Sector_BrambleIsland";
                case "BackerSatellite": return "Sector_BackerSatellite";
                case "HearthianMapSatellite": return "Sector_HearthianMapSatellite";
                case "DarkBramble": return "Sector_DB";
                case "QuantumMoon": return "Sector_QuantumMoon";
                case "DB_EscapePodDimension": return "Sector_EscapePodDimension";
                case "Sector_EscapePodBody": return "Sector_EscapePodBody";
                case "DB_HubDimension": return "Sector_HubDimension";
                case "WhiteHole": return "Sector_WhiteHole";
                case "FocalBody": return "Sector_HGT";
                case "DB_SmallNest": return "Sector_SmallNestDimension";
                case "Sun": return "Sector_SUN";
                case "EyeOfTheUniverse": return "Sector_EyeOfTheUniverse";
                default: return "Sector";
            }
        }

        [Serializable]
        public class NewHorizonsConfig
        {
            [Tooltip("Set to true if you want to delete this planet")]
            public bool Destroy;
            [Tooltip("Should this planet ever be shown on the title screen?")]
            public bool CanShowOnTitle = true;
            [Tooltip("Does this config describe a quantum state of a custom planet defined in another file?")]
            public bool IsQuantumState;
            [Tooltip("Does this config describe a stellar remnant of a custom star defined in another file?")]
            public bool IsStellarRemnant;
            [Tooltip("A list of paths to child GameObjects to destroy on this planet")]
            public List<string> RemoveChildren = new();
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
            [Tooltip("Make this body into a focal point (barycenter)")]
            public FocalPointModule FocalPoint = new();
            [Tooltip("Add funnel from this planet to another")]
            public FunnelModule Funnel = new();
            [Tooltip("Generate the surface of this planet using a heightmap")]
            public HeightMapModule HeightMap = new();
            [Tooltip("Add lava on this planet")]
            public LavaModule Lava = new();
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
            public RingsModule Rings = new();
            [Tooltip("Add sand on this planet")]
            public SandModule Sand = new();
            [Tooltip("Describe how the planet looks in map mode")]
            public ShipLogModule ShipLog = new();
            [Tooltip("Does this planet have a shock effect when the nearest star goes supernova? Should be disabled for stars, focal points, and stellar remnants.")]
            public ShockEffect ShockEffect = new();
            [Tooltip("Spawn the player at this planet")]
            [HideInInspector]
            public SpawnModule Spawn = new();
            [Tooltip("Make this body a star")]
            public StarModule Star = new();
            [Tooltip("Add water on this planet")]
            public WaterModule Water = new();
            [Tooltip("Add various volumes on this body")]
            [HideInInspector]
            public VolumesModule Volumes = new();

            public IEnumerable<PlanetModule> GetPlanetModules()
            {
                yield return Base;
                foreach (var ambientLight in AmbientLights)
                    yield return ambientLight;
                yield return AsteroidBelt;
                yield return Atmosphere;
                yield return Bramble;
                yield return Cloak;
                yield return FocalPoint;
                yield return Funnel;
                yield return HeightMap;
                yield return Lava;
                yield return Orbit;
                yield return ProcGen;
                yield return Props;
                yield return ReferenceFrame;
                yield return Rings;
                yield return Sand;
                yield return ShipLog;
                yield return ShockEffect;
                yield return Spawn;
                yield return Star;
                yield return Water;
                yield return Volumes;
            }
        }
    }
}
