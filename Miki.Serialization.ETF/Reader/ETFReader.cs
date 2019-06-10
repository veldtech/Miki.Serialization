using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Miki.Serialization.ETF.Reader
{
    public class ETFReader : IDisposable
    {
        private MemoryStream _stream;

        private bool _isDisposed;
        private int _offset;

        public ETFReader(byte[] packet)
        {
        }  

        private byte[] ReadBytes(int num)
        {
            byte[] val = new byte[num];
            var bytesRead = _stream.Read(val, _offset, num);
            if(bytesRead != num)
            {
                throw new EndOfStreamException();
            }
            _offset += num;
            return val;
        }

        public void Dispose()
        {
            _stream.Dispose();
            _isDisposed = true;
        }
    }
}
