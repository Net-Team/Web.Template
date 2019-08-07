using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Web.Core.Middlewares;
using Web.Core.Options;

namespace Web.Core.Startups
{
    /// <summary>
    /// jwt扩展
    /// </summary>
    public static class JwtParserExtensions
    {
        /// <summary>
        /// 添加jwt解析器
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddJwtParser(this IServiceCollection services)
        {
            return services.AddJwtParser(c => { });
        }

        /// <summary>
        /// 添加jwt解析器
        /// </summary>
        /// <param name="services"></param> 
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtParser(this IServiceCollection services, Action<JwtOptions> config)
        {
            return services.Configure(config);
        }

        /// <summary>
        /// 添加jwt解析器
        /// </summary>
        /// <param name="app"></param> 
        /// <returns></returns>
        public static IApplicationBuilder UseJwtParser(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtParserMiddleware>();
        }
    }
}
