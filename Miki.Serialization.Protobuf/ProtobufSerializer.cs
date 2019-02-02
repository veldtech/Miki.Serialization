using System;
using System.IO;
using ProtoBuf;

namespace Miki.Serialization.Protobuf
{
	public class ProtobufSerializer : ISerializer
	{
		public T Deserialize<T>(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			{
				return Serializer.Deserialize<T>(ms);
			}
		}

		public byte[] Serialize<T>(T data)
		{
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, data);
				return ms.ToArray();
			}
		}
	}
}
