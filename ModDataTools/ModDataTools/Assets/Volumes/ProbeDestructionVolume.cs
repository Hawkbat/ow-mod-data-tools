﻿using ModDataTools.Assets.Props;
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
    public class ProbeDestructionVolumeData : GeneralVolumeData
    {

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(ProbeDestructionVolumeAsset))]
    public class ProbeDestructionVolumeAsset : GeneralVolumeAsset<ProbeDestructionVolumeData> { }
    public class ProbeDestructionVolumeComponent : GeneralVolumeComponent<ProbeDestructionVolumeData> { }
}
