using ModDataTools.Assets;
using ModDataTools.Assets.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Utilities
{
    public interface IAssetRepositoryStore
    {
        public IEnumerable<T> LoadAllAssets<T>() where T : DataAsset;
        public string GetAssetBundle(UnityEngine.Object obj);
        public string GetAssetPath(UnityEngine.Object obj);
    }
}
