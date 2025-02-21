using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.PlanetModules
{
    [Serializable]
    public class ShipLogModule : PlanetModule
    {
        [Tooltip("Completely remove this planet (and it's children) from map mode.")]
        public bool Remove;
        [Tooltip("Can this ship log map mode entry be selected. Ex) Set to false for stars with no entries on them so they are skipped in navigation")]
        public bool Selectable = true;
        [Tooltip("The path to the sprite to show when the planet is unexplored in map mode.")]
        public Texture2D OutlineSprite;
        [Tooltip("The path to the sprite to show when the planet is revealed in map mode.")]
        public Texture2D RevealedSprite;
        [Tooltip("Hide the planet completely if unexplored instead of showing an outline.")]
        public bool InvisibleWhenHidden;
        [Tooltip("Whether to position this planet automatically in map mode.")]
        public bool AutoPosition = true;
        [Tooltip("Specify where this planet is in terms of navigation.")]
        [ConditionalField(nameof(AutoPosition), false)]
        public Vector2Int ManualNavigationPosition;
        [Tooltip("Manually place this planet at the specified position.")]
        [ConditionalField(nameof(AutoPosition), false)]
        public Vector2 ManualPosition;
        [Tooltip("Extra distance to apply to this object in map mode.")]
        [ConditionalField(nameof(AutoPosition))]
        public float Offset;
        [Tooltip("Scale to apply to the planet in map mode.")]
        public float Scale = 1f;
        [Tooltip("Place non-selectable objects in map mode (like sand funnels).")]
        public List<DetailsSubModule> Details = new();

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WritePropertyName("mapMode");
            writer.WriteStartObject();
            if (Remove)
            {
                writer.WriteProperty("remove", Remove);
            }
            else
            {
                writer.WriteProperty("details", Details, planet);
                if (InvisibleWhenHidden)
                    writer.WriteProperty("invisibleWhenHidden", InvisibleWhenHidden);
                if (AutoPosition)
                {
                    if (Offset != 0f)
                        writer.WriteProperty("offset", Offset);
                }
                else
                {
                    writer.WriteProperty("manualNavigationPosition", ManualNavigationPosition);
                    writer.WriteProperty("manualPosition", ManualPosition);
                }
                if (OutlineSprite)
                    writer.WriteProperty("outlineSprite", planet.GetResourcePath(OutlineSprite));
                if (RevealedSprite)
                    writer.WriteProperty("revealedSprite", planet.GetResourcePath(RevealedSprite));
                if (Scale != 1f)
                    writer.WriteProperty("scale", Scale);
                if (!Selectable)
                    writer.WriteProperty("selectable", Selectable);
            }
            writer.WriteEndObject();

            writer.WriteProperty("spriteFolder", planet.GetShipLogPhotoPath());
            writer.WriteProperty("xmlFile", planet.GetShipLogFilePath());
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            if (!Remove)
            {
                foreach (var detail in Details)
                    foreach (var resource in detail.GetResources(planet))
                        yield return resource;
                if (OutlineSprite)
                    yield return new ImageResource(OutlineSprite, planet);
                if (RevealedSprite)
                    yield return new ImageResource(RevealedSprite, planet);
            }
        }

        [Serializable]
        public class DetailsSubModule : PlanetModule
        {
            [Tooltip("The sprite to show when the parent AstroBody is rumored/unexplored.")]
            public Texture2D OutlineSprite;
            [Tooltip("The sprite to show when the parent AstroBody is revealed.")]
            public Texture2D RevealedSprite;
            [Tooltip("Whether to completely hide this detail when the parent AstroBody is unexplored.")]
            public bool InvisibleWhenHidden;
            [Tooltip("The position (relative to the parent) to place the detail.")]
            public Vector2 Position;
            [Tooltip("The angle in degrees to rotate the detail.")]
            public float Rotation;
            [Tooltip("The amount to scale the x and y-axis of the detail by.")]
            public Vector2 Scale = Vector2.one;

            public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
            {
                if (InvisibleWhenHidden)
                    writer.WriteProperty("invisibleWhenHidden", InvisibleWhenHidden);
                if (OutlineSprite)
                    writer.WriteProperty("outlineSprite", planet.GetResourcePath(OutlineSprite));
                if (Position != Vector2.zero)
                    writer.WriteProperty("position", Position);
                if (RevealedSprite)
                    writer.WriteProperty("revealedSprite", planet.GetResourcePath(RevealedSprite));
                if (Rotation != 0f)
                    writer.WriteProperty("rotation", Rotation);
                if (Scale != Vector2.one)
                    writer.WriteProperty("scale", Scale);
            }

            public override bool ShouldWrite(PlanetAsset planet) => OutlineSprite || RevealedSprite;

            public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
            {
                if (OutlineSprite)
                    yield return new ImageResource(OutlineSprite, planet);
                if (RevealedSprite)
                    yield return new ImageResource(RevealedSprite, planet);
            }
        }
    }
}
