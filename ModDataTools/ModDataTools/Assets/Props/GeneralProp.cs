using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public abstract class GeneralPropData : GeneralPointPropData
    {
    }

    public abstract class GeneralPropAsset<T> : GeneralPointPropAsset<T> where T : GeneralPropData
    {
        [Tooltip("Rotate this prop")]
        public Vector3 Rotation;
        [Tooltip("Do we try to automatically align this object to stand upright relative to the body's center? Stacks with rotation.")]
        public bool AlignRadial;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("rotation", Rotation);
            writer.WriteProperty("alignRadial", AlignRadial);
            base.WriteJsonProps(context, writer);
        }
    }

    public abstract class GeneralPropComponent<T> : GeneralPointPropComponent<T> where T : GeneralPropData
    {
        [Tooltip("Do we try to automatically align this object to stand upright relative to the body's center? Stacks with rotation.")]
        public bool AlignRadial;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("rotation", transform.localEulerAngles);
            writer.WriteProperty("alignRadial", AlignRadial);
            base.WriteJsonProps(context, writer);
        }
    }
}
