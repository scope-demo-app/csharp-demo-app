using System;
using BenchmarkDotNet.Running;

namespace csharp_demo_app.benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}