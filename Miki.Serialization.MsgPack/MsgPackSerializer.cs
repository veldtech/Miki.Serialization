using System.IO;
using System.Threading.Tasks;
using MessagePack;

namespace Miki.Serialization.MsgPack
{
	public class MsgPackSerializer : ISerializer
    {
        public byte[] Serialize<T>(T data)
        {
            return MessagePackSerializer.Serialize(data);
        }

        public T Deserialize<T>(byte[] data)
		{
			return MessagePackSerializer.Deserialize<T>(data);
		}

        public Task SerializeAsync<T>(Stream stream, T data)
        {
            return MessagePackSerializer.SerializeAsync(stream, data);
        }

        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            return MessagePackSerializer.DeserializeAsync<T>(stream);
        }
    }
}
