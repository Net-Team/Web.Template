using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 表示托管服务抽象类
    /// 该对象只适合在应用启动时做等待执行的短时间任务
    /// 任务并不会在后台运行
    /// </summary>
    public abstract class HostedService : Disposable, IHostedService
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider services;

        /// <summary>
        /// 托管服务抽象类
        /// 该对象只适合在应用启动时做等待执行的短时间任务
        /// 任务并不会在后台运行
        /// </summary>
        /// <param name="services">服务提供者</param>
        public HostedService(IServiceProvider services)
        {
            this.services = services;
        }

        /// <summary>
        /// 服务启动入口
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await this.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                this.services
                    .GetService<ILoggerFactory>()?
                    .CreateLogger(this.GetType().FullName)?
                    .LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task StartAsync(CancellationToken cancellationToken);


        /// <summary>
        /// 服务停止入口
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return this.StopAsync(cancellationToken);
        }

        /// <summary>
        /// 服务停止时
        /// 如果应用意外关闭（例如，应用的进程失败），则可能不会调用 StopAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
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
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
