using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace Core
{
    /// <summary>
    /// 依赖注入注册
    /// </summary>
    public static class ServiceCollectionDependencyExtensions
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
            foreach (var impType in types)
            {
                var register = impType.GetCustomAttribute<RegisterAttribute>();
                if (register == null)
                {
                    register = impType
                        .GetInterfaces()
                        .Select(item => item.GetCustomAttribute<RegisterAttribute>())
                        .FirstOrDefault(item => item != null);
                }

                if (register != null)
                {
                    var descriptor = ServiceDescriptor.Describe(register.ServiceType ?? impType, impType, register.Lifetime);
                    if (services.Any(item => item.ServiceType == descriptor.ServiceType && item.ImplementationType == descriptor.ImplementationType) == false)
                    {
                        services.Add(descriptor);
                    }
                }
            }
            return services;
        }
    }
}