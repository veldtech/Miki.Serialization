using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Miki.Serialization.JsonNet
{
    public class JsonNetSerializer : ISerializer
    {
        private readonly JsonSerializer _serializer;

        public JsonNetSerializer()
        {
            _serializer = new JsonSerializer();
        }

        public JsonNetSerializer(JsonSerializerSettings serializerSettings)
        {
            _serializer = JsonSerializer.Create(serializerSettings);
        }

        public byte[] Serialize<T>(T data)
        {
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                _serializer.Serialize(jsonWriter, data);
                jsonWriter.Flush();

                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new StreamReader(ms))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return _serializer.Deserialize<T>(jsonReader);
            }
        }

        public async Task SerializeAsync<T>(Stream stream, T data)
        {
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                _serializer.Serialize(jsonWriter, data);
                await jsonWriter.FlushAsync();
            }
        }

        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return Task.FromResult(_serializer.Deserialize<T>(jsonReader));
            }
        }
    }
}
