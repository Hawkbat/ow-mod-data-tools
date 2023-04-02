using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Utilities
{
    public static class UnityUtility
    {
        public static string GetTransformPath(Transform t, bool skipRoot)
        {
            if (t == null) return null;
            var path = t.name;
            while (t.parent != null && (!skipRoot || t.parent != t.root))
            {
                path = t.parent.name + "/" + path;
                t = t.parent;
            }
            return path;
        }
    }
}
