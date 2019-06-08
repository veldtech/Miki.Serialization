using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SpanJson;

namespace Miki.Serialization.SpanJson
{
    public class SpanJsonSerializer : ISerializer
    {
        public byte[] Serialize<T>(T data)
        {
            return JsonSerializer.Generic.Utf8.Serialize(data);
        }

        public T Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Generic.Utf8.Deserialize<T>(data);
        }

        public Task SerializeAsync<T>(Stream stream, T data)
        {
            return JsonSerializer.Generic.Utf8.SerializeAsync(data, stream).AsTask();
        }

        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            return JsonSerializer.Generic.Utf8.DeserializeAsync<T>(stream).AsTask();
        }
    }
}
