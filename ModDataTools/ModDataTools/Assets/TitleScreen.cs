using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    public class TitleScreenConfig : IJsonSerializable
    {
        readonly IEnumerable<TitleScreenAsset> titleScreens;

        public TitleScreenConfig(IEnumerable<TitleScreenAsset> titleScreens)
        {
            this.titleScreens = titleScreens;
        }

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("$schema", "https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/title_screen_schema.json");
            writer.WriteProperty("titleScreens", titleScreens);
            writer.WriteEndObject();
        }
    }

    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(TitleScreenAsset))]
    public class TitleScreenAsset : DataAsset, IValidateableAsset, IJsonSerializable
    {
        [Tooltip("The mod this asset belongs to")]
        public ModManifestAsset Mod;
        [Tooltip("The order of this title screen relative to the mod's other title screens. The last title screen with its display conditions met will be displayed, so higher priorities take precedence.")]
        public int Priority;
        [Header("Data")]
        [Tooltip("Colour of the text on the main menu")]
        public NullishColor MenuTextTint;
        [Tooltip("Ship log fact required for this title screen to appear.")]
        public FactAsset FactRequiredForTitle;
        [Tooltip("Persistent condition required for this title screen to appear.")]
        public ConditionAsset PersistentConditionRequiredForTitle;
        [Tooltip("If set to true, NH generated planets will not show on the title screen. If false, this title screen has the same chance as other NH planet title screens to show.")]
        public bool DisableNHPlanets = true;
        [Tooltip("If set to true, this custom title screen will merge with all other custom title screens with this set to true. If false, NH will randomly select between this and other valid title screens that are loaded.")]
        public bool ShareTitleScreen = true;
        [Tooltip("Customize the skybox for this title screen")]
        public StarSystemAsset.SkyboxConfig Skybox;
        [Tooltip("The music audio that will play on the title screen.")]
        public AudioClip Music;
        [Tooltip("The music audio that will play on the title screen, if not using a custom audio clip.")]
        [ConditionalField(nameof(Music), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType MusicType;
        [Tooltip("How long (in seconds) it should take for the music to fade in from 0 to the configured volume. 0 disables fade-in and music will be at full volume immediately. Vanilla is 8.")]
        public float MusicFadeInTime = 8f;
        [Tooltip("Volume multiplier for the music (0.0 - 1.0). Vanilla is 0.1.")]
        [Range(0f, 1f)]
        public float MusicVolume = 0.1f;
        [Tooltip("The ambience audio that will play on the title screen.")]
        public AudioClip Ambience;
        [Tooltip("The ambience audio that will play on the title screen, if not using a custom audio clip.")]
        [ConditionalField(nameof(Ambience), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType AmbienceType;
        [Tooltip("Volume multiplier for the ambience (0.0 - 1.0). Vanilla is 0.7.")]
        [Range(0f, 1f)]
        public float AmbienceVolume = 0.7f;
        [Tooltip("Edit properties of the background")]
        public BackgroundConfig Background;
        [Tooltip("Edit properties of the main menu planet")]
        public MenuPlanetConfig MenuPlanet;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Mod) yield return Mod;
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (PersistentConditionRequiredForTitle && !PersistentConditionRequiredForTitle.Persistent)
                validator.Error(this, $"{nameof(PersistentConditionRequiredForTitle)} must be a persistent condition");
            if (Skybox.HasCustomSkybox && !Skybox.IsCustomSkyboxValid())
                validator.Error(this, $"Missing some skybox images");
        }

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("menuTextTint", MenuTextTint);
            if (FactRequiredForTitle)
                writer.WriteProperty("factRequiredForTitle", FactRequiredForTitle.FullID);
            if (PersistentConditionRequiredForTitle)
                writer.WriteProperty("persistentConditionRequiredForTitle", PersistentConditionRequiredForTitle.FullID);
            if (!DisableNHPlanets)
                writer.WriteProperty("disableNHPlanets", DisableNHPlanets);
            if (!ShareTitleScreen)
                writer.WriteProperty("shareTitleScreen", ShareTitleScreen);
            if (Skybox.DestroyStarField || Skybox.HasCustomSkybox)
            {
                writer.WritePropertyName("Skybox");
                writer.WriteStartObject();
                if (Skybox.DestroyStarField)
                    writer.WriteProperty("destroyStarField", Skybox.DestroyStarField);
                if (Skybox.HasCustomSkybox)
                {
                    if (Skybox.UseCube)
                        writer.WriteProperty("useCube", Skybox.UseCube);
                    writer.WriteProperty("rightPath", GetResourcePath(Skybox.Right));
                    writer.WriteProperty("leftPath", GetResourcePath(Skybox.Left));
                    writer.WriteProperty("topPath", GetResourcePath(Skybox.Top));
                    writer.WriteProperty("bottomPath", GetResourcePath(Skybox.Bottom));
                    writer.WriteProperty("frontPath", GetResourcePath(Skybox.Front));
                    writer.WriteProperty("backPath", GetResourcePath(Skybox.Back));
                }
                writer.WriteEndObject();
            }
            if (Music)
                writer.WriteProperty("music", GetResourcePath(Music));
            else if (MusicType != AudioType.None)
                writer.WriteProperty("music", MusicType, false);
            if (MusicFadeInTime != 8f)
                writer.WriteProperty("musicFadeInTime", MusicFadeInTime);
            if (MusicVolume != 0.1f)
                writer.WriteProperty("musicVolume", MusicVolume);
            if (Ambience)
                writer.WriteProperty("ambience", GetResourcePath(Ambience));
            else if (AmbienceType != AudioType.None)
                writer.WriteProperty("ambience", AmbienceType, false);
            if (AmbienceVolume != 0.7f)
                writer.WriteProperty("ambienceVolume", AmbienceVolume);
            if (Background.RotationSpeed != 1f || Background.RemoveChildren.Any() || Background.Details.Any())
            {
                writer.WritePropertyName("Background");
                writer.WriteStartObject();
                if (Background.RotationSpeed != 1f)
                    writer.WriteProperty("rotationSpeed", Background.RotationSpeed);
                if (Background.RemoveChildren.Any())
                    writer.WriteProperty("removeChildren", Background.RemoveChildren);
                if (Background.Details.Any())
                    writer.WriteProperty("details", Background.Details);
                writer.WriteEndObject();
            }
            if (MenuPlanet.DestroyMenuPlanet || MenuPlanet.RotationSpeed != 2f || MenuPlanet.RemoveChildren.Any() || MenuPlanet.Details.Any())
            {
                writer.WritePropertyName("MenuPlanet");
                writer.WriteStartObject();
                if (MenuPlanet.DestroyMenuPlanet)
                    writer.WriteProperty("destroyMenuPlanet", MenuPlanet.DestroyMenuPlanet);
                if (MenuPlanet.RotationSpeed != 2f)
                    writer.WriteProperty("rotationSpeed", MenuPlanet.RotationSpeed);
                if (MenuPlanet.RemoveChildren.Any())
                    writer.WriteProperty("removeChildren", MenuPlanet.RemoveChildren);
                if (MenuPlanet.Details.Any())
                    writer.WriteProperty("details", MenuPlanet.Details);
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }

        public static string GetResourcePath(UnityEngine.Object resource) => $"title-screen/{AssetRepository.GetAssetFileName(resource)}";

        public override IEnumerable<AssetResource> GetResources()
        {
            if (Skybox.HasCustomSkybox)
            {
                foreach (var texture in new[] { Skybox.Right, Skybox.Left, Skybox.Top, Skybox.Bottom, Skybox.Front, Skybox.Back })
                    if (texture)
                        yield return new ImageResource(texture, GetResourcePath(texture));
            }
            if (Music)
                yield return new AudioResource(Music, GetResourcePath(Music));
            if (Ambience)
                yield return new AudioResource(Ambience, GetResourcePath(Ambience));
            foreach (var detail in Background.Details.Concat(MenuPlanet.Details).Where(d => d.Prefab))
                yield return new PrefabResource(detail.Prefab, string.Empty);
        }

        [Serializable]
        public class BackgroundConfig
        {
            [Tooltip("Changes the speed the background rotates (and by extension the main menu planet). This is in degrees per second.")]
            public float RotationSpeed = 1f;
            [Tooltip("Disables the renderers of objects at the provided paths")]
            public List<string> RemoveChildren = new();
            [Tooltip("A list of details to populate the background with.")]
            public List<TitleScreenDetailConfig> Details = new();
        }

        [Serializable]
        public class MenuPlanetConfig
        {
            [Tooltip("Disables the renderers of the main menu planet and all objects on it (this is to improve compatibility with other mods that don't use the NH title screen json).")]
            public bool DestroyMenuPlanet;
            [Tooltip("Changes the speed the main menu planet rotates. This is in degrees per second.")]
            public float RotationSpeed = 2f;
            [Tooltip("Disables the renderers of objects at the provided paths")]
            public List<string> RemoveChildren = new();
            [Tooltip("A list of details to populate the main menu planet with.")]
            public List<TitleScreenDetailConfig> Details = new();
        }

        [Serializable]
        public class TitleScreenDetailConfig : IJsonSerializable
        {
            [Tooltip("The prefab to spawn, if spawning a custom object")]
            public GameObject Prefab;
            [Tooltip("The path in the scene hierarchy of the item to copy")]
            [ConditionalField(nameof(Prefab), (GameObject)null)]
            public string Path;
            [Tooltip("An optional rename of the detail")]
            public string Rename;
            [Tooltip("The path of the parent of this game object. Optional (will default to the root).")]
            public string ParentPath;
            [Tooltip("Whether the positional and rotational coordinates are relative to parent instead of the root.")]
            public bool IsRelativeToParent;
            [Tooltip("Position of this detail")]
            public Vector3 Position;
            [Tooltip("Rotation of this detail")]
            public Vector3 Rotation;
            [Tooltip("Scale the detail")]
            public float Scale = 1f;
            [Tooltip("Scale each axis of the detail. Multiplied with scale.")]
            public Vector3 Stretch = Vector3.one;
            [Tooltip("A list of children to remove from this detail")]
            public List<string> RemoveChildren = new();
            [Tooltip("Do we reset all the components on this object? Useful for certain props that have dialogue components attached to them.")]
            public bool RemoveComponents;

            public void ToJson(JsonTextWriter writer)
            {
                writer.WriteStartObject();
                if (Prefab)
                {
                    writer.WriteProperty("assetBundle", AssetRepository.GetAssetBundlePath(Prefab));
                    writer.WriteProperty("path", AssetRepository.GetAssetPath(Prefab));
                }
                else
                    writer.WriteProperty("path", Path);
                if (!string.IsNullOrEmpty(Rename))
                    writer.WriteProperty("rename", Rename);
                if (!string.IsNullOrEmpty(ParentPath))
                    writer.WriteProperty("parentPath", ParentPath);
                if (IsRelativeToParent)
                    writer.WriteProperty("isRelativeToParent", IsRelativeToParent);
                writer.WriteProperty("position", Position);
                writer.WriteProperty("rotation", Rotation);
                if (Stretch == Vector3.one)
                {
                    if (Scale != 1f)
                        writer.WriteProperty("scale", Scale);
                }
                else
                    writer.WriteProperty("stretch", Stretch * Scale);
                if (RemoveChildren.Any())
                    writer.WriteProperty("removeChildren", RemoveChildren);
                if (RemoveComponents)
                    writer.WriteProperty("removeComponents", RemoveComponents);
                writer.WriteEndObject();
            }
        }
    }
}
