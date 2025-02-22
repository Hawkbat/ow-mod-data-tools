﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModDataTools.Assets.Props;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using ModDataTools.Assets.Resources;
using System.EnterpriseServices.CompensatingResourceManager;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(StarSystemAsset))]
    public class StarSystemAsset : DataAsset, IValidateableAsset, IJsonSerializable
    {
        [Tooltip("The mod this asset belongs to")]
        public ModManifestAsset Mod;
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

        public override string GetIDPrefix()
        {
            if (Mod) return $"{Mod.Author}.";
            return string.Empty;
        }

        public override string GetChildIDPrefix()
        {
            if (!string.IsNullOrEmpty(ChildIDPrefix))
                return ChildIDPrefix + "_";
            return string.Empty;
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (NewHorizons.Vessel.HasWarpCoordinates && !NewHorizons.Vessel.WarpCoordinates.IsValid())
                validator.Error(this, $"Invalid warp coordinates");
            if (NewHorizons.Skybox.HasCustomSkybox && !NewHorizons.Skybox.IsCustomSkyboxValid())
                validator.Error(this, $"Missing some skybox images");
            if (NewHorizons.ConditionalChecks.Any())
            {
                foreach (var check in NewHorizons.ConditionalChecks)
                {
                    check.Validate(this, validator);
                }
            }
        }

        public void ToJson(JsonTextWriter writer)
        {
            var nh = NewHorizons;
            writer.WriteStartObject();
            writer.WriteProperty("$schema", "https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/star_system_schema.json");
            writer.WriteProperty("name", FullID);
            writer.WriteProperty("allowOutsideItems", nh.AllowOutsideItems);
            writer.WriteProperty("freeMapAngle", nh.FreeMapAngle);
            writer.WriteProperty("returnToSolarSystemWhenTooFar", nh.ReturnToSolarSystemWhenTooFar);
            writer.WriteProperty("farClipPlaneOverride", nh.FarClipPlaneOverride);
            writer.WriteProperty("canEnterViaWarpDrive", nh.CanEnterViaWarpDrive);
            if (nh.CanEnterViaWarpDrive && nh.FactRequiredForWarp)
                writer.WriteProperty("factRequiredForWarp", nh.FactRequiredForWarp.FullID);
            writer.WriteProperty("canExitViaWarpDrive", nh.CanExitViaWarpDrive);
            if (nh.CanExitViaWarpDrive && nh.FactRequiredForWarp)
                writer.WriteProperty("factRequiredToExitViaWarpDrive", nh.FactRequiredToExitViaWarpDrive.FullID);
            writer.WriteProperty("destroyStockPlanets", nh.DestroyStockPlanets);
            writer.WriteProperty("enableTimeLoop", nh.EnableTimeLoop);
            if (nh.EnableTimeLoop)
                writer.WriteProperty("loopDuration", nh.LoopDuration);
            writer.WriteProperty("mapRestricted", nh.MapRestricted);
            writer.WritePropertyName("Skybox");
            writer.WriteStartObject();
            writer.WriteProperty("destroyStarField", nh.Skybox.DestroyStarField);
            if (nh.Skybox.HasCustomSkybox)
            {
                writer.WriteProperty("useCube", nh.Skybox.UseCube);
                writer.WriteProperty("rightPath", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.Skybox.Right)}");
                writer.WriteProperty("leftPath", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.Skybox.Left)}");
                writer.WriteProperty("topPath", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.Skybox.Top)}");
                writer.WriteProperty("bottomPath", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.Skybox.Bottom)}");
                writer.WriteProperty("frontPath", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.Skybox.Front)}");
                writer.WriteProperty("backPath", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.Skybox.Back)}");
            }
            writer.WriteEndObject();
            writer.WriteProperty("startHere", nh.StartHere);
            writer.WriteProperty("respawnHere", nh.RespawnHere);
            if (nh.GlobalMusic.HasCustomAudio)
            {
                writer.WritePropertyName("GlobalMusic");
                writer.WriteStartObject();
                if (nh.GlobalMusic.TravelAudio)
                    writer.WriteProperty("travelAudio", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.GlobalMusic.TravelAudio)}");
                else if (nh.GlobalMusic.TravelAudioType != AudioType.None)
                    writer.WriteProperty("travelAudio", nh.GlobalMusic.TravelAudioType, false);
                if (nh.GlobalMusic.EndTimesAudio)
                    writer.WriteProperty("endTimesAudio", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.GlobalMusic.EndTimesAudio)}");
                else if (nh.GlobalMusic.EndTimesAudioType != AudioType.None)
                    writer.WriteProperty("endTimesAudio", nh.GlobalMusic.EndTimesAudioType, false);
                if (nh.GlobalMusic.EndTimesDreamAudio)
                    writer.WriteProperty("endTimesDreamAudio", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.GlobalMusic.EndTimesDreamAudio)}");
                else if (nh.GlobalMusic.EndTimesDreamAudioType != AudioType.None)
                    writer.WriteProperty("endTimesDreamAudio", nh.GlobalMusic.EndTimesDreamAudioType, false);
                if (nh.GlobalMusic.BrambleDimensionAudio)
                    writer.WriteProperty("brambleDimensionAudio", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.GlobalMusic.BrambleDimensionAudio)}");
                else if (nh.GlobalMusic.BrambleDimensionAudioType != AudioType.None)
                    writer.WriteProperty("brambleDimensionAudio", nh.GlobalMusic.BrambleDimensionAudioType, false);
                if (nh.GlobalMusic.FinalEndTimesIntroAudio)
                    writer.WriteProperty("finalEndTimesIntroAudio", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.GlobalMusic.FinalEndTimesIntroAudio)}");
                else if (nh.GlobalMusic.FinalEndTimesIntroAudioType != AudioType.None)
                    writer.WriteProperty("finalEndTimesIntroAudio", nh.GlobalMusic.FinalEndTimesIntroAudioType, false);
                if (nh.GlobalMusic.FinalEndTimesLoopAudio)
                    writer.WriteProperty("finalEndTimesLoopAudio", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.GlobalMusic.FinalEndTimesLoopAudio)}");
                else if (nh.GlobalMusic.FinalEndTimesLoopAudioType != AudioType.None)
                    writer.WriteProperty("finalEndTimesLoopAudio", nh.GlobalMusic.FinalEndTimesLoopAudioType, false);
                if (nh.GlobalMusic.FinalEndTimesBrambleAudio)
                    writer.WriteProperty("finalEndTimesBrambleAudio", $"systems/{FullID}/{AssetRepository.GetAssetFileName(nh.GlobalMusic.FinalEndTimesBrambleAudio)}");
                else if (nh.GlobalMusic.FinalEndTimesBrambleAudioType != AudioType.None)
                    writer.WriteProperty("finalEndTimesBrambleAudio", nh.GlobalMusic.FinalEndTimesBrambleAudioType, false);
                writer.WriteEndObject();
            }
            if (nh.ConditionalChecks.Any())
                writer.WriteProperty("conditionalChecks", nh.ConditionalChecks);

            var vesselProp = AssetRepository.GetAllProps<VesselPropData>().FirstOrDefault(c => c.Planet && c.Planet.StarSystem == this);
            var warpExitProp = AssetRepository.GetAllProps<VesselWarpExitPropData>().FirstOrDefault(c => c.Planet && c.Planet.StarSystem == this);
            if (nh.Vessel.HasWarpCoordinates || nh.Vessel.VesselPosition.HasValue || nh.Vessel.WarpExitPosition.HasValue || vesselProp != null && warpExitProp != null)
            {
                writer.WritePropertyName("Vessel");
                writer.WriteStartObject();
                if (nh.Vessel.HasWarpCoordinates)
                {
                    writer.WritePropertyName("coords");
                    writer.WriteStartObject();
                    writer.WriteProperty("x", nh.Vessel.WarpCoordinates.x);
                    writer.WriteProperty("y", nh.Vessel.WarpCoordinates.y);
                    writer.WriteProperty("z", nh.Vessel.WarpCoordinates.z);
                    writer.WriteEndObject();
                    if (nh.Vessel.PromptFact)
                        writer.WriteProperty("promptFact", nh.Vessel.PromptFact.FullID);
                }
                if (nh.Vessel.VesselPosition.HasValue)
                {
                    writer.WritePropertyName("vesselSpawn");
                    writer.WriteStartObject();
                    writer.WriteProperty("position", nh.Vessel.VesselPosition);
                    writer.WriteProperty("rotation", nh.Vessel.VesselRotation);
                    writer.WriteEndObject();
                    if (nh.Vessel.AlwaysPresent)
                        writer.WriteProperty("alwaysPresent", nh.Vessel.AlwaysPresent);
                    if (nh.Vessel.SpawnOnVessel)
                        writer.WriteProperty("spawnOnVessel", nh.Vessel.SpawnOnVessel);
                    if (!nh.Vessel.HasPhysics)
                        writer.WriteProperty("hasPhysics", nh.Vessel.HasPhysics);
                    if (!nh.Vessel.HasZeroGravityVolume)
                        writer.WriteProperty("hasZeroGravityVolume", nh.Vessel.HasZeroGravityVolume);
                }
                else
                {
                    if (vesselProp != null)
                    {
                        writer.WriteProperty("vesselSpawn", vesselProp);
                        var vesselPropData = vesselProp.Prop.GetData() as VesselPropData;
                        if (nh.Vessel.AlwaysPresent)
                            writer.WriteProperty("alwaysPresent", vesselPropData.AlwaysPresent);
                        if (nh.Vessel.SpawnOnVessel)
                            writer.WriteProperty("spawnOnVessel", vesselPropData.SpawnOnVessel);
                        if (nh.Vessel.HasPhysics)
                            writer.WriteProperty("hasPhysics", vesselPropData.HasPhysics);
                        if (nh.Vessel.HasZeroGravityVolume)
                            writer.WriteProperty("hasZeroGravityVolume", vesselPropData.HasZeroGravityVolume);
                    }
                }
                if (nh.Vessel.WarpExitPosition.HasValue)
                {
                    writer.WritePropertyName("warpExit");
                    writer.WriteStartObject();
                    writer.WriteProperty("position", nh.Vessel.WarpExitPosition);
                    writer.WriteProperty("rotation", nh.Vessel.WarpExitRotation);
                    writer.WriteProperty("attachToVessel", true);
                    writer.WriteEndObject();
                }
                else
                {
                    if (warpExitProp != null)
                        writer.WriteProperty("warpExit", warpExitProp);
                }
                writer.WriteEndObject();
            }
            var entries = AssetRepository.GetAllAssets<EntryAsset>().Where(e => e.Planet && e.Planet.StarSystem == this);
            if (entries.Any())
            {
                writer.WritePropertyName("entryPositions");
                writer.WriteStartArray();
                foreach (var entry in entries)
                {
                    writer.WriteStartObject();
                    writer.WriteProperty("id", entry.FullID);
                    writer.WriteProperty("position", entry.RumorModePosition);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            var initialFacts = AssetRepository.GetAllAssets<FactAsset>()
                .Where(f => f.InitiallyRevealed && f.Entry && f.Entry.Planet && f.Entry.Planet.StarSystem == this);
            if (initialFacts.Any())
                writer.WriteProperty("initialReveal", initialFacts.Select(f => f.FullID));
            var curiosities = AssetRepository.GetAllAssets<EntryAsset>().Where(e => e.IsCuriosity && e.Planet && e.Planet.StarSystem == this);
            if (curiosities.Any())
            {
                writer.WritePropertyName("curiosities");
                writer.WriteStartArray();
                foreach (var curiosity in curiosities)
                {
                    writer.WriteStartObject();
                    writer.WriteProperty("color", (Color32)curiosity.NormalColor);
                    writer.WriteProperty("highlightColor", (Color32)curiosity.HighlightColor);
                    writer.WriteProperty("id", curiosity.FullID);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        public override void Localize(Localization l10n)
        {
            l10n.AddUI(FullID, FullName);
        }

        public override IEnumerable<AssetResource> GetResources()
        {
            if (ExportConfigFile)
            {
                if (OverrideConfigFile)
                    yield return new TextResource(OverrideConfigFile, $"systems/{FullID}.json");
                else
                    yield return new TextResource(ExportUtility.ToJsonString(this), $"systems/{FullID}.json");
            }
            if (NewHorizons.Skybox.HasCustomSkybox)
            {
                if (NewHorizons.Skybox.Right)
                    yield return new ImageResource(NewHorizons.Skybox.Right, this);
                if (NewHorizons.Skybox.Left)
                    yield return new ImageResource(NewHorizons.Skybox.Left, this);
                if (NewHorizons.Skybox.Top)
                    yield return new ImageResource(NewHorizons.Skybox.Top, this);
                if (NewHorizons.Skybox.Bottom)
                    yield return new ImageResource(NewHorizons.Skybox.Bottom, this);
                if (NewHorizons.Skybox.Front)
                    yield return new ImageResource(NewHorizons.Skybox.Front, this);
                if (NewHorizons.Skybox.Back)
                    yield return new ImageResource(NewHorizons.Skybox.Back, this);
            }
            if (NewHorizons.GlobalMusic.HasCustomAudio)
            {
                if (NewHorizons.GlobalMusic.TravelAudio)
                    yield return new AudioResource(NewHorizons.GlobalMusic.TravelAudio, this);
                if (NewHorizons.GlobalMusic.EndTimesAudio)
                    yield return new AudioResource(NewHorizons.GlobalMusic.EndTimesAudio, this);
                if (NewHorizons.GlobalMusic.EndTimesDreamAudio)
                    yield return new AudioResource(NewHorizons.GlobalMusic.EndTimesDreamAudio, this);
                if (NewHorizons.GlobalMusic.BrambleDimensionAudio)
                    yield return new AudioResource(NewHorizons.GlobalMusic.BrambleDimensionAudio, this);
                if (NewHorizons.GlobalMusic.FinalEndTimesIntroAudio)
                    yield return new AudioResource(NewHorizons.GlobalMusic.FinalEndTimesIntroAudio, this);
                if (NewHorizons.GlobalMusic.FinalEndTimesLoopAudio)
                    yield return new AudioResource(NewHorizons.GlobalMusic.FinalEndTimesLoopAudio, this);
                if (NewHorizons.GlobalMusic.FinalEndTimesBrambleAudio)
                    yield return new AudioResource(NewHorizons.GlobalMusic.FinalEndTimesBrambleAudio, this);
            }
        }

        public string GetResourcePath(UnityEngine.Object resource) => $"systems/{FullID}/{AssetRepository.GetAssetFileName(resource)}";

        [Serializable]
        public class NewHorizonsConfig
        {
            [Tooltip("When changing star systems are you allowed to bring items into this system?")]
            public bool AllowOutsideItems = true;
            [Tooltip("In this system should the player be able to rotate their map camera freely or be stuck above the plane of the solar system?")]
            public bool FreeMapAngle;
            [Tooltip("When well past the furthest orbit, should the player be summoned back to the star?")]
            public bool ReturnToSolarSystemWhenTooFar;
            [Tooltip("An override value for the far clip plane. Allows you to see farther.")]
            public NullishSingle FarClipPlaneOverride;
            [Tooltip("Whether this system can be warped to via the warp drive. Does NOT effect the base SolarSystem. For that, see `canExitViaWarpDrive` and `factRequiredToExitViaWarpDrive`")]
            public bool CanEnterViaWarpDrive = true;
            [Tooltip("Set to the Fact that must be revealed before it can be warped to.")]
            [ConditionalField(nameof(CanEnterViaWarpDrive))]
            public FactAsset FactRequiredForWarp;
            [Tooltip("Can you use the warp drive to leave this system?")]
            public bool CanExitViaWarpDrive = true;
            [Tooltip("The FactID that must be revealed for you to warp back to the main solar system from here.")]
            public FactAsset FactRequiredToExitViaWarpDrive;
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
            [Tooltip("Replace music that plays globally")]
            public GlobalMusicConfig GlobalMusic;
            [Tooltip("Settings for the vessel")]
            public VesselConfig Vessel;
            [Tooltip("The planet to focus on when entering the ship log for the first time in a loop. If not set this will be the planet at navtigation position (1, 0)")]
            public PlanetAsset ShipLogStartingPlanet;
            [Tooltip("A list of conditional checks to be performed while in this star system.")]
            public List<ConditionalCheckConfig> ConditionalChecks;
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
        public class GlobalMusicConfig
        {
            [Tooltip("The audio that will play when travelling in space.")]
            public AudioClip TravelAudio;
            [Tooltip("The audio that will play when travelling in space, if not using a custom audio clip")]
            [ConditionalField(nameof(TravelAudio), (AudioClip)null)]
            [EnumValuePicker]
            public AudioType TravelAudioType;
            [Tooltip("The audio that will play right before the loop ends.")]
            public AudioClip EndTimesAudio;
            [Tooltip("The audio that will play right before the loop ends, if not using a custom audio clip")]
            [ConditionalField(nameof(EndTimesAudio), (AudioClip)null)]
            [EnumValuePicker]
            public AudioType EndTimesAudioType;
            [Tooltip("The audio that will play right before the loop ends while inside the dreamworld.")]
            public AudioClip EndTimesDreamAudio;
            [Tooltip("The audio that will play right before the loop ends while inside the dreamworld, if not using a custom audio clip")]
            [ConditionalField(nameof(EndTimesDreamAudio), (AudioClip)null)]
            [EnumValuePicker]
            public AudioType EndTimesDreamAudioType;
            [Tooltip("The audio that will play when travelling through a bramble dimension.")]
            public AudioClip BrambleDimensionAudio;
            [Tooltip("The audio that will play when travelling through a bramble dimension, if not using a custom audio clip")]
            [ConditionalField(nameof(BrambleDimensionAudio), (AudioClip)null)]
            [EnumValuePicker]
            public AudioType BrambleDimensionAudioType;
            [Tooltip("The audio that will play when you leave the ash twin project after taking out the advanced warp core.")]
            public AudioClip FinalEndTimesIntroAudio;
            [Tooltip("The audio that will play when you leave the ash twin project after taking out the advanced warp core, if not using a custom audio clip")]
            [ConditionalField(nameof(FinalEndTimesIntroAudio), (AudioClip)null)]
            [EnumValuePicker]
            public AudioType FinalEndTimesIntroAudioType;
            [Tooltip("The audio that will loop after the final end times intro.")]
            public AudioClip FinalEndTimesLoopAudio;
            [Tooltip("The audio that will loop after the final end times intro, if not using a custom audio clip")]
            [ConditionalField(nameof(FinalEndTimesLoopAudio), (AudioClip)null)]
            [EnumValuePicker]
            public AudioType FinalEndTimesLoopAudioType;
            [Tooltip("The audio that will loop after the final end times intro while inside a bramble dimension.")]
            public AudioClip FinalEndTimesBrambleAudio;
            [Tooltip("The audio that will loop after the final end times intro while inside a bramble dimension, if not using a custom audio clip")]
            [ConditionalField(nameof(FinalEndTimesBrambleAudio), (AudioClip)null)]
            [EnumValuePicker]
            public AudioType FinalEndTimesBrambleAudioType;

            public bool HasCustomAudio =>
                TravelAudio || TravelAudioType != AudioType.None
                || EndTimesAudio || EndTimesAudioType != AudioType.None
                || EndTimesDreamAudio || EndTimesDreamAudioType != AudioType.None
                || BrambleDimensionAudio || BrambleDimensionAudioType != AudioType.None
                || FinalEndTimesIntroAudio || FinalEndTimesIntroAudioType != AudioType.None
                || FinalEndTimesLoopAudio || FinalEndTimesLoopAudioType != AudioType.None
                || FinalEndTimesBrambleAudio || FinalEndTimesBrambleAudioType != AudioType.None;
        }

        [Serializable]
        public class VesselConfig
        {
            [Tooltip("The position in the solar system the vessel will warp to.")]
            public NullishVector3 VesselPosition;
            [Tooltip("Euler angles by which the vessel will be oriented.")]
            [ConditionalField(nameof(VesselPosition))]
            public NullishVector3 VesselRotation;

            [Tooltip("Whether the vessel should spawn in this system even if it wasn't used to warp to it. This will automatically power on the vessel.")]
            public bool AlwaysPresent;
            [Tooltip("Whether to always spawn the player on the vessel, even if it wasn't used to warp to the system.")]
            public bool SpawnOnVessel;
            [Tooltip("Whether the vessel should have physics enabled. Defaults to false if parentBody is set, and true otherwise.")]
            public bool HasPhysics;
            [Tooltip("Whether the vessel should have a zero-gravity volume around it. Defaults to false if parentBody is set, and true otherwise.")]
            public bool HasZeroGravityVolume;

            [Tooltip("The relative position to the vessel that you will be teleported to when you exit the vessel through the black hole.")]
            public NullishVector3 WarpExitPosition;
            [Tooltip("Euler angles by which the warp exit will be oriented.")]
            [ConditionalField(nameof(WarpExitPosition))]
            public NullishVector3 WarpExitRotation;

            [Tooltip("Whether you can warp to this system with the vessel")]
            public bool HasWarpCoordinates;
            [Tooltip("A ship log fact which will make a prompt appear showing the coordinates when you're in the Vessel.")]
            [ConditionalField(nameof(HasWarpCoordinates))]
            public FactAsset PromptFact;
            [Tooltip("The warp coordinates to use with the vessel")]
            [ConditionalField(nameof(HasWarpCoordinates))]
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

        [Serializable]
        public class ConditionalCheckConfig : IJsonSerializable
        {
            [Tooltip("The conditions that must be met for the check to pass.")]
            public ConditionalCheckConditionsConfig Check;
            [Tooltip("The effects of the check if it passes.")]
            public ConditionalCheckEffectsConfig Then;

            public void ToJson(JsonTextWriter writer)
            {
                writer.WriteStartObject();
                writer.WriteProperty("check", Check);
                writer.WriteProperty("then", Then);
                writer.WriteEndObject();
            }

            public void Validate(DataAsset asset, IAssetValidator validator)
            {
                Check.Validate(asset, validator);
                Then.Validate(asset, validator);
            }
        }

        [Serializable]
        public class ConditionalCheckConditionsConfig : IJsonSerializable
        {
            public List<ConditionAsset> AllConditionsSet;
            public List<ConditionAsset> AnyConditionsSet;
            public List<ConditionAsset> AllPersistentConditionsSet;
            public List<ConditionAsset> AnyPersistentConditionsSet;
            public List<FactAsset> AllFactsRevealed;
            public List<FactAsset> AnyFactsRevealed;
            public bool Invert;

            public void ToJson(JsonTextWriter writer)
            {
                writer.WriteStartObject();
                if (AllConditionsSet.Any())
                    writer.WriteProperty("allConditionsSet", AllConditionsSet.Select(c => c.FullID));
                if (AnyConditionsSet.Any())
                    writer.WriteProperty("anyConditionsSet", AnyConditionsSet.Select(c => c.FullID));
                if (AllPersistentConditionsSet.Any())
                    writer.WriteProperty("allPersistentConditionsSet", AllPersistentConditionsSet.Select(c => c.FullID));
                if (AnyPersistentConditionsSet.Any())
                    writer.WriteProperty("anyPersistentConditionsSet", AnyPersistentConditionsSet.Select(c => c.FullID));
                if (AllFactsRevealed.Any())
                    writer.WriteProperty("allFactsRevealed", AllFactsRevealed.Select(f => f.FullID));
                if (AnyFactsRevealed.Any())
                    writer.WriteProperty("anyFactsRevealed", AnyFactsRevealed.Select(f => f.FullID));
                if (Invert)
                    writer.WriteProperty("invert", Invert);
                writer.WriteEndObject();
            }

            public void Validate(DataAsset asset, IAssetValidator validator)
            {
                foreach (var condition in AnyConditionsSet)
                {
                    if (condition.Persistent)
                        validator.Error(asset, $"Conditional checks cannot have persistent conditions in {nameof(AnyConditionsSet)} (found '{condition.FullID}')");
                }
                foreach (var condition in AllConditionsSet)
                {
                    if (condition.Persistent)
                        validator.Error(asset, $"Conditional checks cannot have persistent conditions in {nameof(AllConditionsSet)} (found '{condition.FullID}')");
                }
                foreach (var condition in AnyPersistentConditionsSet)
                {
                    if (!condition.Persistent)
                        validator.Error(asset, $"Conditional checks cannot have non-persistent conditions in {nameof(AnyPersistentConditionsSet)} (found '{condition.FullID}')");
                }
                foreach (var condition in AllPersistentConditionsSet)
                {
                    if (!condition.Persistent)
                        validator.Error(asset, $"Conditional checks cannot have non-persistent conditions in {nameof(AllPersistentConditionsSet)} (found '{condition.FullID}')");
                }
            }
        }

        [Serializable]
        public class ConditionalCheckEffectsConfig : IJsonSerializable
        {
            public List<ConditionAsset> SetConditions;
            public List<ConditionAsset> UnsetConditions;
            public List<ConditionAsset> SetPersistentConditions;
            public List<ConditionAsset> UnsetPersistentConditions;
            public List<FactAsset> RevealFacts;
            public bool Reversible;

            public void ToJson(JsonTextWriter writer)
            {
                writer.WriteStartObject();
                if (SetConditions.Any())
                    writer.WriteProperty("setConditions", SetConditions.Select(c => c.FullID));
                if (UnsetConditions.Any())
                    writer.WriteProperty("unsetConditions", UnsetConditions.Select(c => c.FullID));
                if (SetPersistentConditions.Any())
                    writer.WriteProperty("setPersistentConditions", SetPersistentConditions.Select(c => c.FullID));
                if (UnsetPersistentConditions.Any())
                    writer.WriteProperty("unsetPersistentConditions", UnsetPersistentConditions.Select(c => c.FullID));
                if (RevealFacts.Any())
                    writer.WriteProperty("revealFacts", RevealFacts.Select(f => f.FullID));
                if (Reversible)
                    writer.WriteProperty("reversible", Reversible);
                writer.WriteEndObject();
            }

            public void Validate(DataAsset asset, IAssetValidator validator)
            {
                foreach (var condition in SetConditions)
                {
                    if (condition.Persistent)
                        validator.Error(asset, $"Conditional checks cannot have persistent conditions in {nameof(SetConditions)} (found '{condition.FullID}')");
                }
                foreach (var condition in UnsetConditions)
                {
                    if (condition.Persistent)
                        validator.Error(asset, $"Conditional checks cannot have persistent conditions in {nameof(UnsetConditions)} (found '{condition.FullID}')");
                }
                foreach (var condition in SetPersistentConditions)
                {
                    if (!condition.Persistent)
                        validator.Error(asset, $"Conditional checks cannot have non-persistent conditions in {nameof(SetPersistentConditions)} (found '{condition.FullID}')");
                }
                foreach (var condition in UnsetPersistentConditions)
                {
                    if (!condition.Persistent)
                        validator.Error(asset, $"Conditional checks cannot have non-persistent conditions in {nameof(UnsetPersistentConditions)} (found '{condition.FullID}')");
                }
            }
        }
    }
}
