using System;
using Miki.Serialization;

namespace Miki.Serialization.ETF
{
    public class ETFSerializer : ISerializer
    {
        public T Deserialize<T>(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize<T>(T data)
        {
            throw new NotImplementedException();
        }
    }
}
