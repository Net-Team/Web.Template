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
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        private static void AddHttpApi<THttpApi>(IServiceCollection services) where THttpApi : class, IHttpApi
        {
            var key = $"HttpApi:{typeof(THttpApi).Name}";
            services.AddHttpApi<THttpApi>((o, s) =>
            {
                s.GetService<IConfiguration>().GetSection(key).Bind(o);
            });
        }
    }
}

