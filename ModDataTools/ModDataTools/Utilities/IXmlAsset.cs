using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ModDataTools.Utilities
{
    public interface IXmlAsset
    {
        public void ToXml(XmlWriter writer);
        public string ToXmlString();
    }
}
