using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModDataTools.Assets;
using ModDataTools.Assets.Props;
using UnityEngine;

namespace ModDataTools.Utilities
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
            => AssetCache<T>.GetAssetByName(name);
        public static T GetAssetByID<T>(string id) where T : DataAsset
            => AssetCache<T>.GetAssetByID(id);
        public static IEnumerable<T> GetAllAssets<T>() where T : DataAsset
            => AssetCache<T>.GetAllAssets();
        public static IEnumerable<PropContext<T>> GetAllProps<T>() where T : PropData
            => PropCache<T>.GetAllProps();
        public static IEnumerable<PropContext<T>> GetProps<T>(PlanetAsset planet) where T : PropData
            => PropCache<T>.GetProps(planet);

        public static string GetAssetBundle(UnityEngine.Object obj)
            => store == null ? null : store.GetAssetBundle(obj);
        public static string GetAssetPath(UnityEngine.Object obj)
            => store == null ? null : store.GetAssetPath(obj);
        public static string GetAssetFileName(UnityEngine.Object obj)
        {
            var path = GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) return null;
            return Path.GetFileName(path);
        }

        internal static class AssetCache<T> where T : DataAsset
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
                    if (store == null)
                    {
                        Debug.LogWarning("Asset repository was accessed before it was initialized");
                        return;
                    }
                    foreach (var asset in store.LoadAllAssets<T>())
                    {
                        if (!valuesByName.ContainsKey(asset.FullName))
                            valuesByName.Add(asset.FullName, asset);
                        if (!valuesByID.ContainsKey(asset.FullID))
                            valuesByID.Add(asset.FullID, asset);
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

        internal static class PropCache<T> where T : PropData
        {
            static DateTime? reloadTime;
            static readonly List<PropContext<T>> values = new();
            static readonly Dictionary<string, List<PropContext<T>>> valuesByPlanet = new();

            public static void Reload(bool force = false)
            {
                if (force || !reloadTime.HasValue || reloadTime.Value < AssetRepository.reloadTime)
                {
                    reloadTime = AssetRepository.reloadTime;
                    values.Clear();
                    valuesByPlanet.Clear();
                    foreach (var planet in GetAllAssets<PlanetAsset>())
                    {
                        var planetValues = new List<PropContext<T>>();

                        var rootProp = new PropContext<PropData> {
                            Planet = planet,
                            DetailPath = planet.GetSectorPath(),
                            Prop = null,
                        };

                        var propAssets = GetAllAssets<PropDataAsset<T>>().Where(p => p.Planet == planet);
                        foreach (var propAsset in propAssets)
                        {
                            planetValues.Add(new PropContext<T>
                            {
                                Planet = planet,
                                DetailPath = propAsset.GetPlanetPath(rootProp),
                                Prop = propAsset,
                            });
                        }

                        var detailProps = new Queue<PropContext<DetailPropData>>();
                        var detailAssets = GetAllAssets<DetailPropAsset>()
                            .Where(p => p.Planet == planet && p.Data.Prefab && !p.Data.IsProxyDetail);
                        foreach (var detailAsset in detailAssets)
                        {
                            detailProps.Enqueue(new PropContext<DetailPropData>
                            {
                                Planet = planet,
                                DetailPath = detailAsset.GetPlanetPath(rootProp),
                                Prop = detailAsset,
                            });
                        }
                        while (detailProps.Any())
                        {
                            var detailProp = detailProps.Dequeue();
                            var propComponents = detailProp.Data.Prefab.GetComponentsInChildren<PropDataComponent<T>>();
                            foreach (var propComponent in propComponents)
                            {
                                planetValues.Add(new PropContext<T>
                                {
                                    Planet = planet,
                                    DetailPath = propComponent.GetPlanetPath(detailProp),
                                    Prop = propComponent,
                                });
                            }
                            var detailComponents = detailProp.Data.Prefab.GetComponentsInChildren<DetailPropComponent>()
                                .Where(p => p.Data.Prefab && !p.Data.IsProxyDetail);
                            foreach (var detailComponent in detailComponents)
                            {
                                detailProps.Enqueue(new PropContext<DetailPropData>
                                {
                                    Planet = planet,
                                    DetailPath = detailComponent.GetPlanetPath(detailProp),
                                    Prop = detailComponent,
                                });
                            }
                        }
                        foreach (var prop in planetValues)
                            values.Add(prop);
                        valuesByPlanet.Add(planet.FullID, planetValues);
                    }
                }
            }

            public static IEnumerable<PropContext<T>> GetAllProps()
            {
                Reload();
                return values;
            }

            public static IEnumerable<PropContext<T>> GetProps(PlanetAsset planet)
            {
                Reload();
                if (!valuesByPlanet.ContainsKey(planet.FullID))
                    return Enumerable.Empty<PropContext<T>>();
                return valuesByPlanet[planet.FullID];
            }
        }
    }
}
