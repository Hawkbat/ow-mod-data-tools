using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Utilities
{
    public class BitFieldAttribute : PropertyAttribute
    {
        public int Length;

        public BitFieldAttribute(int length)
        {
            Length = length;
        }
    }
}
