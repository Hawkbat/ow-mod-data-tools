using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModDataTools.Utilities
{
    public interface IJsonSerializable
    {
        public void ToJson(JsonTextWriter writer);
    }
}
