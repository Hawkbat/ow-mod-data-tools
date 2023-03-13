using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

namespace ModDataTools.Utilities
{
    public static class Extensions
    {
        public static void WriteEmptyElement(this XmlWriter writer, string localName)
        {
            writer.WriteStartElement(localName);
            writer.WriteEndElement();
        }
        public static void WriteSchemaAttributes(this XmlWriter writer, string schemaPath)
        {
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", XmlSchema.InstanceNamespace, schemaPath);

        }
        public static string ReadElementString(this XmlReader reader, string localName)
        {
            reader.ReadStartElement(localName);
            var value = reader.ReadContentAsString();
            reader.ReadEndElement();
            return value;
        }
        public static bool ReadOptionalEmptyElement(this XmlReader reader, string localName)
        {
            if (reader.IsStartElement(localName))
            {
                reader.ReadStartElement(localName);
                reader.ReadEndElement();
                return true;
            }
            return false;
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, string value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, bool value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, int value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, float value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, IEnumerable<string> values)
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var value in values)
                writer.WriteValue(value);
            writer.WriteEndArray();
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, IEnumerable<int> values)
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var value in values)
                writer.WriteValue(value);
            writer.WriteEndArray();
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, IJsonAsset asset)
        {
            writer.WritePropertyName(name);
            asset.ToJson(writer);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, IEnumerable<IJsonAsset> assets)
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var asset in assets)
                asset.ToJson(writer);
            writer.WriteEndArray();
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, Vector3 value)
        {
            writer.WritePropertyName(name);
            writer.WriteStartObject();
            writer.WriteProperty("x", value.x);
            writer.WriteProperty("y", value.y);
            writer.WriteProperty("z", value.z);
            writer.WriteEndObject();
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, Vector2 value)
        {
            writer.WritePropertyName(name);
            writer.WriteStartObject();
            writer.WriteProperty("x", value.x);
            writer.WriteProperty("y", value.y);
            writer.WriteEndObject();
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, Color32 value)
        {
            writer.WritePropertyName(name);
            writer.WriteStartObject();
            writer.WriteProperty("r", value.r);
            writer.WriteProperty("g", value.g);
            writer.WriteProperty("b", value.b);
            writer.WriteProperty("a", value.a);
            writer.WriteEndObject();
        }
    }
}
