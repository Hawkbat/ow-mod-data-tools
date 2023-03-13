using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModDataTools.Assets;
using ModDataTools.Utilities;
using UnityEngine;

namespace ModDataTools
{
    public static class AssetRepository
    {
        static IAssetRepositoryStore store;
        static DateTime reloadTime;

        public static void Initialize(IAssetRepositoryStore newStore)
        {
            store = newStore;
            Reload();
        }

        public static void Reload()
        {
            reloadTime = DateTime.Now;
        }

        public static T GetAssetByName<T>(string name) where T : DataAsset
            => Repository<T>.GetAssetByName(name);
        public static T GetAssetByID<T>(string id) where T : DataAsset
            => Repository<T>.GetAssetByID(id);
        public static IEnumerable<T> GetAllAssets<T>() where T : DataAsset
            => Repository<T>.GetAllAssets();

        internal static class Repository<T> where T : DataAsset
        {
            static DateTime? reloadTime;
            static readonly Dictionary<string, T> valuesByName = new();
            static readonly Dictionary<string, T> valuesByID = new();

            public static void Reload(bool force = false)
            {
                if (force || !reloadTime.HasValue || reloadTime.Value < AssetRepository.reloadTime)
                {
                    reloadTime = AssetRepository.reloadTime;
                    valuesByName.Clear();
                    valuesByID.Clear();
                    foreach (var asset in store.LoadAssets<T>())
                    {
                        if (!valuesByName.ContainsKey(asset.GetFullName()))
                            valuesByName.Add(asset.GetFullName(), asset);
                        if (!valuesByID.ContainsKey(asset.GetFullID()))
                            valuesByID.Add(asset.GetFullID(), asset);
                    }
                }
            }

            public static T GetAssetByName(string name)
            {
                Reload();
                return valuesByName.GetValueOrDefault(name);
            }

            public static T GetAssetByID(string id)
            {
                Reload();
                return valuesByID.GetValueOrDefault(id);
            }

            public static IEnumerable<T> GetAllAssets()
            {
                Reload();
                return valuesByID.Values;
            }
        }
    }
}
