using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        /// <param name="assembly"></param> 
        /// <param name="configuration"></param>
        public static IServiceCollection AddConfigureOptions(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            foreach (var optionsType in assembly.GetTypes())
            {
                if (optionsType.IsInheritFrom<IConfigureOptions>() == false)
                {
                    continue;
                }

                var builderType = typeof(BindOptionsBuilder<>).MakeGenericType(optionsType);
                var builder = (IBindOptionsBuilder)Activator.CreateInstance(builderType);
                builder.Bind(services, configuration.GetSection(optionsType.Name));
            }
            return services;
        }

        private interface IBindOptionsBuilder
        {
            void Bind(IServiceCollection services, IConfiguration configuration);
        }

        private class BindOptionsBuilder<TOptions> : IBindOptionsBuilder where TOptions : class
        {
            public void Bind(IServiceCollection services, IConfiguration configuration)
            {
                services.AddOptions<TOptions>().Bind(configuration);
            }
        }
    }
}
