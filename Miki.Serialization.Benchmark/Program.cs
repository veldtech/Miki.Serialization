using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using Miki.Serialization.Benchmark.Exporter;

namespace Miki.Serialization.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }

        /// <summary>
        ///     BenchmarkDotNet Configuration
        /// </summary>
        public class Config : ManualConfig
        {
            public Config()
            {
                Add(MemoryDiagnoser.Default);
                Add(CsvMeasurementsExporter.Default);
                Add(CustomRPlotExporter.Default);
            }
        }
    }
}
