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
        public static IServiceCollection AddHttpApis(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            var httpApiTypes = assembly.GetTypes().Where(item => item.IsInterface && item.IsInheritFrom<IHttpApi>());
            foreach (var httpApiType in httpApiTypes)
            {
                services
                    .ConfigureHttpApi(httpApiType, configuration.GetSection(httpApiType.Name))
                    .AddHttpApi(httpApiType)
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        return new HttpClientHandler { ServerCertificateCustomValidationCallback = (a, b, c, d) => true };
                    });
            }

            return services;
        }
    }
}

