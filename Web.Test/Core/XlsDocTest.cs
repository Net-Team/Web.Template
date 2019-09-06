using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Core.Xls;
using System.Linq;

namespace Web.Test.Core
{
    public class XlsDocTest
    {
        [Fact]
        public void Test()
        {
            var items = new XlsDoc<Model>("1.xlsx")[0].ToArray();
        }

        class Model
        {
            public int age { get; set; }
        }
    }
}
