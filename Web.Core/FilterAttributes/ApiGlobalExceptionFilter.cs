using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

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
            var apiResult = new ApiResult<object>
            {
                Code = Code.ServiceError,
                Message = context.Exception.Message
            };

            if (context.Exception is ArgumentException)
            {
                apiResult.Code = Code.ParameterError;
            }

            context.Result = new ObjectResult(apiResult);
        }
    }
}
