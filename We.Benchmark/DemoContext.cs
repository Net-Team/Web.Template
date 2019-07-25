using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace We.Benchmark
{
    public class DemoContext
    {
        [Benchmark]
        public void Method1()
        {
        }

        [Benchmark]
        public void Method2()
        {
        }
    }
}
