using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        /// <param name="assemblies"></param> 
        /// <param name="configuration"></param>
        public static IServiceCollection AddConfigureOptions(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var optionsType in assembly.GetTypes())
                {
                    if (optionsType.IsInheritFrom<IConfigureOptions>() == true)
                    {
                        var configSection = configuration.GetSection(optionsType.Name);
                        services.Bind(optionsType, configSection);
                    }
                }
            }
            return services;
        }

        /// <summary>
        /// 绑定配置到选项
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsType"></param>
        /// <param name="configuration"></param>
        private static void Bind(this IServiceCollection services, Type optionsType, IConfiguration configuration)
        {
            var builderType = typeof(BindingOptionsBuilder<>).MakeGenericType(optionsType);
            var builder = (IBindingOptionsBuilder)Activator.CreateInstance(builderType, services);
            builder.Bind(configuration);
        }

        private interface IBindingOptionsBuilder
        {
            void Bind(IConfiguration configureOptions);
        }

        private class BindingOptionsBuilder<T> : IBindingOptionsBuilder where T : class
        {
            private readonly OptionsBuilder<T> builder;

            public BindingOptionsBuilder(IServiceCollection services)
            {
                this.builder = new OptionsBuilder<T>(services, Options.DefaultName);
            }

            public void Bind(IConfiguration configureOptions)
            {
                this.builder.Bind(configureOptions);
            }
        }
    }
}
