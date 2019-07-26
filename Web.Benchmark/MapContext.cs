using BenchmarkDotNet.Attributes;
using Core;
using System;

namespace Web.Benchmark
{
    public class MapContext
    {
        private A a = new A();


        [Benchmark]
        public void Map()
        {
            a.AsMap().To<B>();
        }


        [Benchmark]
        public void EmitMap()
        {
            a.AsEmitMap().To<B>();
        }

        [Benchmark]
        public void MapIgnore()
        {
            a.AsMap().Ignore(i => i.Name).To<B>();
        }

        [Benchmark]
        public void EmitMapIgnore()
        {
            a.AsEmitMap().Ignore(i => i.Name).To<B>();
        }
    }

    public class A
    {
        public string Name { get; set; } = "A";

        public int? Age { get; set; } = 9;

        public string Email { get; set; } = "@A";
    }

    public class B
    {
        public string Name { get; set; } = "B";

        public int Age { get; set; } = 100;

        public string Email { get; set; } = "@B";
    }
}
