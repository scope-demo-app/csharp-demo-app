using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using ScopeAgent.BenchmarkDotNet;

namespace csharp_demo_app.benchmark
{
    [ScopeExporter]
    [MemoryDiagnoser]
    [SimpleJob]
    public class Benchmarks
    {
        private static MemoryStream _smallSource;
        private static MemoryStream _mediumSource;
        private static MemoryStream _largeSource;
        
        public Benchmarks()
        {
            var smallBuffer = new byte[1024];
            var mediumBuffer = new byte[1024*1024];
            var largeBuffer = new byte[1024*1024*1024];
            
            var rnd = new Random();
            rnd.NextBytes(smallBuffer);
            rnd.NextBytes(mediumBuffer);
            rnd.NextBytes(largeBuffer);
            
            _smallSource = new MemoryStream(smallBuffer);
            _mediumSource = new MemoryStream(mediumBuffer);
            _largeSource = new MemoryStream(largeBuffer);
        }
        
        [Benchmark]
        public void Benchmark_SmallStreamToByteArray()
        {
            _ = Utils.GetBytesFromStreamAsync(_smallSource).GetAwaiter().GetResult();
        }
        
        [Benchmark]
        public void Benchmark_MediumStreamToByteArray()
        {
            _ = Utils.GetBytesFromStreamAsync(_mediumSource).GetAwaiter().GetResult();
        }
        
        [Benchmark]
        public void Benchmark_LargeStreamToByteArray()
        {
            _ = Utils.GetBytesFromStreamAsync(_largeSource).GetAwaiter().GetResult();
        }
    }
}