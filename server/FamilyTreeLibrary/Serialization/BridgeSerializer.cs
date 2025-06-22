using FamilyTreeLibrary.Serialization.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FamilyTreeLibrary.Serialization
{
    public class BridgeSerializer : JsonConverter<IBridge>
    {
        public override bool HandleNull => true;
        public override IBridge Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            BridgeInstance instance = ReadInstance(ref reader);
            if (instance.TryGetArray(out IEnumerable<BridgeInstance> array))
            {
                return new Bridge(array);
            }
            else if (instance.TryGetBoolean(out bool b))
            {
                return new Bridge(b);
            }
            else if (instance.TryGetNumber(out Number num))
            {
                return new Bridge(num);
            }
            else if (instance.TryGetString(out string s))
            {
                return new Bridge(s);
            }
            else if (instance.TryGetObject(out IDictionary<string,BridgeInstance> obj))
            {
                return new Bridge(obj);
            }
            return new Bridge();
        }

        public override void Write(Utf8JsonWriter writer, IBridge value, JsonSerializerOptions options)
        {
            WriteInstance(writer, value.Instance);
        }

        private static BridgeInstance ReadInstance(ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString() ?? throw new JsonException("The string wasn't found");
                return new(value);
            }
            else if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            {
                return new(reader.GetBoolean());
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return new(new Number(reader.GetDouble()));
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                ICollection<BridgeInstance> instances = [];
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    instances.Add(ReadInstance(ref reader));
                }
                reader.Read();
                return new(instances);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                Dictionary<string, BridgeInstance> obj = [];
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        string attribute = reader.GetString() ?? throw new JsonException("The property wasn't found");
                        reader.Read();
                        obj[attribute] = ReadInstance(ref reader);
                    }
                }
                reader.Read();
                return new(obj);
            }
            return new();
        }

        private static void WriteInstance(Utf8JsonWriter writer, BridgeInstance instance)
        {
            if (instance.TryGetArray(out IEnumerable<BridgeInstance> array))
            {
                writer.WriteStartArray();
                foreach (BridgeInstance element in array)
                {
                    WriteInstance(writer, element);
                }
                writer.WriteEndArray();
            }
            else if (instance.TryGetBoolean(out bool b))
            {
                writer.WriteBooleanValue(b);
            }
            else if (instance.TryGetNumber(out Number num))
            {
                if (num.TryGetInt(out int i))
                {
                    writer.WriteNumberValue(i);
                }
                else if (num.TryGetLong(out long l))
                {
                    writer.WriteNumberValue(l);
                }
                else
                {
                    writer.WriteNumberValue(num.AsDouble);
                }
            }
            else if (instance.TryGetString(out string s))
            {
                writer.WriteStringValue(s);
            }
            else if (instance.TryGetObject(out IDictionary<string,BridgeInstance> obj))
            {
                writer.WriteStartObject();
                foreach (KeyValuePair<string, BridgeInstance> pair in obj)
                {
                    writer.WritePropertyName(pair.Key);
                    WriteInstance(writer, pair.Value);
                }
                writer.WriteEndObject();
            }
            else if (instance.IsNull)
            {
                writer.WriteNullValue();
            }
        }
    }
}