using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

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
	}
}
