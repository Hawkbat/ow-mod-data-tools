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
    public class ShapeConfig : IJsonSerializable
    {
        [Tooltip("The type of shape or collider to add. Sphere, box, and capsule colliders are more performant and support collision.")]
        public ShapeType Type = ShapeType.Sphere;
        [Tooltip("The radius of the shape or collider.")]
        [ConditionalField(nameof(Type), ShapeType.Sphere, ShapeType.Capsule, ShapeType.Cylinder, ShapeType.Hemisphere, ShapeType.Hemicapsule, ShapeType.Ring)]
        public float Radius = 1f;
        [Tooltip("The height of the shape or collider.")]
        [ConditionalField(nameof(Type), ShapeType.Capsule, ShapeType.Cylinder, ShapeType.Cone, ShapeType.Hemicapsule, ShapeType.Ring)]
        public float Height = 1f;
        [Tooltip("The axis that the shape or collider is aligned with. The flat bottom of the shape will be pointing towards the negative axis.")]
        [ConditionalField(nameof(Type), ShapeType.Capsule, ShapeType.Cone, ShapeType.Hemisphere, ShapeType.Hemicapsule)]
        public ColliderAxis Direction = ColliderAxis.Y;
        [Tooltip("The inner radius of the shape.")]
        [ConditionalField(nameof(Type), ShapeType.Cone, ShapeType.Ring)]
        public float InnerRadius = 0f;
        [Tooltip("The outer radius of the shape.")]
        [ConditionalField(nameof(Type), ShapeType.Cone, ShapeType.Ring)]
        public float OuterRadius = 0.5f;
        [Tooltip("Whether the shape has an end cap.")]
        [ConditionalField(nameof(Type), ShapeType.Hemisphere, ShapeType.Hemicapsule)]
        public bool Cap = true;
        [Tooltip("The size of the shape or collider.")]
        [ConditionalField(nameof(Type), ShapeType.Box)]
        public Vector3 Size = Vector3.one;
        [Tooltip("The offset of the shape or collider from the object's origin.")]
        public Vector3 Offset;
        [Tooltip("Whether the collider should have collision enabled. If false, the collider will be a trigger.")]
        [ConditionalField(nameof(Type), ShapeType.Sphere, ShapeType.Box, ShapeType.Capsule)]
        public bool HasCollision;
        [Tooltip("Setting this to false will force it to use a collider, and setting to true will force it to use a shape. Shapes do not support collision and are less performant, but support a wider set of shapes and are required by some components. If left empty it will default to using a shape, unless collision is enabled in which case it defaults to using a collider.")]
        public NullishBool UseShape;

        public bool UsesRadius => Type is ShapeType.Sphere or ShapeType.Capsule or ShapeType.Cylinder or ShapeType.Hemisphere or ShapeType.Hemicapsule or ShapeType.Ring;
        public bool UsesHeight => Type is ShapeType.Capsule or ShapeType.Cylinder or ShapeType.Cone or ShapeType.Hemicapsule or ShapeType.Ring;
        public bool UsesDirection => Type is ShapeType.Capsule or ShapeType.Cone or ShapeType.Hemisphere or ShapeType.Hemicapsule;
        public bool UsesInnerOuterRadius => Type is ShapeType.Cone or ShapeType.Ring;
        public bool UsesCap => Type is ShapeType.Hemisphere or ShapeType.Hemicapsule;
        public bool SupportsCollision => Type is ShapeType.Sphere or ShapeType.Box or ShapeType.Capsule;

        public void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("type", Type);
            if (UsesRadius && Radius != 1f)
                writer.WriteProperty("radius", Radius);
            if (UsesHeight && Height != 1f)
                writer.WriteProperty("height", Height);
            if (UsesDirection && Direction != ColliderAxis.Y)
                writer.WriteProperty("direction", Direction);
            if (UsesInnerOuterRadius)
            {
                if (InnerRadius != 0f)
                    writer.WriteProperty("innerRadius", InnerRadius);
                if (OuterRadius != 0.5f)
                    writer.WriteProperty("outerRadius", OuterRadius);
            }
            if (UsesCap && !Cap)
                writer.WriteProperty("cap", Cap);
            if (Type == ShapeType.Box && Size != Vector3.one)
                writer.WriteProperty("size", Size);
            if (Offset != Vector3.zero)
                writer.WriteProperty("offset", Offset);
            if (SupportsCollision && HasCollision)
                writer.WriteProperty("hasCollision", HasCollision);
            writer.WriteProperty("useShape", UseShape);
            writer.WriteEndObject();
        }

        public void DrawGizmo(Matrix4x4 matrix)
        {
            var axis = Direction switch
            {
                ColliderAxis.X => Vector3.right,
                ColliderAxis.Z => Vector3.forward,
                _ => Vector3.up,
            };
            var extentRadius = UsesInnerOuterRadius ? Mathf.Max(Radius, InnerRadius, OuterRadius) : Radius;
            var axisAlignedSize = Vector3.one * extentRadius * 2f - axis * extentRadius * 2f + axis * Height;
            Gizmos.matrix = matrix;
            switch (Type)
            {
                case ShapeType.Sphere:
                case ShapeType.Hemisphere:
                    Gizmos.DrawWireSphere(Offset, Radius);
                    break;
                case ShapeType.Box:
                    Gizmos.DrawWireCube(Offset, Size);
                    break;
                default:
                    Gizmos.DrawWireCube(Offset, axisAlignedSize);
                    break;
            }
            Gizmos.matrix = Matrix4x4.identity;
        }

        public enum ShapeType
        {
            Sphere = 0,
            Box = 1,
            Capsule = 2,
            Cylinder = 3,
            Cone = 4,
            Hemisphere = 5,
            Hemicapsule = 6,
            Ring = 7,
        }

        public enum ColliderAxis
        {
            X = 0,
            Y = 1,
            Z = 2,
        }
    }
}
