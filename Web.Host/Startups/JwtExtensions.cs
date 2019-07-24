using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Web.Host.Startups.Jwt;

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
        /// <param name="services"></param> 
        public static void AddJwtParser(this IServiceCollection services)
        {
            services.AddJwtParser(c => { });
        }

        /// <summary>
        /// 添加jwt解析器
        /// </summary>
        /// <param name="services"></param> 
        /// <param name="config"></param>
        public static void AddJwtParser(this IServiceCollection services, Action<JwtOptions> config)
        { 
            services.Configure(config);
        }

        /// <summary>
        /// 添加jwt解析器
        /// </summary>
        /// <param name="app"></param> 
        /// <returns></returns>
        public static IApplicationBuilder UseJwtParser(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JWTMiddleware>();
        }
    }
}
