using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 表示后台任务服务
    /// </summary>
    public abstract class HostedBackgroundService : Disposable, IHostedService
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider services;

        /// <summary>
        /// 后台服务包装器
        /// </summary>
        private readonly BackgroundWrapperService backgroundService;

        /// <summary>
        /// 后台任务服务
        /// </summary>
        /// <param name="services">服务提供者</param>
        public HostedBackgroundService(IServiceProvider services)
        {
            this.services = services;
            this.backgroundService = new BackgroundWrapperService(this);
        }

        /// <summary>
        /// 任务启动入口
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            return this.backgroundService.StartAsync(cancellationToken);
        }

        /// <summary>
        /// 任务停止入口      
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return this.backgroundService.StopAsync(cancellationToken);
        }


        /// <summary>
        /// 启动时
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止时
        /// 如果应用意外关闭（例如，应用的进程失败），则可能不会调用 StopAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 后台执行的任务
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);


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
        /// 处理异常
        /// 默认是输出异常日志
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected virtual Task HandleExceptionAsync(Exception ex)
        {
            this.services
                .GetService<ILoggerFactory>()?
                .CreateLogger(this.GetType().FullName)?
                .LogError(ex, ex.Message);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.backgroundService.Dispose();
        }

        /// <summary>
        /// 后台任务包装服务
        /// </summary>
        private class BackgroundWrapperService : BackgroundService
        {
            private readonly HostedBackgroundService backgroundService;

            /// <summary>
            /// 后台任务包装服务
            /// </summary>
            /// <param name="backgroundService"></param>
            public BackgroundWrapperService(HostedBackgroundService backgroundService)
            {
                this.backgroundService = backgroundService;
            }

            /// <summary>
            /// 启动时
            /// </summary>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public override async Task StartAsync(CancellationToken cancellationToken)
            {
                try
                {
                    await this.backgroundService.StartAsync(cancellationToken);
                    await base.StartAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    await this.backgroundService.HandleExceptionAsync(ex);
                }
            }

            /// <summary>
            /// 停止时
            /// </summary>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public override async Task StopAsync(CancellationToken cancellationToken)
            {
                try
                {
                    await this.backgroundService.StopAsync(cancellationToken);
                    await base.StopAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    await this.backgroundService.HandleExceptionAsync(ex);
                }
            }

            /// <summary>
            /// 执行后台任务
            /// </summary>
            /// <param name="stoppingToken"></param>
            /// <returns></returns>
            protected async override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                try
                {
                    await this.backgroundService.ExecuteAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    await this.backgroundService.HandleExceptionAsync(ex);
                }
            }
        }
    }
}
