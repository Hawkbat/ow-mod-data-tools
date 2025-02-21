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
    public class OrbitModule : PlanetModule
    {
        [Tooltip("Is the body meant to stay in one place without moving? If staticPosition is not set, the initial position will be determined using its orbital parameters.")]
        public bool IsStatic;
        [Tooltip("Specify this if you want the body to remain stationary at a given location (ie not orbit its parent). Required for Bramble dimensions")]
        public NullishVector3 StaticPosition;
        [Tooltip("The body this one will orbit around")]
        public PlanetAsset PrimaryBody;
        [Tooltip("Is this the moon of a planet? Used for determining when its name is shown on the map.")]
        public bool IsMoon;
        [Tooltip("The angle between the normal to the orbital plane and its axis of rotation.")]
        public float AxialTilt;
        [Tooltip("Rotation period in minutes.")]
        public float SiderealPeriod;
        [Tooltip("Offsets the planet's starting sidereal rotation. In degrees.")]
        public float InitialRotation;
        [Tooltip("Should the body always have one side facing its primary?")]
        public bool IsTidallyLocked;
        [Tooltip("If it is tidally locked, this direction will face towards the primary. Ex: Interloper uses 0, -1, 0. Most planets will want something like -1, 0, 0.")]
        [ConditionalField(nameof(IsTidallyLocked))]
        public Vector3 AlignmentAxis = Vector3.left;
        [Tooltip("Referring to the orbit line in the map screen.")]
        public bool ShowOrbitLine = true;
        [Tooltip("Colour of the orbit-line in the map view.")]
        [ConditionalField(nameof(ShowOrbitLine))]
        public Color OrbitLineTint = Color.white;
        [Tooltip("Should the orbit line be dotted?")]
        [ConditionalField(nameof(ShowOrbitLine))]
        public bool DottedOrbitLine;
        [Tooltip("Should we just draw a line behind its orbit instead of the entire circle/ellipse?")]
        [ConditionalField(nameof(ShowOrbitLine))]
        public bool TrackingOrbitLine;
        [Tooltip("If the camera is closer than this distance the orbit line will fade out. Leave empty to not have it fade out.")]
        public NullishSingle OrbitLineFadeStartDistance;
        [Tooltip("If the camera is farther than this distance the orbit line will fade out. Leave empty to not have it fade out.")]
        public NullishSingle OrbitLineFadeEndDistance;
        [Tooltip("The semi-major axis of the ellipse that is the body's orbit. For a circular orbit this is the radius.")]
        public float SemiMajorAxis = 5000f;
        [Tooltip("The angle (in degrees) between the body's orbit and the plane of the star system")]
        public float Inclination;
        [Tooltip("An angle (in degrees) defining the point where the orbit of the body rises above the orbital plane if it has nonzero inclination.")]
        public float LongitudeOfAscendingNode;
        [Tooltip("At 0 the orbit is a circle. The closer to 1 it is, the more oval-shaped the orbit is.")]
        [Range(0f, 1f)]
        public float Eccentricity;
        [Tooltip("An angle (in degrees) defining the location of the periapsis (the closest distance to it's primary body) if it has nonzero eccentricity.")]
        public float ArgumentOfPeriapsis;
        [Tooltip("Where the planet should start off in its orbit in terms of the central angle.")]
        public float TrueAnomaly;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("staticPosition", StaticPosition);
            if (PrimaryBody)
                writer.WriteProperty("primaryBody", PrimaryBody.FullID);
            if (IsMoon)
                writer.WriteProperty("isMoon", IsMoon);
            if (AxialTilt != 0f)
                writer.WriteProperty("axialTilt", AxialTilt);
            if (SiderealPeriod != 0f)
                writer.WriteProperty("siderealPeriod", SiderealPeriod);
            if (InitialRotation != 0f)
                writer.WriteProperty("initialRotation", InitialRotation);
            if (IsTidallyLocked)
                writer.WriteProperty("isTidallyLocked", IsTidallyLocked);
            if (IsStatic)
                writer.WriteProperty("isStatic", IsStatic);
            if (IsTidallyLocked)
                writer.WriteProperty("alignmentAxis", AlignmentAxis);
            if (!ShowOrbitLine)
                writer.WriteProperty("showOrbitLine", ShowOrbitLine);
            if (ShowOrbitLine && DottedOrbitLine)
                writer.WriteProperty("dottedOrbitLine", DottedOrbitLine);
            if (OrbitLineTint != Color.white)
                writer.WriteProperty("tint", (Color32)OrbitLineTint);
            if (ShowOrbitLine && TrackingOrbitLine)
                writer.WriteProperty("trackingOrbitLine", TrackingOrbitLine);
            if (ShowOrbitLine && OrbitLineFadeEndDistance.HasValue)
                writer.WriteProperty("orbitLineFadeEndDistance", OrbitLineFadeEndDistance);
            if (ShowOrbitLine && OrbitLineFadeStartDistance.HasValue)
                writer.WriteProperty("orbitLineFadeStartDistance", OrbitLineFadeStartDistance);
            if (SemiMajorAxis != 5000f)
                writer.WriteProperty("semiMajorAxis", SemiMajorAxis);
            if (Inclination != 0f)
                writer.WriteProperty("inclination", Inclination);
            if (LongitudeOfAscendingNode != 0f)
                writer.WriteProperty("longitudeOfAscendingNode", LongitudeOfAscendingNode);
            if (Eccentricity != 0f)
                writer.WriteProperty("eccentricity", Eccentricity);
            if (ArgumentOfPeriapsis != 0f)
                writer.WriteProperty("argumentOfPeriapsis", ArgumentOfPeriapsis);
            if (TrueAnomaly != 0f)
                writer.WriteProperty("trueAnomaly", TrueAnomaly);
        }
    }
}
