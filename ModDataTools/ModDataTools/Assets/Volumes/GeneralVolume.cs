using ModDataTools.Assets.Props;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Volumes
{
    [Serializable]
    public abstract class GeneralVolumeData : GeneralPointPropData
    {
        [Tooltip("The radius of this volume.")]
        public float Radius;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("radius", Radius);
        }
    }

    public abstract class GeneralVolumeAsset<T> : GeneralPointPropAsset<T> where T : GeneralVolumeData { }
    public abstract class GeneralVolumeComponent<T> : GeneralPointPropComponent<T> where T : GeneralVolumeData
    {
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Data.Radius);
        }
    }
}
