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
    public class QuantumToggleVolumeData : GeneralVolumeData
    {
        [Tooltip("Paths to additional quantum objects to toggle when entering the volume.")]
        public List<string> QuantumObjectPaths = new();
        [Tooltip("Invert the toggle so it starts on but turns off when entering volume.")]
        public bool Invert;
        [Tooltip("Whether exiting the volume will undo the toggle that happened when entering it.")]
        public bool UndoOnExit = true;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Invert)
                writer.WriteProperty("invert", Invert);
            if (!UndoOnExit)
                writer.WriteProperty("undoOnExit", UndoOnExit);
        }

        public void WriteQuantumObjects(PropContext context, JsonTextWriter writer, IEnumerable<IProp> quantumGroups)
        {
            var paths = quantumGroups.SelectMany(g => QuantumGroupPropData.GetQuantumObjectPaths(context, g))
                .Concat(QuantumObjectPaths.Where(p => !string.IsNullOrEmpty(p)));
            if (paths.Any())
                writer.WriteProperty("quantumObjects", paths);
        }

        public void ValidateQuantumObjects(PropContext context, IAssetValidator validator, IEnumerable<IProp> quantumGroups)
        {
            foreach (var group in quantumGroups)
                if (!QuantumGroupPropData.GetQuantumObjectPaths(context, group).Any())
                    validator.Error(context.Planet, $"Quantum toggle volume '{context.GetProp().PropName}' references quantum group '{group.PropName}', which is not on the same planet or has no details");
            if (!quantumGroups.Any() && !QuantumObjectPaths.Any(p => !string.IsNullOrEmpty(p)))
                validator.Error(context.Planet, $"Quantum toggle volume '{context.GetProp().PropName}' has no quantum objects");
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(QuantumToggleVolumeAsset))]
    public class QuantumToggleVolumeAsset : GeneralVolumeAsset<QuantumToggleVolumeData>
    {
        [Tooltip("The quantum groups whose quantum objects will be toggled when entering the volume.")]
        public List<QuantumGroupPropAsset> QuantumGroups = new();

        IEnumerable<IProp> GetQuantumGroups() => QuantumGroups.Where(g => g);

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            Data.WriteQuantumObjects(context, writer, GetQuantumGroups());
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            Data.ValidateQuantumObjects(new PropContext<QuantumToggleVolumeData> { Planet = Planet, Prop = this }, validator, GetQuantumGroups());
        }
    }

    public class QuantumToggleVolumeComponent : GeneralVolumeComponent<QuantumToggleVolumeData>
    {
        [Tooltip("The quantum groups whose quantum objects will be toggled when entering the volume.")]
        public List<QuantumGroupPropAsset> QuantumGroupAssets = new();
        [Tooltip("The quantum groups whose quantum objects will be toggled when entering the volume.")]
        public List<QuantumGroupPropComponent> QuantumGroups = new();

        IEnumerable<IProp> GetQuantumGroups() => QuantumGroupAssets.Where(g => g).Cast<IProp>().Concat(QuantumGroups.Where(g => g));

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            ((QuantumToggleVolumeData)GetData()).WriteQuantumObjects(context, writer, GetQuantumGroups());
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            ((QuantumToggleVolumeData)GetData()).ValidateQuantumObjects(context, validator, GetQuantumGroups());
        }
    }
}
