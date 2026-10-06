using ModDataTools.Assets.Props;
using ModDataTools.Assets.Resources;
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
    public class RepairVolumeData : GeneralVolumeData
    {
        [Tooltip("The name displayed in the UI when the player is repairing this object. If not set, the name of the object will be used.")]
        public string Name;
        [Tooltip("How much of the object is initially repaired. 0 = not repaired, 1 = fully repaired.")]
        [Range(0f, 1f)]
        public float RepairFraction;
        [Tooltip("The time it takes to repair the object.")]
        public float RepairTime = 3f;
        [Tooltip("The distance from the object that the player can be to repair it.")]
        public float RepairDistance = 3f;
        [Tooltip("A dialogue condition that will be set while the object is damaged. It will be unset when the object is repaired.")]
        public ConditionAsset DamagedCondition;
        [Tooltip("A dialogue condition that will be set when the object is repaired. It will be unset if the object is damaged again.")]
        public ConditionAsset RepairedCondition;
        [Tooltip("A ship log fact that will be revealed when the object is repaired.")]
        public FactAsset RevealFact;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            writer.WriteProperty("name", context.GetProp().PropID);
            if (RepairFraction != 0f)
                writer.WriteProperty("repairFraction", RepairFraction);
            if (RepairTime != 3f)
                writer.WriteProperty("repairTime", RepairTime);
            if (RepairDistance != 3f)
                writer.WriteProperty("repairDistance", RepairDistance);
            if (DamagedCondition)
                writer.WriteProperty("damagedCondition", DamagedCondition.FullID);
            if (RepairedCondition)
                writer.WriteProperty("repairedCondition", RepairedCondition.FullID);
            if (RevealFact)
                writer.WriteProperty("revealFact", RevealFact.FullID);
        }

        public override void Localize(PropContext context, Localization l10n)
        {
            base.Localize(context, l10n);
            l10n.AddUI(context.GetProp().PropID, string.IsNullOrEmpty(Name) ? context.GetProp().PropName : Name);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            if (DamagedCondition && DamagedCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(DamagedCondition)} must not be a persistent condition");
            if (RepairedCondition && RepairedCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(RepairedCondition)} must not be a persistent condition");
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(RepairVolumeAsset))]
    public class RepairVolumeAsset : GeneralVolumeAsset<RepairVolumeData> { }
    public class RepairVolumeComponent : GeneralVolumeComponent<RepairVolumeData> { }
}
