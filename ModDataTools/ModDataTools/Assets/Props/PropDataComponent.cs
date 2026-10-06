using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    public abstract class PropDataComponent : MonoBehaviour, IProp
    {
        public string PropID => "PROP_" + GetInstanceID().ToString();
        public string PropName => transform.name;

        public abstract PropData GetData();
        public virtual void WriteJsonProps(PropContext context, JsonTextWriter writer)
            => GetData().WriteJsonProps(context, writer);
        public virtual void Validate(PropContext context, IAssetValidator validator)
            => GetData().Validate(context, validator);
        public virtual void Localize(PropContext context, Localization l10n)
            => GetData().Localize(context, l10n);
        public virtual IEnumerable<AssetResource> GetResources(PropContext context)
        {
            foreach (var resource in GetData().GetResources(context))
                yield return resource;
        }

        public abstract string GetPlanetPath(PropContext context);

        Transform spawnedProp;

        public void Start()
        {
            GetSpawnedProp();
            gameObject.SetActive(false);
        }

        public Transform GetSpawnedProp()
        {
            if (spawnedProp != null) return spawnedProp;
            spawnedProp = UnityUtility.GetChildren(transform.parent)
                .FirstOrDefault(t => t != transform && t.name == PropName);
            return spawnedProp;
        }
    }

    public abstract class PropDataComponent<T> : PropDataComponent, IProp<T> where T : PropData
    {
        [Header("Overrides")]
        [Tooltip("An asset to use the data from instead of this component")]
        public PropDataAsset<T> OverrideAsset;
        [Header("Data")]
        [Tooltip("The data for this prop")]
        public T Data;

        T IProp<T>.Data => Data;

        public override PropData GetData() => OverrideAsset ? OverrideAsset.Data : Data;
    }
}
