using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Core.Jwt
{
    /// <summary>
    /// 提供RSA-SHA jwt的生成
    /// </summary>
    public class RSJwt
    {
        /// <summary>
        /// 证书
        /// </summary>
        private readonly X509Certificate2 certificate;

        /// <summary>
        /// RSA-SHA jwt
        /// </summary>
        /// <param name="certificate">证书 1024位或以上</param>
        public RSJwt(X509Certificate2 certificate)
        {
            this.certificate = certificate;
        }

        /// <summary>
        /// 创建jwt
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="claims"></param>
        /// <param name="expires"></param>
        /// <param name="alg"></param>
        /// <returns></returns>
        public string CreateToken(string issuer, string audience, IEnumerable<Claim> claims, DateTime? expires, string alg = SecurityAlgorithms.RsaSha256)
        {
            var signingCredentials = new SigningCredentials(new X509SecurityKey(this.certificate), alg);
            var jwtHandler = new JwtSecurityTokenHandler();

            var jwt = jwtHandler.CreateJwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: expires,
                notBefore: DateTime.Now,
                signingCredentials: signingCredentials,
                subject: new ClaimsIdentity(claims)
            );
            return jwtHandler.WriteToken(jwt);
        }
    }
}
