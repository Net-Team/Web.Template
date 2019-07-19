namespace Web.Core.ServiceRegistration
{
    /// <summary>
    /// Consul信息
    /// </summary>
    public class ConsulInfo
    {
        /// <summary>
        /// Consulip
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Consul端口
        /// </summary>
        public int Port { get; set; }
    }
}
