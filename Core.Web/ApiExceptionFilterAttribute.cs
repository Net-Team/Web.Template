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
    public class ApiExceptionFilterAttribute : Attribute, IExceptionFilter, IOrderedFilter
    {
        /// <summary>
        /// 获取或设置是否启用
        /// 默认为true
        /// </summary>
        public bool Enbale { get; set; } = true;

        /// <summary>
        /// 获取或设置排序顺序
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// 异常时
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnException(ExceptionContext context)
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
            if (context.Exception is ArgumentException)
            {
                apiResult.Code = Code.ParameterError;
            }

            var inner = context.Exception;
            var exceptionTypeName = inner.GetType().Name;
            while (inner != null)
            {
                apiResult.Message = $"{exceptionTypeName}：{inner.Message}";
                inner = inner.InnerException;
            }

            context.HttpContext
                .RequestServices
                .GetService<ILogger<ApiExceptionFilterAttribute>>()
                .LogError(context.Exception, context.Exception.Message);

            context.ExceptionHandled = true;
            context.Result = new ObjectResult(apiResult);
        }
    }
}
