using QMapper;
using Xunit;

namespace Web.Test.Common
{
    public class MapTest
    {
        [Fact]
        public void Map()
        {
            var a = new A();
            var b = new A().AsMap().To<B>();

            Assert.Equal(a.Age, b.Age);
            Assert.Equal(a.Email, b.Email);
            Assert.Equal(a.Name, b.Name);
        }


        [Fact]
        public void MapIgnore()
        {
            var a = new A();
            var b = new A().AsMap().Ignore(i => i.Name).To<B>();

            Assert.Equal(a.Age, b.Age);
            Assert.Equal(a.Email, b.Email);
            Assert.NotEqual(a.Name, b.Name);
        }

        class A
        {
            public string Name { get; set; } = "A";

            public int? Age { get; set; } = 9;

            public string Email { get; set; } = "@A";
        }

        class B
        {
            public string Name { get; set; } = "B";

            public int Age { get; set; } = 100;

            public string Email { get; set; } = "@B";
        }
    }
}
