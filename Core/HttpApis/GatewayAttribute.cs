using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Core.HttpApis
{
    /// <summary>
    /// 标记http接口配置网关的域名
    /// </summary>  
    public class GatewayAttribute : HttpHostBaseAttribute
    {
        /// <summary>
        /// 请求路径
        /// </summary>
        public HttpPath Path { get; }

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
            this.Path = HttpPath.Create(path);
        }

        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var host = context.HttpContext.ServiceProvider.GetService<IOptionsMonitor<GatewayOptions>>().CurrentValue.HttpHost;
            context.HttpContext.RequestMessage.RequestUri = this.Path.MakeUri(host);
            return Task.CompletedTask;
        }
    }
}
