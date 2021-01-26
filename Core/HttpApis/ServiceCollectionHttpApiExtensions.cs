using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        public static IServiceCollection AddHttpApis(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            foreach (var httpApiType in assembly.GetTypes())
            {
                if (httpApiType.IsInterface && httpApiType.IsInheritFrom<IHttpApi>())
                {
                    services
                        .AddHttpApi(httpApiType)
                        .ConfigureHttpApi(configuration.GetSection(httpApiType.Name))
                        .ConfigurePrimaryHttpMessageHandler(() =>
                        {
                            return new HttpClientHandler { ServerCertificateCustomValidationCallback = (a, b, c, d) => true };
                        });
                }
            }

            return services;
        }
    }
}

