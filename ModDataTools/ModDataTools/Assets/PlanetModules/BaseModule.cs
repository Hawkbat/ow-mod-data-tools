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
    public class BaseModule : PlanetModule
    {
        [Tooltip("Set this to true if you are replacing the sun with a different body. Only one object in a star system should ever have this set to true.")]
        public bool CenterOfSolarSystem;
        [Tooltip("A scale height used for a number of things. Should be the approximate radius of the body.")]
        public float SurfaceSize;
        [Tooltip("The acceleration due to gravity felt as the surfaceSize. Timber Hearth has 12 for reference")]
        public float SurfaceGravity;
        [Tooltip("How gravity falls off with distance. Most planets use linear but the sun and some moons use inverseSquared.")]
        public GravityFallOffType GravityFallOff = GravityFallOffType.Linear;
        [Tooltip("You can force this planet's gravity to be felt over other gravity/zero-gravity sources by increasing this number.")]
        public NullishInt GravityVolumePriority;
        [Tooltip("Optional. Overrides how far the player must be from the planet for their feet to automatically orient towards the ground.")]
        public NullishInt GravityAligmentRadiusOverride;
        [Tooltip("An override for the radius of the planet's gravitational sphere of influence.")]
        public NullishSingle SphereOfInfluenceOverride;
        [Tooltip("Radius of a simple sphere used as the ground for the planet. If you want to use more complex terrain, leave this as 0.")]
        public NullishSingle GroundSize;
        [Tooltip("Do we show the minimap when walking around this planet?")]
        public bool ShowMinimap = true;
        [Tooltip("Is this planet able to detect fluid volumes? Disabling this means that entering a star or lava volume will not destroy this planet. May have adverse effects if anglerfish are added to this planet, disable this if you want those to work (they have fluid volumes in their mouths)")]
        public bool HasFluidDetector = true;
        [Tooltip("Apply physics to this planet when you bump into it. Will have a spherical collider the size of surfaceSize. For custom colliders they have to all be convex and you can leave surface size as 0. This is meant for stuff like satellites which are relatively simple and can be de-orbited. If you are using an orbit line but a tracking line, it will be removed when the planet is bumped in to.")]
        public bool Pushable;
        [Tooltip("Set this to true to have no proxy be generated for this planet. This is a small representation of the planet that appears when it is outside of the regular Unity camera range.")]
        public bool HideProxy;

        public enum GravityFallOffType
        {
            Linear = 0,
            InverseSquared = 1,
        }

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (CenterOfSolarSystem)
                writer.WriteProperty("centerOfSolarSystem", CenterOfSolarSystem);
            if (GravityFallOff != GravityFallOffType.Linear)
                writer.WriteProperty("gravityFallOff", GravityFallOff);
            writer.WriteProperty("groundSize", GroundSize);
            if (!HasFluidDetector)
                writer.WriteProperty("hasFluidDetector", HasFluidDetector);
            if (!ShowMinimap)
                writer.WriteProperty("showMinimap", ShowMinimap);
            writer.WriteProperty("soiOverride", SphereOfInfluenceOverride);
            writer.WriteProperty("surfaceGravity", SurfaceGravity);
            writer.WriteProperty("surfaceSize", SurfaceSize);
            writer.WriteProperty("gravityVolumePriority", GravityVolumePriority);
            writer.WriteProperty("gravityAlignmentRadiusOverride", GravityAligmentRadiusOverride);
            if (Pushable)
                writer.WriteProperty("pushable", Pushable);
            if (HideProxy)
                writer.WriteProperty("hideProxy", HideProxy);
        }
    }
}
