using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Core.ServiceRegistration
{
    /// <summary>
    /// Consul注册服务
    /// </summary>
    public class ConsulHostService : IHostedService
    {
        private readonly ConsulInfo consul;
        private readonly ServiceInfo service;
        private readonly ILogger<ConsulHostService> logger;

        /// <summary>
        /// Consul注册服务
        /// </summary>
        /// <param name="service"></param>
        /// <param name="consul"></param>
        /// <param name="logger"></param>
        public ConsulHostService(IOptions<ServiceInfo> service, IOptions<ConsulInfo> consul, ILogger<ConsulHostService> logger)
        {
            this.service = service.Value;
            this.consul = consul.Value;
            this.logger = logger;
        }

        /// <summary>
        /// 启动时
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (this.service.ServiceRouteEnable == false)
            {
                return;
            }

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5d),
                Interval = TimeSpan.FromSeconds(10d),
                HTTP = $"http://{service.IPAddress}:{service.Port}/health",
                Timeout = TimeSpan.FromSeconds(5d)
            };

            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = $"{service.Name}_{service.Port}",
                Name = service.Name,
                Address = service.IPAddress,
                Port = service.Port,
                Tags = new[] { $"urlprefix-/{service.Name}" } // 添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
            };

            try
            {
                using var consulClient = new ConsulClient(x => x.Address = new Uri($"http://{consul.IPAddress}:{consul.Port}"));
                await consulClient.Agent.ServiceRegister(registration);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "服务注册异常");
            }
        }

        /// <summary>
        /// 停止时
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.service.ServiceRouteEnable == false)
            {
                return;
            }           

            try
            {
                var id = $"{service.Name}_{service.Port}";
                using var consulClient = new ConsulClient(x => x.Address = new Uri($"http://{consul.IPAddress}:{consul.Port}"));
                await consulClient.Agent.ServiceDeregister(id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "服务解除注册异常");
            }
        }
    }
}