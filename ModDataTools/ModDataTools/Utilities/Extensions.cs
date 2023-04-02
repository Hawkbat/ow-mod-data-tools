using ModDataTools.Assets.PlanetModules;
using ModDataTools.Assets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;
using ModDataTools.Assets.Props;

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
        public static void WriteProperty(this JsonTextWriter writer, string name, IJsonSerializable asset)
        {
            writer.WritePropertyName(name);
            asset.ToJson(writer);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, IEnumerable<IJsonSerializable> assets)
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var asset in assets)
                asset.ToJson(writer);
            writer.WriteEndArray();
        }
        public static void WriteProperty<T>(this JsonTextWriter writer, string name, PropContext<T> prop) where T : PropData
        {
            writer.WritePropertyName(name);
            writer.WriteStartObject();
            prop.Prop.WriteJsonProps(prop, writer);
            writer.WriteEndObject();
        }
        public static void WriteProperty<T>(this JsonTextWriter writer, string name, IEnumerable<PropContext<T>> props) where T : PropData
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var prop in props)
            {
                writer.WriteStartObject();
                prop.Prop.WriteJsonProps(prop, writer);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, PlanetModule module, PlanetAsset planet)
        {
            if (!module.ShouldWrite(planet)) return;
            writer.WritePropertyName(name);
            module.WriteJsonObject(planet, writer);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, IEnumerable<PlanetModule> modules, PlanetAsset planet)
        {
            if (!modules.Any(m => m.ShouldWrite(planet))) return;
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var module in modules.Where(m => m.ShouldWrite(planet)))
                module.WriteJsonObject(planet, writer);
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
        public static void WriteProperty(this JsonTextWriter writer, string name, NullishSingle value)
        {
            if (!value.HasValue) return;
            writer.WriteProperty(name, value.Value);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, NullishVector3 value)
        {
            if (!value.HasValue) return;
            writer.WriteProperty(name, value.Value);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, NullishColor value)
        {
            if (!value.HasValue) return;
            writer.WriteProperty(name, (Color32)value.Value);
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, AnimationCurve curve)
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var keyframe in curve.keys)
            {
                writer.WriteStartObject();
                writer.WriteProperty("time", keyframe.time);
                writer.WriteProperty("value", keyframe.value);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
        public static void WriteProperty(this JsonTextWriter writer, string name, Gradient gradient)
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            var times = new List<float>();
            foreach (var keyframe in gradient.colorKeys)
                times.Add(keyframe.time);
            foreach (var keyframe in gradient.alphaKeys)
                times.Add(keyframe.time);
            foreach (var time in times.OrderBy(t => t).Distinct())
            {
                writer.WriteStartObject();
                writer.WriteProperty("time", time);
                writer.WriteProperty("value", (Color32)gradient.Evaluate(time));
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
        public static void WriteProperty<T>(this JsonWriter writer, string name, T value, bool camelCase = true) where T : struct, Enum
        {
            var s = value.ToString();
            if (camelCase)
            {
                if (s.Length > 1) s = s.Substring(0, 1).ToLower() + s.Substring(1);
                else if (s.Length == 1) s = s.ToLower();
            }
            writer.WritePropertyName(name);
            writer.WriteValue(s);
        }
        public static bool IsUniform(this Vector3 vec) => vec.x == vec.y && vec.y == vec.z;
        public static float Average(this Vector3 vec) => (vec.x + vec.y + vec.z) / 3f;
    }
}
