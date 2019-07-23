using System;

namespace Web.Core.Options
{
    /// <summary>
    /// 表示服务信息
    /// </summary>
    public class ServiceOptions
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
        /// 健康检查的路由
        /// </summary>
        public string HealthRoute { get; set; }         
    }
}
