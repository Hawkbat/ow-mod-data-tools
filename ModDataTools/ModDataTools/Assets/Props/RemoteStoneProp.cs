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
    [Serializable]
    public class RemoteStonePropData : GeneralPropData
    {
        [Tooltip("The remote projection that this stone belongs to")]
        public RemoteProjectionAsset RemoteProjection;
        [Tooltip("The text for this stone")]
        public TranslatorTextAsset TranslatorText;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {

        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(RemoteStonePropAsset))]
    public class RemoteStonePropAsset : GeneralPropAsset<RemoteStonePropData> { }
    public class RemoteStonePropComponent : GeneralPropComponent<RemoteStonePropData> { }
}
