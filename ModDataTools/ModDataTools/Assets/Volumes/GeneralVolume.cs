using ModDataTools.Assets.Props;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Volumes
{
    [Serializable]
    public abstract class GeneralVolumeData : GeneralPropData
    {
        [Tooltip("The radius of this volume, if a shape is not specified.")]
        [ConditionalField(nameof(HasShape), false)]
        public float Radius = 1f;
        [Tooltip("Whether to use a custom shape for this volume instead of a sphere.")]
        public bool HasShape;
        [Tooltip("The shape of this volume.")]
        [ConditionalField(nameof(HasShape))]
        public ShapeConfig Shape;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (HasShape)
                writer.WriteProperty("shape", Shape);
            else if (Radius != 1f)
                writer.WriteProperty("radius", Radius);
        }

        public void DrawGizmo(Transform transform)
        {
            if (HasShape)
            {
                Shape.DrawGizmo(transform.localToWorldMatrix);
                return;
            }
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }

    public abstract class GeneralVolumeAsset<T> : GeneralPropAsset<T> where T : GeneralVolumeData { }
    public abstract class GeneralVolumeComponent<T> : GeneralPropComponent<T> where T : GeneralVolumeData
    {
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            ((T)GetData()).DrawGizmo(transform);
        }
    }
}
