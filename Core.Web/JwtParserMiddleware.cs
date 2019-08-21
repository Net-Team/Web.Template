using Core.Jwt;
using Core.Web.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Web
{
    /// <summary>
    /// jwt解析器中间件
    /// </summary>
    class JwtParserMiddleware : Middleware
    {
        /// <summary>
        /// 选项
        /// </summary>
        private readonly JwtOptions options;

        /// <summary>
        /// 中间件基类
        /// </summary> 
        public JwtParserMiddleware(RequestDelegate next, IOptions<JwtOptions> options)
            : base(next)
        {
            this.options = options.Value;
        }

        /// <summary>
        /// 执行中间件
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override Task InvokeAsync(HttpContext httpContext)
        {
            var jwt = this.ReadJwtSecurityToken(httpContext);
            if (jwt != null)
            {
                var json = jwt.Payload.SerializeToJson();
                var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                identity.AddClaims(jwt.Claims);
                httpContext.User = new JwtClaimsPrincipal(identity, this.options.RoleClaimType, json);
            }

            return this.Next(httpContext);
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

            if (string.IsNullOrEmpty(this.options.JwtCookieName) == false)
            {
                if (httpContext.Request.Cookies.TryGetValue(this.options.JwtCookieName, out string token) == true)
                {
                    return new JwtSecurityToken(token);
                }
            }

            if (string.IsNullOrEmpty(this.options.JwtQueryName) == false)
            {
                if (httpContext.Request.Query.TryGetValue(this.options.JwtQueryName, out var queryToken) == true)
                {
                    var token = queryToken[0];
                    return new JwtSecurityToken(token);
                }
            }

            return null;
        }
    }
}
