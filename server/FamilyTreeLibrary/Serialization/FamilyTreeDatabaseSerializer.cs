using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization.Models;
using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace FamilyTreeLibrary.Serialization
{
    public class FamilyTreeDatabaseSerializer : CosmosSerializer
    {
        private readonly JsonSerializerOptions options;

        public FamilyTreeDatabaseSerializer()
        {
            options = new()
            {
                Converters = {
                    new BridgeSerializer()
                },
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public override T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                using StreamReader reader = new(stream);
                string json = reader.ReadToEnd();
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
                IBridge bridge = JsonSerializer.Deserialize<IBridge>(json, options) ?? throw new ArgumentException($"{typeof(T).Name} doesn't exist.");
                if (bridge.Instance.TryGetBoolean(out bool b))
                {
                    return (T)(object)b;
                }
                else if (bridge.Instance.TryGetString(out string str))
                {
                    return (T)(object)str;
                }
                else if (bridge.Instance.TryGetArray(out IEnumerable<BridgeInstance> array))
                {
                    Type type = typeof(T);
                    if (type.IsAssignableTo(typeof(IEnumerable<Person>)))
                    {
                        return (T)(object)array.Select(x => new Person(x.AsObject)).ToArray();
                    }
                    else if (type.IsAssignableTo(typeof(IEnumerable<FamilyDynamic>)))
                    {
                        return (T)(object)array.Select(x => new FamilyDynamic(x.AsObject)).ToArray() ;
                    }
                    return (T)(object)array;
                }
                else if (bridge.Instance.TryGetObject(out IDictionary<string,BridgeInstance> obj))
                {
                    Type type = typeof(T);
                    if (type == typeof(Person))
                    {
                        return (T)(object)new Person(obj);
                    }
                    else if (type == typeof(FamilyDynamic))
                    {
                        return (T)(object)new FamilyDynamic(obj);
                    }
                    return (T)(object)array;
                }
                else if (bridge.Instance.TryGetNumber(out Number num))
                {
                    Type type = typeof(T);
                    if (type == typeof(int))
                    {
                        return (T)(object)num.AsInt;
                    }
                    else if (type == typeof(long))
                    {
                        return (T)(object)num.AsLong;
                    }
                    return (T)(object)num.AsDouble;
                }
                return (T)(object)new BridgeInstance();
            }
        }

        public override Stream ToStream<T>(T input)
        {
            MemoryStream stream = new();
            if (input is IBridge bridge)
            {
                string json = JsonSerializer.Serialize(bridge, options);
                StreamWriter writer = new(stream);
                writer.Write(json);
                writer.Flush();
                stream.Position = 0;
            }
            return stream;
        }
    }
}