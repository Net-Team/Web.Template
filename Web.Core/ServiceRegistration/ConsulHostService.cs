using Consul;
using Microsoft.Extensions.Hosting;
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

        /// <summary>
        /// Consul注册服务
        /// </summary>
        /// <param name="serviceInfo"></param>
        /// <param name="consulInfo"></param>
        public ConsulHostService(IOptions<ServiceInfo> serviceInfo, IOptions<ConsulInfo> consulInfo)
        {
            this.service = serviceInfo.Value;
            this.consul = consulInfo.Value;
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

            var consulClient = new ConsulClient(x => x.Address = new Uri($"http://{consul.IPAddress}:{consul.Port}"));
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5d),// 服务启动多久后注册
                Interval = TimeSpan.FromSeconds(10d),// 健康检查时间间隔，或者称为心跳间隔
                HTTP = $"http://{service.IPAddress}:{service.Port}/health",// 健康检查地址
                Timeout = TimeSpan.FromSeconds(5)
            };

            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = $"{service.Name}_{service.Port}",
                Name = service.Name,
                Address = service.IPAddress,
                Port = service.Port,
                Tags = new[] { $"urlprefix-/{service.Name}" } //添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
            };

            await consulClient.Agent.ServiceRegister(registration);
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

            var id = $"{service.Name}_{service.Port}";
            var consulClient = new ConsulClient(x => x.Address = new Uri($"http://{consul.IPAddress}:{consul.Port}"));
            await consulClient.Agent.ServiceDeregister(id);
        }
    }
}