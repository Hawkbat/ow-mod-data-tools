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
    public interface IProp
    {
        public abstract string PropID { get; }
        public abstract string PropName { get; }
        public PropData GetData();
        public void WriteJsonProps(PropContext context, JsonTextWriter writer);
        public string GetPlanetPath(PropContext context);
    }

    public interface IProp<T> : IProp where T : PropData
    {
        public T Data { get; }
    }
}
