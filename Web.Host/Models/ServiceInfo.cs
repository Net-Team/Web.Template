using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Host.Models
{
    /// <summary>
    /// 表示服务信息
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

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
            return $"/{this.ServiceName}/{uri.TrimStart('/')}";
        }
    }
}
