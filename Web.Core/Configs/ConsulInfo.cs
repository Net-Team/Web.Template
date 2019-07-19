using System;

namespace Web.Core.Configs
{
    /// <summary>
    /// Consul信息
    /// </summary>
    public class ConsulInfo
    {
        /// <summary>
        /// Consul的Uri
        /// </summary>
        public Uri Uri { get; set; }

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
