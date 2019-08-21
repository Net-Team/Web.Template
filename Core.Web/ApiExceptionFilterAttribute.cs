using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Core.Web
{
    /// <summary>
    /// 表示Api异常处理过滤器
    /// 将异常转换为IApiResult返回值
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        /// <summary>
        /// 获取或设置是否启用
        /// 默认为true
        /// </summary>
        public bool Enbale { get; set; } = true;

        /// <summary>
        /// 异常时
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            if (this.Enbale == false)
            {
                return;
            }

            var apiResultType = context.GetApiDescription()
                .SupportedResponseTypes
                .Select(item => item.Type)
                .Where(item => item.IsInheritFrom<IApiResult>())
                .FirstOrDefault();

            if (apiResultType == null)
            {
                apiResultType = typeof(ApiResult<object>);
            }

            var apiResult = Activator.CreateInstance(apiResultType) as IApiResult;
            apiResult.Code = Code.ServiceError;
            apiResult.Message = context.Exception.Message;

            if (context.Exception is ArgumentException)
            {
                apiResult.Code = Code.ParameterError;
            }

            context.HttpContext
                .RequestServices
                .GetService<ILogger<ApiExceptionFilterAttribute>>()
                .LogError(context.Exception, context.Exception.Message);

            context.Result = new ObjectResult(apiResult);
        }
    }
}
