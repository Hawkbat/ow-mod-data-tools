using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Utilities
{
    // Because Unity can't serialize System.Nullable<T>
    [Serializable]
    public abstract class Nullish<T> : IEquatable<Nullish<T>> where T : struct, IEquatable<T>
    {
        [SerializeField]
        protected bool hasValue;
        [SerializeField]
        protected T value;

        public bool HasValue => hasValue;
        public T Value
        {
            get {
                if (!hasValue) throw new NullReferenceException();
                return value;
            }
            set {
                Value = value;
                hasValue = true;
            }
        }
        public virtual T SafeValue => hasValue ? value : default;

        public static bool operator ==(Nullish<T> left, Nullish<T> right)
        {
            if (left is null || !left.hasValue)
            {
                return right is null || !right.hasValue;
            }
            return left.value.Equals(right.value);

        }

        public static bool operator !=(Nullish<T> left, Nullish<T> right) => !(left == right);
        public bool Equals(Nullish<T> other) => this == other;
        public override bool Equals(object obj) => this == (obj as Nullish<T>);
        public override int GetHashCode() => SafeValue.GetHashCode();
        public override string ToString() => hasValue ? value.ToString() : "(null)";
    }

    [Serializable]
    public class NullishSingle : Nullish<float>
    {
        public static implicit operator float(NullishSingle instance) => instance.SafeValue;
        public static implicit operator NullishSingle(float instance) => new() { hasValue = true, value = instance };
    }

    [Serializable]
    public class NullishInt : Nullish<int>
    {
        public static implicit operator int(NullishInt instance) => instance.SafeValue;
        public static implicit operator NullishInt(int instance) => new() { hasValue = true, value = instance };
    }

    [Serializable]
    public class NullishVector3 : Nullish<Vector3>
    {
        public static implicit operator Vector3(NullishVector3 instance) => instance.SafeValue;
        public static implicit operator NullishVector3(Vector3 instance) => new() { hasValue = true, value = instance };
    }

    [Serializable]
    public class NullishColor : Nullish<Color>
    {
        public override Color SafeValue => hasValue ? value : Color.white;
        public static implicit operator Color(NullishColor instance) => instance.SafeValue;
        public static implicit operator NullishColor(Color instance) => new() { hasValue = true, value = instance };
    }
}
