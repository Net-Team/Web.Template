using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Core.Web
{
    /// <summary>
    /// 表示Api异常处理过滤器
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
        /// 获取或设置是否过滤开发环境的异常
        /// 默认为false
        /// </summary>
        public bool FilterDevelopmentException { get; set; } = false;

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

            if (this.FilterDevelopmentException == false &&
                context.HttpContext.RequestServices.GetService<IWebHostEnvironment>().IsDevelopment())
            {
                return;
            }

            var apiResult = new ApiResult<object>
            {
                Code = Code.ServiceError,
                Message = context.Exception.Message
            };

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
