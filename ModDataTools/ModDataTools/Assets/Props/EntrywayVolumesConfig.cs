using ModDataTools.Assets.Volumes;
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
    public class EntrywayVolumesConfig
    {
        [Tooltip("Volume assets to add to. They must be on the same planet.")]
        public List<PropDataAsset> VolumeAssets = new();
        [Tooltip("Volume components to add to. They must be on the same planet.")]
        public List<PropDataComponent> Volumes = new();
        [Tooltip("Paths relative to the planet to other trigger volumes to add to, such as the Vessel's oxygen volume.")]
        public List<string> VolumePaths = new();

        IEnumerable<IProp> GetVolumeProps() => VolumeAssets.Where(v => v).Cast<IProp>().Concat(Volumes.Where(v => v));

        public void WriteJsonProperty(PlanetAsset planet, JsonTextWriter writer)
        {
            var paths = GetVolumeProps().Select(v => AssetRepository.GetPropPlanetPath(planet, v))
                .Concat(VolumePaths)
                .Where(p => !string.IsNullOrEmpty(p));
            if (paths.Any())
                writer.WriteProperty("entrywayVolumes", paths);
        }

        public void Validate(PropContext context, PlanetAsset planet, IAssetValidator validator)
        {
            foreach (var volume in GetVolumeProps())
            {
                if (!(volume.GetData() is GeneralVolumeData))
                    validator.Error(context.Planet, $"Entryway volume '{volume.PropName}' on '{context.GetProp().PropName}' is not a volume");
                else if (AssetRepository.GetPropContext(planet, volume) == null)
                    validator.Error(context.Planet, $"Entryway volume '{volume.PropName}' on '{context.GetProp().PropName}' is not on planet '{planet.FullID}'");
            }
        }
    }
}
