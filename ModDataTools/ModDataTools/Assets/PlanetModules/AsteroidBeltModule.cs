using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.PlanetModules
{
    [Serializable]
    public class AsteroidBeltModule : PlanetModule
    {
        [Tooltip("Amount of asteroids to create.")]
        [Range(-1f, 200f)]
        public int Amount = -1;
        [Tooltip("Maximum size of the asteroids.")]
        [Min(0f)]
        public float MaxSize = 50f;
        [Tooltip("Minimum size of the asteroids.")]
        [Min(0f)]
        public float MinSize = 20f;
        [Tooltip("Lowest distance from the planet asteroids can spawn")]
        [Min(0f)]
        public float InnerRadius;
        [Tooltip("Greatest distance from the planet asteroids can spawn")]
        [Min(0f)]
        public float OuterRadius;
        [Tooltip("Angle between the rings and the equatorial plane of the planet.")]
        public float Inclination;
        [Tooltip("Angle defining the point where the rings rise up from the planet's equatorial plane if inclination is nonzero.")]
        public float LongitudeOfAscendingNode;
        [Tooltip("Should the detail of the asteroid be randomly oriented, or should it point towards the center.")]
        public bool RandomOrientation = true;
        [Tooltip("Surface gravity of the asteroids.")]
        [Min(0f)]
        public float Gravity = 1f;
        [Tooltip("The color of the generated asteroids")]
        public NullishColor ProcGenColor;
        [Tooltip("The scale of the generated asteroids")]
        public float ProcGenScale;
        [Tooltip("Number used to randomize asteroid positions")]
        public int RandomSeed = -1;
        [Tooltip("You can use this to load a custom asset, instead of using ProcGen. It will be scaled by \"minSize\" and \"maxSize\", so ideally it should be near a 1 meter radius.")]
        public GameObject Prefab;
        [Tooltip("The path in the scene hierarchy of the item to copy instead of using ProcGen. It will be scaled by \"minSize\" and \"maxSize\", so ideally it should be near a 1 meter radius.")]
        [ConditionalField(nameof(Prefab), (GameObject)null)]
        public string Path;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("amount", Amount);
            writer.WriteProperty("inclination", Inclination);
            writer.WriteProperty("innerRadius", InnerRadius);
            writer.WriteProperty("longitudeOfAscendingNode", LongitudeOfAscendingNode);
            writer.WriteProperty("maxSize", MaxSize);
            writer.WriteProperty("minSize", MinSize);
            writer.WriteProperty("outerRadius", OuterRadius);
            writer.WritePropertyName("procGen");
            writer.WriteStartObject();
            writer.WriteProperty("color", ProcGenColor);
            writer.WriteProperty("scale", ProcGenScale);
            writer.WriteEndObject();
            writer.WriteProperty("randomSeed", RandomSeed);
            if (Prefab)
            {
                writer.WriteProperty("assetBundle", AssetRepository.GetAssetBundlePath(Prefab));
                writer.WriteProperty("path", AssetRepository.GetAssetPath(Prefab));
            }
            else
            {
                writer.WriteProperty("path", Path);
            }
            if (!RandomOrientation)
                writer.WriteProperty("randomOrientation", RandomOrientation);
            if (Gravity != 1f)
                writer.WriteProperty("gravity", Gravity);
        }
    }
}
