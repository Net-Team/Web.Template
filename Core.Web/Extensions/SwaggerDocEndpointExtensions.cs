using Core.Web.Swaggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Core.Web
{
    /// <summary>
    /// json文档路径和ui路径扩展
    /// </summary>
    public static class SwaggerDocEndpointExtensions
    {
        /// <summary>
        /// 添加json文档路径和ui路径
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddSwaggerDocUIAndEndpoints(this IServiceCollection services)
        {
            return services
                .AddTransient<IConfigureOptions<SwaggerOptions>, SwaggerOptionsConfigureOptions>()
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenOptionsConfigureOptions>()
                .AddTransient<IConfigureOptions<SwaggerUIOptions>, SwaggerUIOptionsConfigureOptions>();
        }
    }
}
