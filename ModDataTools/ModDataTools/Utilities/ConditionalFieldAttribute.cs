using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Utilities
{
    public class ConditionalFieldAttribute : PropertyAttribute
    {
        public string Field;
        public object[] Values;

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
