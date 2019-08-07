using Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Web.Core.Startups
{
    /// <summary>
    /// 依赖注入注册
    /// </summary>
    public static class DependencyExtensions
    {
        /// <summary>
        /// 注册程序集下实现依赖注入接口的类型
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddDependencyServices(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(item => item.IsClass && item.IsAbstract == false).ToArray();
            var singletons = types.Where(item => item.IsInheritFrom<ISingletonDependency>());
            var transients = types.Where(item => item.IsInheritFrom<ITransientDependency>());

            foreach (var item in singletons)
            {
                services.AddSingleton(item);
            }

            foreach (var item in transients)
            {
                services.AddTransient(item);
            }
            return services;
        }
    }
}
