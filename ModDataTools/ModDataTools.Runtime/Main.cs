using ModDataTools.Assets;
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

        public IEnumerable<T> LoadAssets<T>() where T : DataAsset
            => FindObjectsOfType<T>();
    }
}
