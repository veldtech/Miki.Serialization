﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Serialization
{
    public interface ISerializer
    {
		byte[] Serialize<T>(T data);
		T Deserialize<T>(byte[] data);
    }
}