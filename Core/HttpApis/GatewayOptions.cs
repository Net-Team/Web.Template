using System;

namespace Core.HttpApis
{
    /// <summary>
    /// 网关代理选项
    /// </summary>
    public class GatewayOptions : IConfigureOptions
    {
        /// <summary>
        /// 网关代理地址
        /// </summary>
        public Uri HttpHost { get; set; }
    }
}
