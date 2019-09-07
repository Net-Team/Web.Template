using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Web
{
    /// <summary>
    /// Api版本控制扩展
    /// </summary>
    public static class ApiVersioningExtensions
    {
        /// <summary>
        /// 添加以namespace来查找Api版本信息 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="apiVersionHeaderName">api版本请求头名称</param>
        /// <returns></returns>
        public static IServiceCollection AddNamespaceApiVersioning(this IServiceCollection services, string apiVersionHeaderName = "x-api-version")
        {
            return services.PostConfigure<ApiVersioningOptions>(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.Conventions.Add(new VersionByNamespaceConvention());
                o.ApiVersionReader = new HeaderApiVersionReader(apiVersionHeaderName);
            });
        }
    }
}
