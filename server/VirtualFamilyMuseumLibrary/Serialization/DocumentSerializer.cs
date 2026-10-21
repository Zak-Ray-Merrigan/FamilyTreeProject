using Microsoft.Azure.Cosmos;
using System.Text.Json;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibrary.Serialization
{
    public class DocumentSerializer : CosmosSerializer
    {
        private static readonly JsonSerializerOptions options = SerializationExtensions.GetOptions(false);
        
        public override T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(T) == typeof(IBridge))
                {
                    IBridge? bridge = JsonSerializer.Deserialize<IBridge>(stream, options);
                    return (T)bridge!;
                }
                else if (typeof(T) == typeof(IBridge[]))
                {
                    IList<BridgeValue> elements = JsonSerializer.Deserialize<IBridge>(stream, options)!.Value.AsArray;
                    return (T)(object)elements.Select<BridgeValue, IBridge>(bValue => new Bridge(bValue)).ToArray();
                }
                throw new NotSupportedException($"{typeof(T).Name} isn't supported; read as \"IBridge\" or \"IBridge[]\".");
            }
        }

        public override Stream ToStream<T>(T input)
        {
            if (input is not IBridge bridge)
            {
                throw new NotSupportedException($"{input?.GetType().Name ?? "null"} doesn't implement {nameof(IBridge)}.");
            }
            Stream output = new MemoryStream();
            JsonSerializer.Serialize(output, bridge, options);
            output.Position = 0;
            return output;
        }
    }
}