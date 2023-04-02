using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Utilities
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ConditionalFieldAttribute : PropertyAttribute
    {
        public string Field;
        public object[] Values;
        public bool Invert;

        public ConditionalFieldAttribute(string field)
        {
            Field = field;
            Values = null;
        }

        public ConditionalFieldAttribute(string field, params object[] values)
        {
            Field = field;
            Values = values;
        }
    }
}
