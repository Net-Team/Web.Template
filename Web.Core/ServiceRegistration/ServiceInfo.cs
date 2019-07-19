namespace Web.Core.ServiceRegistration
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
        /// 局域网ip
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 是否使用服务路由
        /// </summary>
        public bool ServiceRouteEnable { get; set; }

        /// <summary>
        /// 补齐服务路由
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string ServiceRoute(string uri)
        {
            if (this.ServiceRouteEnable == false || string.IsNullOrEmpty(uri))
            {
                return uri;
            }
            return $"/{this.Name}/{uri.TrimStart('/')}";
        }
    }
}
