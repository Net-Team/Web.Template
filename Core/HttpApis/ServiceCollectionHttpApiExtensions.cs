using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using WebApiClient;

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
        public static IServiceCollection AddHttpApis(this IServiceCollection services, [NotNull] Assembly assembly)
        {
            var httpApis = assembly.GetTypes().Where(item => item.IsInheritFrom<IHttpApi>());
            var method = typeof(ServiceCollectionHttpApiExtensions).GetMethod(nameof(AddHttpApi), BindingFlags.Static | BindingFlags.NonPublic);

            foreach (var item in httpApis)
            {
                if (item.IsDefined(typeof(ApiManualRegisterAttribute)) == false)
                {
                    method.MakeGenericMethod(item).Invoke(null, new object[] { services });
                }
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

            services.AddHttpApiTypedClient<TInterface>((c, p) =>
            {
                p.GetService<IConfiguration>().GetSection(key).Bind(c);
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

