using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class SingularityPropData : GeneralPropData
    {
        [Tooltip("Scale this object over time")]
        public AnimationCurve Curve;
        [Tooltip("Radius of the event horizon (solid part)")]
        public float HorizonRadius;
        [Tooltip("Radius of the distortion effects. Defaults to 2.5 * horizonRadius")]
        public NullishSingle DistortRadius;
        [Tooltip("If you want a black hole to load a new star system scene, put it here.")]
        public StarSystemAsset TargetStarSystem;
        [Tooltip("Type of singularity (white hole or black hole)")]
        public SingularityType Type;
        [Tooltip("Whether a black hole emits blue particles upon warping. It doesn't scale, so disabling this for small black holes is recommended")]
        public bool HasWarpEffects = true;
        [Tooltip("Optional override for the render queue. If the singularity is rendering oddly, increasing this to 3000 can help. Value must be between 2501 and 3500")]
        public NullishInt RenderQueueOverride;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Curve.keys.Any())
                writer.WriteProperty("curve", Curve);
            writer.WriteProperty("horizonRadius", HorizonRadius);
            writer.WriteProperty("distortRadius", DistortRadius);
            if (TargetStarSystem)
                writer.WriteProperty("targetStarSystem", TargetStarSystem.FullName);
            writer.WriteProperty("type", Type);
            if (!HasWarpEffects)
                writer.WriteProperty("hasWarpEffects", HasWarpEffects);
            writer.WriteProperty("renderQueueOverride", RenderQueueOverride);
        }

        public enum SingularityType
        {
            BlackHole = 0,
            WhiteHole = 1,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(SingularityPropAsset))]
    public class SingularityPropAsset : GeneralPropAsset<SingularityPropData>
    {
        [Tooltip("The white hole or black hole that is paired to this one. If you don't set a value, entering will kill the player")]
        public SingularityPropAsset PairedSingularity;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (PairedSingularity)
                writer.WriteProperty("pairedSingularity", PairedSingularity.FullID);
            writer.WriteProperty("uniqueID", FullID);
            base.WriteJsonProps(context, writer);
        }
    }

    public class SingularityPropComponent : GeneralPropComponent<SingularityPropData>
    {
        [Tooltip("The white hole or black hole that is paired to this one. If you don't set a value, entering will kill the player")]
        public SingularityPropAsset PairedSingularityAsset;
        [Tooltip("The white hole or black hole that is paired to this one. If you don't set a value, entering will kill the player")]
        public SingularityPropComponent PairedSingularity;

        public string UniqueID => GetInstanceID().ToString();

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (PairedSingularityAsset)
                writer.WriteProperty("pairedSingularity", PairedSingularityAsset.FullID);
            else if (PairedSingularity)
                writer.WriteProperty("pairedSingularity", PairedSingularity.UniqueID);
            writer.WriteProperty("uniqueID", UniqueID);
            base.WriteJsonProps(context, writer);
        }
    }
}
