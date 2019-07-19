namespace Web.Core.ServiceRegistration
{
    /// <summary>
    /// Consul信息
    /// </summary>
    public class ConsulInfo
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Consulip
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Consul端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 路由
        /// 比如/api
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }
    }
}
