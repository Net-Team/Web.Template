using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace Core.HttpApis
{
    /// <summary>
    /// 标记http接口配置网关的域名
    /// </summary>  
    public class GatewayAttribute : ApiActionAttribute
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

        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            var options = context.GetService<IOptions<GatewayOptions>>().Value;
            var host = options.HttpHost;
            if (this.Path.IsNullOrEmpty() == false)
            {
                host = new Uri(host, this.Path);
            }

            context.RequestMessage.RequestUri = host;
            return Task.CompletedTask;
        }
    }
}
