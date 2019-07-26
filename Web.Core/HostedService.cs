using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Core
{
    /// <summary>
    /// 表示托管服务抽象类
    /// </summary>
    public abstract class HostedService : Disposable, IHostedService
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider services;

        /// <summary>
        /// 托管服务抽象类
        /// </summary>
        /// <param name="services">服务提供者</param>
        public HostedService(IServiceProvider services)
        {
            this.services = services;
        }

        /// <summary>
        /// 创建具有指定生命周期范围的服务提供者
        /// </summary>
        /// <returns></returns>
        protected IScopeServiceProvider CreateScopeServiceProvider()
        {
            var serviceScope = this.services.CreateScope();
            return new ScopeServiceProvider(serviceScope);
        }

        /// <summary>
        /// 服务启动时
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 服务停止时
        /// 如果应用意外关闭（例如，应用的进程失败），则可能不会调用 StopAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task StopAsync(CancellationToken cancellationToken);


        /// <summary>
        /// 具有指定生命周期范围的服务提供者
        /// </summary>
        private class ScopeServiceProvider : Disposable, IScopeServiceProvider
        {
            private readonly IServiceScope serviceScope;

            public ScopeServiceProvider(IServiceScope serviceScope)
            {
                this.serviceScope = serviceScope;
            }

            public object GetService(Type serviceType)
            {
                return this.serviceScope.ServiceProvider.GetService(serviceType);
            }

            protected override void Dispose(bool disposing)
            {
                this.serviceScope.Dispose();
            }
        }
    }
}
