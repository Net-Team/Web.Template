using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Core.Configs;

namespace Web.Core.HostServices
{
    /// <summary>
    /// Consul注册服务
    /// </summary>
    public class ConsulRegistryService : IHostedService
    {
        private readonly ConsulInfo consul;
        private readonly ServiceInfo service;
        private readonly ILogger<ConsulRegistryService> logger;

        /// <summary>
        /// Consul注册服务
        /// </summary>
        /// <param name="service"></param>
        /// <param name="consul"></param>
        /// <param name="logger"></param>
        public ConsulRegistryService(IOptions<ServiceInfo> service, IOptions<ConsulInfo> consul, ILogger<ConsulRegistryService> logger)
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
            if (this.consul.Enable == false)
            {
                return;
            }

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5d),
                Interval = TimeSpan.FromSeconds(10d),
                HTTP = new Uri(service.Uri, service.HealthRoute).ToString(),
                Timeout = TimeSpan.FromSeconds(5d)
            };

            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = service.GetServiceId(),
                Name = service.Name,
                Address = service.Uri.Host,
                Port = service.Uri.Port,
                Tags = new[] { consul.Route }
            };

            try
            {
                using var consulClient = new ConsulClient(x => x.Address = consul.Uri);
                await consulClient.Agent.ServiceRegister(registration);
                this.logger.LogInformation($"服务注册{consul.Uri}成功");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"服务注册{consul.Uri}异常");
            }
        }

        /// <summary>
        /// 停止时
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.consul.Enable == false)
            {
                return;
            }

            try
            {
                var id = this.service.GetServiceId();
                using var consulClient = new ConsulClient(x => x.Address = consul.Uri);
                await consulClient.Agent.ServiceDeregister(id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"服务解除{consul.Uri}异常");
            }
        }
    }
}