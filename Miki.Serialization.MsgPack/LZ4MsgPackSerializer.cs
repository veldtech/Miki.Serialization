using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Serialization.MsgPack
{
	public class LZ4MsgPackSerializer : ISerializer
	{
		public T Deserialize<T>(byte[] data)
		{
			return LZ4MessagePackSerializer.Deserialize<T>(data);
        }

        public byte[] Serialize<T>(T data)
		{
			return LZ4MessagePackSerializer.Serialize(data);
        }

        public Task SerializeAsync<T>(Stream stream, T data)
        {
            LZ4MessagePackSerializer.Serialize(stream, data);
            return Task.CompletedTask;
        }

        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            return Task.FromResult(LZ4MessagePackSerializer.Deserialize<T>(stream));
        }
    }
}
