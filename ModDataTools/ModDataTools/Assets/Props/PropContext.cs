using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public abstract class PropContext
    {
        public PlanetAsset Planet;
        public string DetailPath;

        public PropData GetData() => GetProp().GetData();

        public abstract IProp GetProp();

        public PropContext<T> MakeSibling<T>(IProp<T> prop) where T : PropData
        {
            return new PropContext<T>()
            {
                Planet = Planet,
                DetailPath = DetailPath,
                Prop = prop,
            };
        }
    }

    [Serializable]
    public class PropContext<T> : PropContext where T : PropData
    {
        public IProp<T> Prop;
        public T Data => Prop.Data;

        public override IProp GetProp() => Prop;
    }
}
