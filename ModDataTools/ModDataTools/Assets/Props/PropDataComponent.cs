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
        public abstract PropData GetData();
        public virtual void WriteJsonProps(PropContext context, JsonTextWriter writer)
            => GetData().WriteJsonProps(context, writer);

        public abstract string GetPlanetPath(PropContext context);
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
