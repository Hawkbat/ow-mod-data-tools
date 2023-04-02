using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public abstract class GeneralSolarSystemPropData : GeneralPropData
    {
    }

    public abstract class GeneralSolarSystemPropAsset<T> : GeneralPropAsset<T> where T : GeneralSolarSystemPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("parentBody", context.Planet.FullID);
            base.WriteJsonProps(context, writer);
        }
    }

    public abstract class GeneralSolarSystemPropComponent<T> : GeneralPropComponent<T> where T : GeneralSolarSystemPropData
    {
        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            writer.WriteProperty("parentBody", context.Planet.FullID);
            base.WriteJsonProps(context, writer);
        }
    }
}
