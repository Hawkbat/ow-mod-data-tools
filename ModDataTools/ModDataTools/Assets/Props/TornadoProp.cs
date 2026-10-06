using ModDataTools.Assets.Volumes;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class TornadoPropData : GeneralPropData
    {
        [Tooltip("Alternative to setting the position. Will choose a random place at this elevation.")]
        public NullishSingle Elevation;
        [Tooltip("The height of this tornado.")]
        public float Height = 30f;
        [Tooltip("The colour of the tornado.")]
        public NullishColor Tint;
        [Tooltip("What type of cyclone should this be? Upwards and downwards are both tornados and will push in that direction.")]
        public TornadoType Type;
        [Tooltip("Angular distance from the starting position that it will wander, in terms of the angle around the x-axis.")]
        public float WanderDegreesX = 45f;
        [Tooltip("Angular distance from the starting position that it will wander, in terms of the angle around the z-axis.")]
        public float WanderDegreesZ = 45f;
        [Tooltip("The rate at which the tornado will wander around the planet. Set to 0 for it to be stationary. Should be around 0.1.")]
        public float WanderRate;
        [Tooltip("The maximum distance at which you'll hear the sounds of the cyclone. If not set it will scale relative to the size of the cyclone.")]
        public NullishSingle AudioDistance;
        public TornadoFluidType FluidType = TornadoFluidType.Cloud;
        [Tooltip("The type of hazard for this tornado. Set to None for this tornado to not be hazardous.")]
        public HazardVolumeData.HazardType HazardType = HazardVolumeData.HazardType.None;
        [Tooltip("The amount of damage you will take per second while inside this tornado.")]
        [ConditionalField(nameof(HazardType), HazardVolumeData.HazardType.None, Invert = true)]
        public float DamagePerSecond = 10f;
        [Tooltip("Whether this tornado causes damage on first contact.")]
        public bool HasFirstContactDamage;
        [Tooltip("The type of damage you will take when you first touch this tornado.")]
        [ConditionalField(nameof(HasFirstContactDamage))]
        public HazardVolumeData.InstantDamageType FirstContactDamageType;
        [Tooltip("The amount of damage you will take when you first touch this tornado.")]
        [ConditionalField(nameof(HasFirstContactDamage))]
        public float FirstContactDamage = 10f;
        public override bool SkipPosition => Elevation.HasValue;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("elevation", Elevation);
            writer.WriteProperty("height", Height);
            writer.WriteProperty("tint", Tint);
            writer.WriteProperty("type", Type);
            writer.WriteProperty("wanderDegreesX", WanderDegreesX);
            writer.WriteProperty("wanderDegreesZ", WanderDegreesZ);
            writer.WriteProperty("wanderRate", WanderRate);
            writer.WriteProperty("audioDistance", AudioDistance);
            if (FluidType != TornadoFluidType.Cloud)
                writer.WriteProperty("fluidType", FluidType);
            if (HazardType != HazardVolumeData.HazardType.None)
            {
                writer.WriteProperty("hazardType", HazardType);
                if (DamagePerSecond != 10f)
                    writer.WriteProperty("damagePerSecond", DamagePerSecond);
            }
            if (HasFirstContactDamage)
            {
                writer.WriteProperty("firstContactDamageType", FirstContactDamageType);
                if (FirstContactDamage != 10f)
                    writer.WriteProperty("firstContactDamage", FirstContactDamage);
            }
        }

        public enum TornadoType
        {
            Upwards = 0,
            Downwards = 1,
            Hurricane = 2,
        }

        public enum TornadoFluidType
        {
            None = 0,
            Water = 1,
            Cloud = 2,
            Sand = 3,
            Plasma = 4,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(TornadoPropAsset))]
    public class TornadoPropAsset : GeneralPropAsset<TornadoPropData> { }
    public class TornadoPropComponent : GeneralPropComponent<TornadoPropData> { }
}
