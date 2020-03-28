using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 表示后台任务服务
    /// 该类型不能直接使用
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class _HostedBackgroundService : BackgroundService, IDisposable
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider services;

        /// <summary>
        /// 后台任务服务
        /// </summary>
        /// <param name="services">服务提供者</param>
        public _HostedBackgroundService(IServiceProvider services)
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
        /// 启动任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public sealed override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public sealed override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// 获取对象是否已释放
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 关闭和释放所有相关资源
        /// </summary>
        void IDisposable.Dispose()
        {
            if (this.IsDisposed == false)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            this.IsDisposed = true;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~_HostedBackgroundService()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            base.Dispose();
        }
    }


    /// <summary>
    /// 表示后台任务服务
    /// </summary>
    public abstract class HostedBackgroundService : _HostedBackgroundService, IHostedService
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider services;

        /// <summary>
        /// 后台任务服务
        /// </summary>
        /// <param name="services">服务提供者</param>
        public HostedBackgroundService(IServiceProvider services)
            : base(services)
        {
            this.services = services;
        }

        /// <summary>
        /// 任务启动入口
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            await this.StartAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }


        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        new protected virtual Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务停止入口
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            await this.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// 停止任务
        /// 如果应用意外关闭（例如，应用的进程失败），则可能不会调用 StopAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        new protected virtual Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await this.ExecuteBackgroundAsync(stoppingToken);
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
        /// 后台执行任务
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected abstract Task ExecuteBackgroundAsync(CancellationToken stoppingToken);
    }
}
