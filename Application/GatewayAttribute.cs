using System;

namespace Application
{
    /// <summary>
    /// 标记http接口配置网关的域名
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class GatewayAttribute : Attribute
    {
        /// <summary>
        /// 请求路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// http接口配置网关的域名
        /// </summary>
        public GatewayAttribute()          
        {
        }

        /// <summary>
        /// http接口配置网关的域名
        /// </summary>
        /// <param name="path">路径</param>
        public GatewayAttribute(string path)
        {
            this.Path = path;
        }
    }
}
