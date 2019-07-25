using BenchmarkDotNet.Attributes;
using Core;
namespace We.Benchmark
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
        public void MapIgnore()
        {
            a.AsMap().Ignore(i => i.Name).To<B>();
        }
    }

    public class A
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Email { get; set; }
    }

    public class B
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Email { get; set; }
    }
}
