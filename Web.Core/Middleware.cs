using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Web.Core
{
    /// <summary>
    /// 中间件基类
    /// 所有参数通过DI注册
    /// </summary>
    public abstract class Middleware
    {
        /// <summary>
        /// 获取下一个中间件
        /// </summary>
        protected RequestDelegate Next { get; }

        /// <summary>
        /// 中间件基类
        /// </summary>
        /// <param name="next"></param>
        public Middleware(RequestDelegate next)
        {
            this.Next = next;
        }

        /// <summary>
        /// 执行中间件
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public abstract Task InvokeAsync(HttpContext httpContext);
    }
}
