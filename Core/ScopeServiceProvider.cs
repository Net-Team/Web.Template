using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core
{
    /// <summary>
    /// 表示具有指定生命周期范围的服务提供者
    /// </summary>
    class ScopeServiceProvider : Disposable, IScopeServiceProvider
    {
        /// <summary>
        /// 服务作用域
        /// </summary>
        private readonly IServiceScope serviceScope;

        /// <summary>
        /// 具有指定生命周期范围的服务提供者
        /// </summary>
        /// <param name="serviceScope"></param>
        public ScopeServiceProvider(IServiceScope serviceScope)
        {
            this.serviceScope = serviceScope;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return this.serviceScope.ServiceProvider.GetService(serviceType);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.serviceScope.Dispose();
        }
    }
}
