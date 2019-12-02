using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Web.Test.Corefx
{
    public class IEnumerableExtensionsTest
    {
        class A
        {
            public string Name { get; set; }
        }

        [Fact]
        public void Distinct()
        {
            var a = new A { Name = "a" };
            var b = new A { Name = "a" };
            var c = new A { Name = "c" };

            var v = new[] { a, b, c }.Distinct(item => item.Name).ToArray();
            Assert.True(v.Length == 2);
            Assert.True(v.Last().Name == "c");
        }
    }
}
