using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using WebApiClientCore;

namespace Core.HttpApis
{
    /// <summary>
    /// HttpApi注册
    /// </summary>
    public static class ServiceCollectionHttpApiExtensions
    {
        /// <summary>
        /// 注册程序集下所有IHttpApi
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddHttpApis(this IServiceCollection services, Assembly assembly, string configuration = "HttpApi")
        {
            var httpApis = assembly.GetTypes().Where(item => item.IsInterface && item.IsInheritFrom<IHttpApi>());
            foreach (var httpApi in httpApis)
            {
                if (httpApi.IsDefined(typeof(ApiManualRegisterAttribute)) == true)
                {
                    continue;
                }

                services
                    .AddHttpApi(httpApi, (o, s) =>
                    {
                        var key = configuration == null ? httpApi.Name : $"{configuration}:{httpApi.Name}";
                        s.GetService<IConfiguration>().GetSection(key).Bind(o);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        return new HttpClientHandler { ServerCertificateCustomValidationCallback = (a, b, c, d) => true };
                    });
            }

            return services;
        }

        /// <summary>
        /// 注册程序集下所有IHttpApi
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddHttpApis(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            var httpApis = assembly.GetTypes().Where(item => item.IsInterface && item.IsInheritFrom<IHttpApi>());
            foreach (var httpApi in httpApis)
            {
                if (httpApi.IsDefined(typeof(ApiManualRegisterAttribute)) == true)
                {
                    continue;
                }

                services
                    .ConfigureHttpApi(httpApi, configuration.GetSection(httpApi.Name))
                    .AddHttpApi(httpApi)
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        return new HttpClientHandler { ServerCertificateCustomValidationCallback = (a, b, c, d) => true };
                    });
            }

            return services;
        }
    }
}

