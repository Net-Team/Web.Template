using Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Web.Core
{
    /// <summary>
    /// 表示Api异常处理过滤器
    /// </summary>
    public class ApiExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        /// <summary>
        /// 异常时
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var env = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
            if (env.IsDevelopment() == true)
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
