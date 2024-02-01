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
    public abstract class GeneralPointPropData : PropData
    {
        public virtual bool SkipPosition => false;
    }

    public abstract class GeneralPointPropAsset<T> : PropDataAsset<T> where T : GeneralPointPropData
    {
        [Tooltip("The path (not including the root planet object) of the parent of this game object. Optional (will default to the root sector).")]
        public string ParentPath;
        [Tooltip("Whether the positional and rotational coordinates are relative to parent instead of the root planet object.")]
        public bool IsRelativeToParent;
        [Tooltip("Position of this prop relative to the body's center")]
        public Vector3 Position;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("rename", FullID);
            if (!string.IsNullOrEmpty(ParentPath))
                writer.WriteProperty("parentPath", ParentPath);
            if (IsRelativeToParent)
                writer.WriteProperty("isRelativeToParent", IsRelativeToParent);
            if (!Data.SkipPosition)
                writer.WriteProperty("position", Position);
            base.WriteJsonProps(context, writer);
        }

        public string GetParentPlanetPath(PropContext context) => !string.IsNullOrEmpty(ParentPath) ? ParentPath : context.DetailPath;

        public override string GetPlanetPath(PropContext context)
            => GetParentPlanetPath(context) + "/" + FullID;
    }

    public abstract class GeneralPointPropComponent<T> : PropDataComponent<T> where T : GeneralPointPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("rename", transform.name);
            writer.WriteProperty("parentPath", GetParentPlanetPath(context));
            writer.WriteProperty("isRelativeToParent", true);
            if (!Data.SkipPosition)
                writer.WriteProperty("position", transform.localPosition);
            base.WriteJsonProps(context, writer);
        }

        public string GetParentPlanetPath(PropContext context) => UnityUtility.ResolvePaths(context.DetailPath + "/" + UnityUtility.GetTransformPath(transform.parent, true));

        public override string GetPlanetPath(PropContext context)
            => UnityUtility.ResolvePaths(context.DetailPath + "/" + UnityUtility.GetTransformPath(transform, true));
    }
}
