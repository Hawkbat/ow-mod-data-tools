using ModDataTools.Assets;
using ModDataTools.Assets.Props;
using ModDataTools.Utilities;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools
{
    public class Main : ModBehaviour, IAssetRepositoryStore
    {
        void Start()
        {
            AssetRepository.Initialize(this);
            ModHelper.Console.WriteLine($"{nameof(ModDataTools)} loaded.", MessageType.Info);
        }

        public IEnumerable<T> LoadAllAssets<T>() where T : DataAsset
        {
            throw new NotImplementedException();
        }

        public string GetAssetBundle(UnityEngine.Object obj)
        {
            throw new NotImplementedException();
        }

        public string GetAssetPath(UnityEngine.Object obj)
        {
            throw new NotImplementedException();
        }
    }
}
