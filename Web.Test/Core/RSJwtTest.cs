using Core.Jwt;
using System;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Web.Test.Core
{
    public class RSJwtTest
    {
        [Fact]
        public void CreateToken()
        {
            var pfx = Path.Combine(AppContext.BaseDirectory, "core/jwt.pfx");
            var cert = new X509Certificate2(pfx);
            var jwt = new RSJwt(cert);
            var token = jwt.CreateToken("www.taichuan.com", "www.taichuan.com", new[] { new Claim("id", "id001") }, DateTime.Now.AddDays(1));
            Assert.True(token.Length > 0);
        }
    }
}
