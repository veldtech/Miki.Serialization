using System;
using MessagePack;

namespace Miki.Serialization.MsgPack
{
	public class MsgPackSerializer : ISerializer
	{
		public T Deserialize<T>(byte[] data)
		{
			return MessagePackSerializer.Deserialize<T>(data);
		}

		public byte[] Serialize<T>(T data)
		{
			return MessagePackSerializer.Serialize(data);
		}
	}
}
