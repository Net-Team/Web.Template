using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Core;

namespace Web.Host.Startups.Jwt
{
    /// <summary>
    /// jwt解析器中间件
    /// </summary>
    public class JWTMiddleware : Middleware
    {
        /// <summary>
        /// 选项
        /// </summary>
        private readonly JwtOptions options;

        /// <summary>
        /// 中间件基类
        /// </summary> 
        public JWTMiddleware(RequestDelegate next, JwtOptions options)
            : base(next)
        {
            this.options = options;
        }

        /// <summary>
        /// 执行中间件
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override Task Invoke(HttpContext httpContext)
        {
            var jwt = this.ReadJwtSecurityToken(httpContext);
            if (jwt != null)
            {
                var json = jwt.Payload.SerializeToJson();
                var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                identity.AddClaims(jwt.Claims);
                httpContext.User = new JwtClaimsPrincipal(identity, json);
            }

            return this.Next.Invoke(httpContext);
        }


        /// <summary>
        /// 读取token
        /// </summary>
        /// <param name="httpContext"></param> 
        /// <returns></returns>
        private JwtSecurityToken ReadJwtSecurityToken(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var bearerToken))
            {
                var token = bearerToken.ToString().Split(' ').LastOrDefault();
                return new JwtSecurityToken(token);
            }

            if (string.IsNullOrEmpty(this.options.CookieName) == false)
            {
                if (httpContext.Request.Cookies.TryGetValue(this.options.CookieName, out string token) == true)
                {
                    return new JwtSecurityToken(token);
                }
            }

            if (string.IsNullOrEmpty(this.options.QueryName) == false)
            {
                if (httpContext.Request.Query.TryGetValue(this.options.QueryName, out var queryToken) == true)
                {
                    var token = queryToken[0];
                    return new JwtSecurityToken(token);
                }
            }

            return null;
        }
    }
}
