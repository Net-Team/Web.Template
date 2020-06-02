using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

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
            : this(null)
        {
        }

        /// <summary>
        /// http接口配置网关的域名
        /// </summary>
        /// <param name="path">路径</param>
        public GatewayAttribute(string path)
        {
            this.Path = path;
            this.OrderIndex = int.MinValue;
        }

        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var options = context.HttpContext.Services.GetService<IOptions<GatewayOptions>>().Value;
            var host = options.HttpHost;
            if (this.Path.IsNullOrEmpty() == false)
            {
                host = new Uri(host, this.Path);
            }

            context.HttpContext.RequestMessage.RequestUri = host;
            return Task.CompletedTask;
        }
    }
}
