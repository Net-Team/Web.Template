using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using WebApiClientCore;
using WebApiClientCore.Serialization.JsonConverters;

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
        public static IServiceCollection AddHttpApis(this IServiceCollection services, Assembly assembly)
        {
            var httpApis = assembly.GetTypes().Where(item => item.IsInterface && item.IsInheritFrom<IHttpApi>());
            foreach (var httpApi in httpApis)
            {
                if (httpApi.IsDefined(typeof(ApiManualRegisterAttribute)) == false)
                {
                    var key = $"HttpApi:{httpApi.Name}";
                    services.AddHttpApi(httpApi, (o, s) =>
                    {
                        o.JsonDeserializeOptions.Converters.Add(JsonLocalDateTimeConverter.Default);
                        s.GetRequiredService<IConfiguration>().GetSection(key).Bind(o);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (a, b, c, d) => true };
                        return handler;
                    });
                }
            }
            return services;
        }
    }
}

