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
    public class HazardVolumeData : GeneralVolumeData
    {
        [Tooltip("The type of hazard for this volume.")]
        public HazardType Type = HazardType.General;
        [Tooltip("The amount of damage you will take per second while inside this volume.")]
        public float DamagePerSecond;
        [Tooltip("The type of damage you will take when you first touch this volume.")]
        public InstantDamageType FirstContactDamageType = InstantDamageType.Impact;
        [Tooltip("The amount of damage you will take when you first touch this volume.")]
        public float FirstContactDamage;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Type != HazardType.General)
                writer.WriteProperty("type", Type);
            if (DamagePerSecond != 10f)
                writer.WriteProperty("damagePerSecond", DamagePerSecond);
            if (FirstContactDamage > 0f)
            {
                if (FirstContactDamageType != InstantDamageType.Impact)
                    writer.WriteProperty("firstContactDamageType", FirstContactDamageType);
                writer.WriteProperty("firstContactDamage", FirstContactDamage);
            }
        }

        public enum InstantDamageType
        {
            Impact = 0,
            Puncture = 1,
            Electrical = 2,
        }

        public enum HazardType
        {
            None = 0,
            General = 1,
            GhostMatter = 2,
            Heat = 3,
            Fire = 4,
            Sandfall = 5,
            Electricity = 6,
            Rapids = 7,
            RiverHeat = 8,
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(HazardVolumeAsset))]
    public class HazardVolumeAsset : GeneralVolumeAsset<HazardVolumeData> { }
    public class HazardVolumeComponent : GeneralVolumeComponent<HazardVolumeData> { }
}
