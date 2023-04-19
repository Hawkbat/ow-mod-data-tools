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
            if (skipRoot && t.parent == null) return string.Empty;
            var path = t.name;
            while (t.parent != null && (!skipRoot || t.parent != t.root))
            {
                path = t.parent.name + "/" + path;
                t = t.parent;
            }
            return path;
        }
        
        public static string ResolvePaths(params string[] paths)
        {
            var path = paths[0];
            foreach (var p in paths)
            {
                if (p.StartsWith("./")) path = path + p.Substring(1);
                else path = p;
            }
            if (path.StartsWith("/")) path = path.Substring(1);
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
            return path;
        }
    }
}
