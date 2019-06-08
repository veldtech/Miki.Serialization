using System;
using System.IO;
using System.Threading.Tasks;
using ProtoBuf;

namespace Miki.Serialization.Protobuf
{
	public class ProtobufSerializer : ISerializer
    {
        public byte[] Serialize<T>(T data)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, data);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			{
				return Serializer.Deserialize<T>(ms);
			}
		}

        public Task SerializeAsync<T>(Stream stream, T data)
        {
            Serializer.Serialize(stream, data);
            return Task.CompletedTask;
        }

        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            return Task.FromResult(Serializer.Deserialize<T>(stream));
        }
	}
}
