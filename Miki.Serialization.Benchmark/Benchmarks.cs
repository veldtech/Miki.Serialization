using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Miki.Serialization.SpanJson;
using Miki.Serialization.JsonNet;
using Miki.Serialization.MsgPack;
using Miki.Serialization.Protobuf;

namespace Miki.Serialization.Benchmark
{
	[Config(typeof(Program.Config))]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class SerializeBenchmarks
    {
		private TestClass Data;

        private readonly ISerializer SpanJsonSerializer = new SpanJsonSerializer();
        private readonly ISerializer JsonNetSerializer = new JsonNetSerializer();
        private readonly ISerializer MsgPackSerializer = new MsgPackSerializer();
        private readonly ISerializer ProtobufSerializer = new ProtobufSerializer();

        [GlobalSetup]
        public void Setup()
        {
            Data = new TestClass {Foo = "bar"};
        }

		[Benchmark]
        public byte[] SpanJson()
        {
            return SpanJsonSerializer.Serialize(Data);
        }

		[Benchmark]
        public byte[] JsonNet()
        {
            return JsonNetSerializer.Serialize(Data);
        }

		[Benchmark]
        public byte[] MsgPack()
        {
            return MsgPackSerializer.Serialize(Data);
        }

		[Benchmark]
        public byte[] Protobuf()
        {
            return ProtobufSerializer.Serialize(Data);
        }
	}

	[Config(typeof(Program.Config))]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class DeserializeBenchmarks
    {
		private TestClass Data;

        private readonly ISerializer SpanJsonSerializer = new SpanJsonSerializer();
        private byte[] SpanJsonData;
        private readonly ISerializer JsonNetSerializer = new JsonNetSerializer();
        private byte[] JsonNetData;
        private readonly ISerializer MsgPackSerializer = new MsgPackSerializer();
        private byte[] MsgPackData;
        private readonly ISerializer ProtobufSerializer = new ProtobufSerializer();
        private byte[] ProtobufData;

        [GlobalSetup]
        public void Setup()
        {
            Data = new TestClass {Foo = "bar"};
			SpanJsonData = SpanJsonSerializer.Serialize(Data);
			JsonNetData = JsonNetSerializer.Serialize(Data);
			MsgPackData = MsgPackSerializer.Serialize(Data);
			ProtobufData = ProtobufSerializer.Serialize(Data);
        }

		[Benchmark]
        public TestClass SpanJson()
        {
            return SpanJsonSerializer.Deserialize<TestClass>(SpanJsonData);
        }

		[Benchmark]
        public TestClass JsonNet()
        {
            return JsonNetSerializer.Deserialize<TestClass>(JsonNetData);
        }

		[Benchmark]
        public TestClass MsgPack()
        {
            return MsgPackSerializer.Deserialize<TestClass>(MsgPackData);
        }

		[Benchmark]
        public TestClass Protobuf()
        {
            return ProtobufSerializer.Deserialize<TestClass>(ProtobufData);
        }
	}
}