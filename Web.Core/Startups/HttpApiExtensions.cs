using Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using Web.Core.Options;
using WebApiClient;

namespace Web.Core.Startups
{
    /// <summary>
    /// HttpApi注册
    /// </summary>
    public static class HttpApiExtensions
    {
        /// <summary>
        /// 注册程序集下所有IHttpApi
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        public static IServiceCollection AddHttpApis(this IServiceCollection services, Assembly assembly)
        {
            var httpApis = assembly.GetTypes().Where(item => item.IsInheritFrom<IHttpApi>());
            var method = typeof(HttpApiExtensions).GetMethod(nameof(AddHttpApi), BindingFlags.Static | BindingFlags.NonPublic);

            foreach (var item in httpApis)
            {
                method.MakeGenericMethod(item).Invoke(null, new object[] { services });
            }
            return services;
        }

        /// <summary>
        /// 添加HttpApi
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="services"></param>
        private static void AddHttpApi<TInterface>(IServiceCollection services) where TInterface : class, IHttpApi
        {
            var apiType = typeof(TInterface);
            var key = $"HttpApi:{apiType.Name}";
            var gateway = apiType.GetCustomAttribute<GatewayAttribute>(false);

            services.AddHttpApiTypedClient<TInterface>((c, p) =>
            {
                p.GetService<IConfiguration>().GetSection(key).Bind(c);
                if (gateway != null)
                {
                    var host = p.GetService<IOptions<ServiceOptions>>().Value.GatewayProxyUri;
                    if (gateway.Path.IsNullOrEmpty() == false)
                    {
                        host = new Uri(host, gateway.Path);
                    }
                    c.HttpHost = host;
                }
            });
        }

        /// <summary>
        /// 添加HttpApiClient的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configOptions">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        private static IHttpClientBuilder AddHttpApiTypedClient<TInterface>(this IServiceCollection services, Action<HttpApiConfig, IServiceProvider> configOptions)
            where TInterface : class, IHttpApi
        {
            if (configOptions == null)
            {
                throw new ArgumentNullException(nameof(configOptions));
            }
            return services
                .AddHttpClient(typeof(TInterface).FullName)
                .AddTypedClient((httpClient, provider) =>
                {
                    var httpApiConfig = new HttpApiConfig(httpClient)
                    {
                        ServiceProvider = provider
                    };
                    configOptions.Invoke(httpApiConfig, provider);
                    return HttpApi.Create<TInterface>(httpApiConfig);
                });
        }
    }
}

