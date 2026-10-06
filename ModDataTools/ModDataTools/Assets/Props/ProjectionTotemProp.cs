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
    public class ProjectionTotemPropData : GeneralPropData
    {
        [Tooltip("Whether the totem should start lit or extinguished.")]
        public bool StartLit;
        [Tooltip("Whether the projection totem should be able to extinguished but not be able to be lit again with the artifact. Mainly useful if the totem starts lit.")]
        public bool ExtinguishOnly;
        [Tooltip("If set, projected objects will be set to fully active or fully disabled instantly instead of smoothly fading lights/renderers/colliders. Use this if the normal behavior is insufficient for the objects you're using.")]
        public bool ToggleProjectedObjectsActive;
        [Tooltip("A condition to set when the totem is lit.")]
        public DreamLightConditionConfig Condition;
        [Tooltip("A relative path from this planet to an alarm totem that will be activated or deactivated based on whether this totem is lit. Used if no alarm totem is linked directly.")]
        public string PathToAlarmTotem;
        [Tooltip("Additional relative paths from this planet to objects containing dream candles that will be activated or deactivated based on whether this totem is lit. All dream candles in the selected objects will be connected to this totem.")]
        public List<string> PathsToDreamCandles = new();
        [Tooltip("Additional relative paths from this planet to projection totems that will be deactivated if this totem is extinguished. All projection totems in the selected objects will be connected to this totem.")]
        public List<string> PathsToProjectionTotems = new();
        [Tooltip("Additional relative paths from this planet to objects that will appear or disappear when this totem is lit or extinguished. Some types of objects and effects are not supported and will remain visible and active.")]
        public List<string> PathsToProjectedObjects = new();

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (StartLit)
                writer.WriteProperty("startLit", StartLit);
            if (ExtinguishOnly)
                writer.WriteProperty("extinguishOnly", ExtinguishOnly);
            if (ToggleProjectedObjectsActive)
                writer.WriteProperty("toggleProjectedObjectsActive", ToggleProjectedObjectsActive);
            Condition.WriteJsonProperty(writer, "condition");
        }

        public void WriteLinks(PropContext context, JsonTextWriter writer, IProp alarmTotem, IEnumerable<IProp> dreamCandles, IEnumerable<IProp> projectionTotems, IEnumerable<string> projectedObjectPaths)
        {
            var alarmTotemPath = alarmTotem != null ? AssetRepository.GetPropPlanetPath<AlarmTotemPropData>(context.Planet, alarmTotem) : PathToAlarmTotem;
            if (!string.IsNullOrEmpty(alarmTotemPath))
                writer.WriteProperty("pathToAlarmTotem", alarmTotemPath);
            var dreamCandlePaths = dreamCandles.Select(c => AssetRepository.GetPropPlanetPath<DreamCandlePropData>(context.Planet, c))
                .Concat(PathsToDreamCandles).Where(p => !string.IsNullOrEmpty(p));
            if (dreamCandlePaths.Any())
                writer.WriteProperty("pathsToDreamCandles", dreamCandlePaths);
            var projectionTotemPaths = projectionTotems.Select(t => AssetRepository.GetPropPlanetPath<ProjectionTotemPropData>(context.Planet, t))
                .Concat(PathsToProjectionTotems).Where(p => !string.IsNullOrEmpty(p));
            if (projectionTotemPaths.Any())
                writer.WriteProperty("pathsToProjectionTotems", projectionTotemPaths);
            var allProjectedObjectPaths = projectedObjectPaths.Concat(PathsToProjectedObjects).Where(p => !string.IsNullOrEmpty(p));
            if (allProjectedObjectPaths.Any())
                writer.WriteProperty("pathsToProjectedObjects", allProjectedObjectPaths);
        }

        public void ValidateLinks(PropContext context, IAssetValidator validator, IProp alarmTotem, IEnumerable<IProp> dreamCandles, IEnumerable<IProp> projectionTotems)
        {
            if (alarmTotem != null && AssetRepository.GetPropContext<AlarmTotemPropData>(context.Planet, alarmTotem) == null)
                validator.Error(context.Planet, $"Projection totem '{context.GetProp().PropName}' is linked to alarm totem '{alarmTotem.PropName}', which is not on the same planet");
            foreach (var candle in dreamCandles.Where(c => AssetRepository.GetPropContext<DreamCandlePropData>(context.Planet, c) == null))
                validator.Error(context.Planet, $"Projection totem '{context.GetProp().PropName}' is linked to dream candle '{candle.PropName}', which is not on the same planet");
            foreach (var totem in projectionTotems.Where(t => AssetRepository.GetPropContext<ProjectionTotemPropData>(context.Planet, t) == null))
                validator.Error(context.Planet, $"Projection totem '{context.GetProp().PropName}' is linked to projection totem '{totem.PropName}', which is not on the same planet");
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(ProjectionTotemPropAsset))]
    public class ProjectionTotemPropAsset : GeneralPropAsset<ProjectionTotemPropData>
    {
        [Tooltip("An alarm totem that will be activated or deactivated based on whether this totem is lit.")]
        public AlarmTotemPropAsset AlarmTotem;
        [Tooltip("Dream candles that will be activated or deactivated based on whether this totem is lit.")]
        public List<DreamCandlePropAsset> DreamCandles = new();
        [Tooltip("Projection totems that will be deactivated if this totem is extinguished.")]
        public List<ProjectionTotemPropAsset> ProjectionTotems = new();
        [Tooltip("Details that will appear or disappear when this totem is lit or extinguished.")]
        public List<DetailPropAsset> ProjectedDetails = new();

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            Data.WriteLinks(context, writer, AlarmTotem ? AlarmTotem : null, DreamCandles.Where(c => c), ProjectionTotems.Where(t => t),
                ProjectedDetails.Where(d => d).Select(d => AssetRepository.GetPropPlanetPath<DetailPropData>(context.Planet, d)));
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            var context = new PropContext<ProjectionTotemPropData> { Planet = Planet, Prop = this };
            Data.ValidateLinks(context, validator, AlarmTotem ? AlarmTotem : null, DreamCandles.Where(c => c), ProjectionTotems.Where(t => t));
            foreach (var detail in ProjectedDetails.Where(d => d && d.Planet != Planet))
                validator.Error(this, $"Projected detail '{detail.FullID}' is not on the same planet");
        }
    }

    public class ProjectionTotemPropComponent : GeneralPropComponent<ProjectionTotemPropData>
    {
        [Tooltip("An alarm totem that will be activated or deactivated based on whether this totem is lit.")]
        public AlarmTotemPropAsset AlarmTotemAsset;
        [Tooltip("An alarm totem that will be activated or deactivated based on whether this totem is lit.")]
        public AlarmTotemPropComponent AlarmTotem;
        [Tooltip("Dream candles that will be activated or deactivated based on whether this totem is lit.")]
        public List<DreamCandlePropAsset> DreamCandleAssets = new();
        [Tooltip("Dream candles that will be activated or deactivated based on whether this totem is lit.")]
        public List<DreamCandlePropComponent> DreamCandles = new();
        [Tooltip("Projection totems that will be deactivated if this totem is extinguished.")]
        public List<ProjectionTotemPropAsset> ProjectionTotemAssets = new();
        [Tooltip("Projection totems that will be deactivated if this totem is extinguished.")]
        public List<ProjectionTotemPropComponent> ProjectionTotems = new();
        [Tooltip("Details that will appear or disappear when this totem is lit or extinguished.")]
        public List<DetailPropAsset> ProjectedDetailAssets = new();
        [Tooltip("Objects that will appear or disappear when this totem is lit or extinguished. Some types of objects and effects are not supported and will remain visible and active.")]
        public List<Transform> ProjectedObjects = new();

        IProp GetAlarmTotem() => AlarmTotemAsset ? (IProp)AlarmTotemAsset : AlarmTotem ? AlarmTotem : null;
        IEnumerable<IProp> GetDreamCandles() => DreamCandleAssets.Where(c => c).Cast<IProp>().Concat(DreamCandles.Where(c => c));
        IEnumerable<IProp> GetProjectionTotems() => ProjectionTotemAssets.Where(t => t).Cast<IProp>().Concat(ProjectionTotems.Where(t => t));

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            var projectedObjectPaths = ProjectedDetailAssets.Where(d => d).Select(d => AssetRepository.GetPropPlanetPath<DetailPropData>(context.Planet, d))
                .Concat(ProjectedObjects.Where(t => t).Select(t => UnityUtility.ResolvePaths(context.DetailPath + "/" + UnityUtility.GetTransformPath(t, true))));
            ((ProjectionTotemPropData)GetData()).WriteLinks(context, writer, GetAlarmTotem(), GetDreamCandles(), GetProjectionTotems(), projectedObjectPaths);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            ((ProjectionTotemPropData)GetData()).ValidateLinks(context, validator, GetAlarmTotem(), GetDreamCandles(), GetProjectionTotems());
        }
    }
}
