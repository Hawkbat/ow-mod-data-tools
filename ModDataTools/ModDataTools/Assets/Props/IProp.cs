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
    public interface IProp
    {
        public abstract string PropID { get; }
        public abstract string PropName { get; }
        public PropData GetData();
        public void WriteJsonProps(PropContext context, JsonTextWriter writer);
        public void Validate(PropContext context, IAssetValidator validator);
        public void Localize(PropContext context, Localization l10n);
        public IEnumerable<AssetResource> GetResources(PropContext context);
        public string GetPlanetPath(PropContext context);
    }

    public interface IProp<T> : IProp where T : PropData
    {
        public T Data { get; }
    }
}
