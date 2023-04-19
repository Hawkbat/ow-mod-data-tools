using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public abstract class PropData
    {
        public abstract void WriteJsonProps(PropContext context, JsonTextWriter writer);

        public virtual void Localize(PropContext context, Localization l10n)
        {

        }

        public virtual IEnumerable<AssetResource> GetResources(PropContext context)
            => Enumerable.Empty<AssetResource>();
    }
}
