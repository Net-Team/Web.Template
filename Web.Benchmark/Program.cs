using BenchmarkDotNet.Running;
using System;

namespace Web.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<DemoContext>();
            Console.ReadLine();
        }
    }
}
