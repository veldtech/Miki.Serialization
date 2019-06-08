using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Miki.Serialization.Benchmark
{
    [DataContract]
    public class TestClass
    {
        [DataMember(Name = "foo", Order = 1)]
        public virtual string Foo { get; set; }
    }
}
