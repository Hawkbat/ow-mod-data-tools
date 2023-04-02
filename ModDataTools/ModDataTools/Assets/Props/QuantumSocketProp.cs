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
    public class QuantumSocketPropData : GeneralPropData
    {
        [Tooltip("The probability any props that are part of this group will occupy this socket")]
        public float Probability = 1f;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("probability", Probability);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(QuantumSocketPropAsset))]
    public class QuantumSocketPropAsset : GeneralPropAsset<QuantumSocketPropData> {
        [Tooltip("The quantum group that this socket belongs to")]
        public QuantumGroupPropAsset QuantumGroup;
    }

    public class QuantumSocketPropComponent : GeneralPropComponent<QuantumSocketPropData>
    {
        [Tooltip("The asset representing the quantum group that this socket belongs to")]
        public QuantumGroupPropAsset QuantumGroupAsset;
        [Tooltip("The quantum group that this socket belongs to")]
        public QuantumGroupPropComponent QuantumGroup;
    }
}
