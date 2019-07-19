using System;

namespace Web.Core.Configs
{
    /// <summary>
    /// 表示服务信息
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 服务的Uri
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// 监听的ip与端口
        /// </summary>
        public string Listen { get; set; }

        /// <summary>
        /// 健康检查的路由
        /// </summary>
        public string HealthRoute { get; set; }

        /// <summary>
        /// 获取唯一标识
        /// </summary>
        /// <returns></returns>
        public string GetServiceId()
        {
            return $"{this.Name}_{this.Uri.Port}";
        }
    }
}
