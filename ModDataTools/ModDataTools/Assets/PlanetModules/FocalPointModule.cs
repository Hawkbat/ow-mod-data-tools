using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.PlanetModules
{
    [Serializable]
    public class FocalPointModule : PlanetModule
    {
        [Tooltip("The primary planet in this binary system")]
        public PlanetAsset Primary;
        [Tooltip("The secondary planet in this binary system")]
        public PlanetAsset Secondary;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            writer.WriteProperty("primary", Primary.FullID);
            writer.WriteProperty("secondary", Secondary.FullID);
        }
    }
}
