using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Core.FilterAttributes
{
    /// <summary>
    /// 表示Api异常全局处理过滤器
    /// </summary>
    public class ApiGlobalExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// 异常时
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
        }
    }
}
