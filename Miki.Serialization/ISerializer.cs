using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Serialization
{
    public interface ISerializer
    {
		byte[] Serialize<T>(T data);

        T Deserialize<T>(byte[] data);

        Task SerializeAsync<T>(Stream stream, T data);

        Task<T> DeserializeAsync<T>(Stream stream);
    }
}
