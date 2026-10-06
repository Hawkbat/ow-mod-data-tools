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
    public abstract class GeneralScaleablePropData : GeneralPropData
    {
    }

    public abstract class GeneralScaleablePropAsset<T> : GeneralPropAsset<T> where T : GeneralScaleablePropData
    {
        [Tooltip("Scale the prop")]
        public float Scale = 1f;
        [Tooltip("Scale each axis of the prop. Multiplied with scale.")]
        public Vector3 Stretch = Vector3.one;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Stretch == Vector3.one)
            {
                if (Scale != 1f)
                    writer.WriteProperty("scale", Scale);
            }
            else
                writer.WriteProperty("stretch", Stretch * Scale);
            base.WriteJsonProps(context, writer);
        }
    }

    public abstract class GeneralScaleablePropComponent<T> : GeneralPropComponent<T> where T : GeneralScaleablePropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (!transform.localScale.IsUniform())
                writer.WriteProperty("stretch", transform.localScale);
            else if (transform.localScale.x != 1f)
                writer.WriteProperty("scale", transform.localScale.x);
            base.WriteJsonProps(context, writer);
        }
    }
}
