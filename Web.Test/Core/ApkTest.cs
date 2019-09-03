using Core.Apk;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Web.Test.Core
{
    public  class ApkTest
    {
        [Fact]
        public void Test()
        {
            ApkReader.ReadMedia("001.apk");
        }
    }
}
