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

        public abstract PropData GetData();

        public virtual void WriteJsonProps(PropContext context, JsonTextWriter writer)
            => GetData().WriteJsonProps(context, writer);

        public abstract string GetPlanetPath(PropContext context);

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Planet) yield return Planet;
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
