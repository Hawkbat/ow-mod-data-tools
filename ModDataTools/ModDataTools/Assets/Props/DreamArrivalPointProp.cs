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
    public class DreamArrivalPointPropData : GeneralPropData
    {
        [Tooltip("Whether to generate simulation meshes (the models used in the \"tronworld\" or \"matrix\" view) for most objects on the current planet by cloning the existing meshes and applying the simulation materials. Leave this off if you are building your own simulation meshes or using existing objects which have them.")]
        public bool GenerateSimulationMeshes;
        [Tooltip("Volumes to explicitly add the player to when arriving at this point.")]
        public EntrywayVolumesConfig EntrywayVolumes;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("id", context.GetProp().PropID);
            if (GenerateSimulationMeshes)
                writer.WriteProperty("generateSimulationMeshes", GenerateSimulationMeshes);
            EntrywayVolumes.WriteJsonProperty(context.Planet, writer);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            EntrywayVolumes.Validate(context, context.Planet, validator);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(DreamArrivalPointPropAsset))]
    public class DreamArrivalPointPropAsset : GeneralPropAsset<DreamArrivalPointPropData> { }
    public class DreamArrivalPointPropComponent : GeneralPropComponent<DreamArrivalPointPropData> { }
}
