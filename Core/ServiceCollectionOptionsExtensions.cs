using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Core
{
    /// <summary>
    /// IConfigureOptions自动注册扩展
    /// </summary>
    public static class ServiceCollectionOptionsExtensions
    {
        /// <summary>
        /// 注册程序集下实现IConfigureOptions接口的Options
        /// 并关联到configuration的子项
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param> 
        /// <param name="configuration"></param>
        public static IServiceCollection AddConfigureOptions(this IServiceCollection services, [NotNull] IConfiguration configuration, [NotNull] params Assembly[] assemblies)
        {
            const string name = nameof(OptionsConfigurationServiceCollectionExtensions.Configure);
            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod(name, 1, new[] { typeof(IServiceCollection), typeof(IConfiguration) });

            foreach (var assembly in assemblies)
            {
                var optionsTypes = assembly.GetTypes().Where(item => item.IsInheritFrom<IConfigureOptions>());
                foreach (var type in optionsTypes)
                {
                    var configSection = configuration.GetSection(type.Name);
                    method.MakeGenericMethod(type).Invoke(null, new object[] { services, configSection });
                }
            }

            return services;
        }
    }
}
