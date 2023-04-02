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
    public class RemotePlatformPropData : GeneralPropData
    {
        [Tooltip("The remote projection that this platform belongs to")]
        public RemoteProjectionAsset RemoteProjection;
        [Tooltip("A ship log fact to reveal when the platform is connected to.")]
        public FactAsset RevealFact;
        [Tooltip("Disable the structure, leaving only the pedestal.")]
        public bool DisableStructure;
        [Tooltip("Disable the pool that rises when you place a stone.")]
        public bool DisablePool;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (RevealFact)
                writer.WriteProperty("reveals", RevealFact.FullID);
            if (DisableStructure)
                writer.WriteProperty("disableStructure", DisableStructure);
            if (DisablePool)
                writer.WriteProperty("disablePool", DisablePool);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(RemotePlatformPropAsset))]
    public class RemotePlatformPropAsset : GeneralPropAsset<RemotePlatformPropData> { }
    public class RemotePlatformPropComponent : GeneralPropComponent<RemotePlatformPropData> {  }
}
