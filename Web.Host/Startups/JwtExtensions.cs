using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Host.Startups
{
    /// <summary>
    /// jwt扩展
    /// </summary>
    public static class JwtExtensions
    {
        /// <summary>
        /// 添加jwt解析器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="cookieName">jwt的cookie名称</param>
        /// <param name="queryName">jwt的query名称</param>
        public static void AddJwtParser(this AuthenticationBuilder builder, string cookieName = null, string queryName = null)
        {
            builder.AddJwtBearer(c =>
            {
                c.SecurityTokenValidators.Clear();
                c.SecurityTokenValidators.Add(new TokenUnValidator());
                c.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = ReadToken(context.HttpContext, cookieName, queryName);
                        return Task.CompletedTask;
                    }
                };
            });
        }

        /// <summary>
        /// 读取token
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="cookieName"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        private static string ReadToken(HttpContext httpContext, string cookieName, string queryName)
        {
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var bearerToken))
            {
                return bearerToken.ToString().Split(' ').LastOrDefault();
            }

            if (string.IsNullOrEmpty(cookieName) == false)
            {
                if (httpContext.Request.Cookies.TryGetValue(cookieName, out string token) == true)
                {
                    return token;
                }
            }

            if (string.IsNullOrEmpty(queryName) == false)
            {
                if (httpContext.Request.Query.TryGetValue(queryName, out var queryToken) == true)
                {
                    return queryToken[0];
                }
            }
            return null;
        }

        /// <summary>
        /// 不需要验证的token验证器
        /// </summary>
        private class TokenUnValidator : ISecurityTokenValidator
        {
            /// <summary>
            /// 返回是否可以验证token
            /// </summary>
            public bool CanValidateToken => true;

            /// <summary>
            /// token的最大bytes
            /// </summary>
            public int MaximumTokenSizeInBytes { get; set; }

            /// <summary>
            /// 返回是否可以读取token
            /// </summary>
            /// <param name="securityToken"></param>
            /// <returns></returns>
            public bool CanReadToken(string securityToken)
            {
                return true;
            }

            /// <summary>
            /// 验证token
            /// 返回ClaimsPrincipal
            /// </summary>
            /// <param name="securityToken"></param>
            /// <param name="validationParameters"></param>
            /// <param name="validatedToken"></param>
            /// <returns></returns>
            public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
            {
                var jwt = new JwtSecurityToken(securityToken);
                validatedToken = jwt;

                var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                identity.AddClaims(jwt.Claims);
                return new ClaimsPrincipal(identity);
            }
        }
    }
}
