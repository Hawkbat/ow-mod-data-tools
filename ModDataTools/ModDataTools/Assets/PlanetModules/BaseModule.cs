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
        public GravityFallOffType GravityFallOff;
        [Tooltip("Radius of a simple sphere used as the ground for the planet. If you want to use more complex terrain, leave this as 0.")]
        public NullishSingle GroundSize;
        [Tooltip("If the body should have a marker on the map screen.")]
        public bool HasMapMarker;
        [Tooltip("Do we show the minimap when walking around this planet?")]
        public bool ShowMinimap = true;
        [Tooltip("If you want the body to have a tail like the Interloper.")]
        public bool HasCometTail;
        [Tooltip("If it has a comet tail, it'll be oriented according to these Euler angles.")]
        [ConditionalField(nameof(HasCometTail))]
        public Vector3 CometTailRotation;
        [Tooltip("Can this planet survive entering a star?")]
        public bool InvulnerableToSun;
        [Tooltip("An override for the radius of the planet's gravitational sphere of influence.")]
        public NullishSingle SphereOfInfluenceOverride;
        [Tooltip("You can force this planet's gravity to be felt over other gravity/zero-gravity sources by increasing this number.")]
        public NullishInt GravityVolumePriority;

        public enum GravityFallOffType
        {
            Linear = 0,
            InverseSquared = 1,
        }

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (CenterOfSolarSystem)
                writer.WriteProperty("centerOfSolarSystem", CenterOfSolarSystem);
            if (HasCometTail)
            {
                writer.WriteProperty("hasCometTail", HasCometTail);
                writer.WriteProperty("cometTailRotation", CometTailRotation);
            }
            if (GravityFallOff != GravityFallOffType.Linear)
                writer.WriteProperty("gravityFallOff", GravityFallOff);
            writer.WriteProperty("groundSize", GroundSize);
            if (HasMapMarker)
                writer.WriteProperty("hasMapMarker", HasMapMarker);
            if (InvulnerableToSun)
                writer.WriteProperty("invulnerableToSun", InvulnerableToSun);
            if (!ShowMinimap)
                writer.WriteProperty("showMinimap", ShowMinimap);
            writer.WriteProperty("soiOverride", SphereOfInfluenceOverride);
            writer.WriteProperty("surfaceGravity", SurfaceGravity);
            writer.WriteProperty("surfaceSize", SurfaceSize);
            writer.WriteProperty("gravityVolumePriority", GravityVolumePriority);
        }
    }
}
