using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModDataTools.Utilities
{
    public interface IJsonAsset
    {
        public void ToJson(JsonTextWriter writer);
        public string ToJsonString();
    }
}
