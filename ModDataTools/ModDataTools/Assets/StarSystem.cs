using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModDataTools.Utilities;
using Newtonsoft.Json;

namespace ModDataTools.Assets
{
    [CreateAssetMenu]
    public class StarSystem : DataAsset, IValidateableAsset, IJsonAsset
    {
        [Tooltip("The mod this asset belongs to")]
        public ModManifest Mod;
        [Tooltip("A prefix appended to all entry and fact IDs belonging to planets in this star system")]
        public string ChildIDPrefix;
        [Header("Export")]
        [Tooltip("Whether to export a New Horizons config file")]
        public bool ExportConfigFile = true;
        [Tooltip("The New Horizons star system config .json file to use as-is instead of generating one")]
        [ConditionalField(nameof(ExportConfigFile))]
        public TextAsset OverrideConfigFile;
        [Header("Data")]
        [Tooltip("The configurable fields used to generate the New Horizons config .json file")]
        [ConditionalField(nameof(ExportConfigFile))]
        public NewHorizonsConfig NewHorizons;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Mod) yield return Mod;
        }

        public override string GetChildIDPrefix()
        {
            if (!string.IsNullOrEmpty(ChildIDPrefix))
                return base.GetChildIDPrefix() + ChildIDPrefix + "_";
            return base.GetChildIDPrefix();
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (NewHorizons.HasWarpCoordinates && !NewHorizons.Vessel.WarpCoordinates.IsValid())
                validator.Error(this, $"Invalid warp coordinates");
            if (NewHorizons.Skybox.HasCustomSkybox && !NewHorizons.Skybox.IsCustomSkyboxValid())
                validator.Error(this, $"Missing some skybox images");
        }

        public void ToJson(JsonTextWriter writer)
        {
            var nh = NewHorizons;
            writer.WriteStartObject();
            writer.WriteProperty("$schema", "https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/star_system_schema.json");
            if (nh.FarClipPlaneOverrideEnabled)
                writer.WriteProperty("farClipPlaneOverride", nh.FarClipPlaneOverride);
            writer.WriteProperty("canEnterViaWarpDrive", nh.CanEnterViaWarpDrive);
            writer.WriteProperty("destroyStockPlanets", nh.DestroyStockPlanets);
            writer.WriteProperty("enableTimeLoop", nh.EnableTimeLoop);
            if (nh.CanEnterViaWarpDrive && nh.FactRequiredForWarp)
                writer.WriteProperty("factRequiredForWarp", nh.FactRequiredForWarp);
            if (nh.EnableTimeLoop)
                writer.WriteProperty("loopDuration", nh.LoopDuration);
            writer.WriteProperty("mapRestricted", nh.MapRestricted);
            writer.WritePropertyName("Skybox");
            writer.WriteStartObject();
            writer.WriteProperty("destroyStarField", nh.Skybox.DestroyStarField);
            if (nh.Skybox.HasCustomSkybox)
            {
                writer.WriteProperty("useCube", nh.Skybox.UseCube);
                writer.WriteProperty("rightPath", "systems/" + GetFullID() + "/skybox/" + "right.png");
                writer.WriteProperty("leftPath", "systems/" + GetFullID() + "/skybox/" + "left.png");
                writer.WriteProperty("topPath", "systems/" + GetFullID() + "/skybox/" + "top.png");
                writer.WriteProperty("bottomPath", "systems/" + GetFullID() + "/skybox/" + "bottom.png");
                writer.WriteProperty("frontPath", "systems/" + GetFullID() + "/skybox/" + "front.png");
                writer.WriteProperty("backPath", "systems/" + GetFullID() + "/skybox/" + "back.png");
            }
            writer.WriteEndObject();
            writer.WriteProperty("startHere", nh.StartHere);
            writer.WriteProperty("respawnHere", nh.RespawnHere);
            if (nh.TravelAudio)
                writer.WriteProperty("travelAudio", "systems/" + GetFullID() + "/travel.wav");
            else if (nh.TravelAudioType != AudioType.None)
                writer.WriteProperty("travelAudio", nh.TravelAudioType.ToString());
            if (nh.HasWarpCoordinates)
            {
                writer.WritePropertyName("Vessel");
                writer.WriteStartObject();
                writer.WritePropertyName("coords");
                writer.WriteStartObject();
                writer.WriteProperty("x", nh.Vessel.WarpCoordinates.x);
                writer.WriteProperty("y", nh.Vessel.WarpCoordinates.y);
                writer.WriteProperty("z", nh.Vessel.WarpCoordinates.z);
                writer.WriteEndObject();
                writer.WriteProperty("vesselPosition", nh.Vessel.VesselPosition);
                writer.WriteProperty("vesselRotation", nh.Vessel.VesselRotation);
                writer.WriteProperty("warpExitPosition", nh.Vessel.WarpExitPosition);
                writer.WriteProperty("warpExitRotation", nh.Vessel.WarpExitRotation);
                if (nh.Vessel.PromptFact)
                    writer.WriteProperty("promptFact", nh.Vessel.PromptFact.GetFullID());
                writer.WriteEndObject();
            }
            var entries = AssetRepository.GetAllAssets<EntryBase>().Where(e => e.Planet && e.Planet.SolarSystem == this);
            if (entries.Any())
            {
                writer.WritePropertyName("entryPositions");
                writer.WriteStartArray();
                foreach (var entry in entries)
                {
                    writer.WriteStartObject();
                    writer.WriteProperty("id", entry.GetFullID());
                    writer.WriteProperty("position", entry.RumorModePosition);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            var initialFacts = AssetRepository.GetAllAssets<FactBase>()
                .Where(f => f.InitiallyRevealed && f.Entry && f.Entry.Planet && f.Entry.Planet.SolarSystem == this);
            if (initialFacts.Any())
                writer.WriteProperty("initialReveal", initialFacts.Select(f => f.GetFullID()));
            var curiosities = AssetRepository.GetAllAssets<Curiosity>().Where(e => e.Planet && e.Planet.SolarSystem == this);
            if (curiosities.Any())
            {
                writer.WritePropertyName("curiosities");
                writer.WriteStartArray();
                foreach (var curiosity in curiosities)
                {
                    writer.WriteStartObject();
                    writer.WriteProperty("color", curiosity.NormalColor);
                    writer.WriteProperty("highlightColor", curiosity.HighlightColor);
                    writer.WriteProperty("id", curiosity.GetFullID());
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
        public string ToJsonString() => ExportUtility.ToJsonString(this);

        [Serializable]
        public class NewHorizonsConfig
        {
            [Tooltip("An override value for the far clip plane. Allows you to see farther.")]
            public bool FarClipPlaneOverrideEnabled;
            [Tooltip("An override value for the far clip plane. Allows you to see farther.")]
            [ConditionalField(nameof(FarClipPlaneOverrideEnabled))]
            public float FarClipPlaneOverride;
            [Tooltip("Whether this system can be warped to via the warp drive")]
            public bool CanEnterViaWarpDrive = true;
            [Tooltip("Set to the Fact that must be revealed before it can be warped to.")]
            [ConditionalField(nameof(CanEnterViaWarpDrive))]
            public FactBase FactRequiredForWarp;
            [Tooltip("Do you want a clean slate for this star system? Or will it be a modified version of the original.")]
            public bool DestroyStockPlanets = true;
            [Tooltip("Should the time loop be enabled in this system?")]
            public bool EnableTimeLoop = true;
            [Tooltip("The duration of the time loop in minutes. This is the time the sun explodes. End Times plays 85 seconds before this time, and your memories get sent back about 40 seconds after this time.")]
            [ConditionalField(nameof(EnableTimeLoop))]
            public float LoopDuration = 22f;
            [Tooltip("Should the player not be able to view the map in this system?")]
            public bool MapRestricted;
            [Tooltip("The skybox to show in this system")]
            public SkyboxConfig Skybox;
            [Tooltip("Set to true if you want to spawn here after dying, not Timber Hearth. You can still warp back to the main star system.")]
            public bool StartHere;
            [Tooltip("Set to true if you want the player to stay in this star system if they die in it.")]
            public bool RespawnHere;
            [Tooltip("The music to play while flying between planets")]
            public AudioClip TravelAudio;
            [Tooltip("The music to play while flying between planets, if not using a custom audio clip")]
            [EnumValuePicker]
            [ConditionalField(nameof(TravelAudio), (AudioClip)null)]
            public AudioType TravelAudioType;
            [Tooltip("Whether you can warp to this system with the vessel")]
            public bool HasWarpCoordinates;
            [Tooltip("Settings for the vessel")]
            [ConditionalField(nameof(HasWarpCoordinates))]
            public VesselConfig Vessel;
        }

        [Serializable]
        public class SkyboxConfig
        {
            [Tooltip("Whether to destroy the star field around the player")]
            public bool DestroyStarField;
            [Tooltip("Whether to use a cube for the skybox instead of a smooth sphere")]
            public bool UseCube;
            [Tooltip("Texture to use for the skybox's positive X direction")]
            public Texture2D Right;
            [Tooltip("Texture to use for the skybox's negative X direction")]
            public Texture2D Left;
            [Tooltip("Texture to use for the skybox's positive Y direction")]
            public Texture2D Top;
            [Tooltip("Texture to use for the skybox's negative Y direction")]
            public Texture2D Bottom;
            [Tooltip("Texture to use for the skybox's positive Z direction")]
            public Texture2D Front;
            [Tooltip("Texture to use for the skybox's negative Z direction")]
            public Texture2D Back;

            public bool HasCustomSkybox => Right || Left || Top || Bottom || Front || Back;
            public bool IsCustomSkyboxValid() => Right && Left && Top && Bottom && Front && Back;
        }

        [Serializable]
        public class VesselConfig
        {
            [Tooltip("The position in the solar system the vessel will warp to.")]
            public Vector3 VesselPosition;
            [Tooltip("Euler angles by which the vessel will be oriented.")]
            public Vector3 VesselRotation;
            [Tooltip("The relative position to the vessel that you will be teleported to when you exit the vessel through the black hole.")]
            public Vector3 WarpExitPosition;
            [Tooltip("Euler angles by which the warp exit will be oriented.")]
            public Vector3 WarpExitRotation;
            [Tooltip("A ship log fact which will make a prompt appear showing the coordinates when you're in the Vessel.")]
            public FactBase PromptFact;
            [Tooltip("The warp coordinates to use with the vessel")]
            public Coordinates WarpCoordinates;
        }

        [Serializable]
        public class Coordinates
        {
            public int[] x;
            public int[] y;
            public int[] z;

            public bool IsValid() =>
                IsCoordinateValid(x) && IsCoordinateValid(y) && IsCoordinateValid(z);

            private bool IsCoordinateValid(int[] arr) =>
                arr != null && arr.Length >= 2 && arr.Length < 7 &&
                arr.Distinct().Count() == arr.Count() &&
                arr.All(n => n >= 0 && n < 6);
        }
    }
}
