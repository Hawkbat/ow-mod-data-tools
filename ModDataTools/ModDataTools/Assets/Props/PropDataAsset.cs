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
    public abstract class PropDataAsset : DataAsset, IProp
    {
        [Tooltip("The planet this prop will be placed on")]
        public PlanetAsset Planet;

        public string PropID => FullID;
        public string PropName => FullName;

        public abstract PropData GetData();

        public virtual void WriteJsonProps(PropContext context, JsonTextWriter writer)
            => GetData().WriteJsonProps(context, writer);

        public abstract string GetPlanetPath(PropContext context);

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Planet) yield return Planet;
        }

        public void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(validator);
            GetData().Validate(context, validator);
        }

        public void Localize(PropContext context, Localization l10n)
        {
            base.Localize(l10n);
            GetData().Localize(context, l10n);
        }

        public IEnumerable<AssetResource> GetResources(PropContext context)
        {
            foreach (var resource in base.GetResources())
                yield return resource;
            foreach (var resource in GetData().GetResources(context))
                yield return resource;
        }
    }

    public abstract class PropDataAsset<T> : PropDataAsset, IProp<T> where T : PropData
    {
        [Header("Data")]
        public T Data;

        T IProp<T>.Data => Data;

        public override PropData GetData() => Data;
    }
}
