using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace Application.TcIots
{
    /// <summary>
    /// IOT平台授权特性
    /// </summary>
    class IotAuthAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 排序
        /// </summary>
        public override int OrderIndex => int.MinValue;

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            var options = context.GetService<IOptions<IotOptions>>().Value;
            if (context.RequestMessage.RequestUri == null)
            {
                context.RequestMessage.RequestUri = options.DefaultHttpHost;
            }
            context.RequestMessage.Headers.Authorization = options.ToAuthorization();
            return Task.CompletedTask;
        }
    }
}
