using BenchmarkDotNet.Running;
using System;

namespace We.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MapContext>();
            Console.ReadLine();
        }
    }
}
