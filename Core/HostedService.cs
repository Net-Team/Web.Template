using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 表示托管服务抽象类
    /// </summary>
    public abstract class HostedService : Disposable, IHostedService
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        private Task executingTask;

        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider services;

        /// <summary>
        /// 停止取消
        /// </summary>
        private readonly CancellationTokenSource stoppingCts = new CancellationTokenSource();



        /// <summary>
        /// 托管服务抽象类
        /// </summary>
        /// <param name="services">服务提供者</param>
        public HostedService(IServiceProvider services)
        {
            this.services = services;
        }


        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            this.executingTask = this.StartAsync(this.stoppingCts.Token);
            return this.executingTask.IsCompleted ? this.executingTask : Task.CompletedTask;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="stoppingToken">停止令牌</param>
        /// <returns></returns>
        protected abstract Task StartAsync(CancellationToken stoppingToken);


        /// <summary>
        /// 服务停止时
        /// 如果应用意外关闭（例如，应用的进程失败），则可能不会调用 StopAsync
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
        protected virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.executingTask == null)
            {
                return;
            }

            try
            {
                this.stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(this.executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
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
            this.stoppingCts.Cancel();
        }


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
