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
    [Serializable]
    public class DreamCampfirePropData : CampfirePropData
    {
        [Tooltip("Volumes to explicitly add the player to when waking up at this campfire.")]
        public EntrywayVolumesConfig EntrywayVolumes;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            EntrywayVolumes.WriteJsonProperty(context.Planet, writer);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            EntrywayVolumes.Validate(context, context.Planet, validator);
        }

        public static void WriteLinks(PropContext context, JsonTextWriter writer, IProp arrivalPoint, IProp alarmBell)
        {
            if (arrivalPoint != null)
                writer.WriteProperty("id", arrivalPoint.PropID);
            if (alarmBell != null)
            {
                var alarmBellPath = AssetRepository.GetPropPlanetPath<AlarmBellPropData>(context.Planet, alarmBell);
                if (alarmBellPath != null)
                    writer.WriteProperty("alarmBellPath", alarmBellPath);
            }
        }

        public static void ValidateLinks(PropContext context, IAssetValidator validator, IProp arrivalPoint, IProp alarmBell)
        {
            if (arrivalPoint == null)
                validator.Error(context.Planet, $"Dream campfire '{context.GetProp().PropName}' has no dream arrival point set");
            if (alarmBell != null && AssetRepository.GetPropContext<AlarmBellPropData>(context.Planet, alarmBell) == null)
                validator.Error(context.Planet, $"Dream campfire '{context.GetProp().PropName}' is connected to alarm bell '{alarmBell.PropName}', which is not on the same planet");
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(DreamCampfirePropAsset))]
    public class DreamCampfirePropAsset : GeneralPropAsset<DreamCampfirePropData>
    {
        [Tooltip("The dream world arrival point that sleeping at this campfire sends the player to.")]
        public DreamArrivalPointPropAsset ArrivalPoint;
        [Tooltip("The alarm bell this campfire is connected to.")]
        public AlarmBellPropAsset AlarmBell;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            DreamCampfirePropData.WriteLinks(context, writer, ArrivalPoint ? ArrivalPoint : null, AlarmBell ? AlarmBell : null);
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            DreamCampfirePropData.ValidateLinks(new PropContext<DreamCampfirePropData> { Planet = Planet, Prop = this }, validator, ArrivalPoint ? ArrivalPoint : null, AlarmBell ? AlarmBell : null);
        }
    }

    public class DreamCampfirePropComponent : GeneralPropComponent<DreamCampfirePropData>
    {
        [Tooltip("The dream world arrival point that sleeping at this campfire sends the player to.")]
        public DreamArrivalPointPropAsset ArrivalPointAsset;
        [Tooltip("The dream world arrival point that sleeping at this campfire sends the player to.")]
        public DreamArrivalPointPropComponent ArrivalPoint;
        [Tooltip("The alarm bell this campfire is connected to.")]
        public AlarmBellPropAsset AlarmBellAsset;
        [Tooltip("The alarm bell this campfire is connected to.")]
        public AlarmBellPropComponent AlarmBell;

        IProp GetArrivalPoint() => ArrivalPointAsset ? (IProp)ArrivalPointAsset : ArrivalPoint ? ArrivalPoint : null;
        IProp GetAlarmBell() => AlarmBellAsset ? (IProp)AlarmBellAsset : AlarmBell ? AlarmBell : null;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            DreamCampfirePropData.WriteLinks(context, writer, GetArrivalPoint(), GetAlarmBell());
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            DreamCampfirePropData.ValidateLinks(context, validator, GetArrivalPoint(), GetAlarmBell());
        }
    }
}
