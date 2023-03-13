using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine.Assertions;

namespace ModDataTools.Utilities
{
    public static class ExportUtility
    {
        public static string ToJsonString(IJsonAsset asset)
        {
            using var stringWriter = new Utf8StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
            asset.ToJson(jsonWriter);
            jsonWriter.Close();
            return stringWriter.ToString();
        }
        public static string ToXmlString(IXmlAsset asset)
        {
            using var stringWriter = new Utf8StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings()
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = true,
            });
            xmlWriter.WriteStartDocument();
            asset.ToXml(xmlWriter);
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
