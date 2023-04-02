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
    public class FluidVolumeData : GeneralPriorityVolumeData
    {
        [Tooltip("The type of fluid for this volume.")]
        public FluidType Type = FluidType.None;
        [Tooltip("Density of the fluid. The higher the density, the harder it is to go through this fluid.")]
        public float Density = 1.2f;
        [Tooltip("Should the player and rafts align to this fluid.")]
        public bool AlignmentFluid = true;
        [Tooltip("Should the ship align to the fluid by rolling.")]
        public bool AllowShipAutoroll;
        [Tooltip("Disable this fluid volume immediately?")]
        public bool DisableOnStart;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Density != 1.2f)
                writer.WriteProperty("density", Density);
            if (Type != FluidType.None)
                writer.WriteProperty("type", Type);
            if (!AlignmentFluid)
                writer.WriteProperty("alignmentFluid", AlignmentFluid);
            if (AllowShipAutoroll)
                writer.WriteProperty("allowShipAutoroll", AllowShipAutoroll);
            if (DisableOnStart)
                writer.WriteProperty("disableOnStart", DisableOnStart);
        }

        public enum FluidType
        {
            None = 0,
            Water = 1,
            Cloud = 2,
            Sand = 3,
            Plasma = 4,
            Fog = 5,
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(FluidVolumeAsset))]
    public class FluidVolumeAsset : GeneralPriorityVolumeAsset<FluidVolumeData> { }
    public class FluidVolumeComponent : GeneralPriorityVolumeComponent<FluidVolumeData> { }
}
