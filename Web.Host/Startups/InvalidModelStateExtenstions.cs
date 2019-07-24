using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Host.Startups
{
    /// <summary>
    /// 模型验证熔岩
    /// </summary>
    public static class InvalidModelStateExtenstions
    {
        /// <summary>
        /// 添加模型验证转换为ApiResult输出
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiResultInvalidModelState(this IServiceCollection services)
        {
            return services.AddApiResultInvalidModelState(null);
        }

        /// <summary>
        /// 添加模型验证转换为ApiResult输出
        /// </summary>
        /// <param name="services"></param>
        /// <param name="messageFormate">消息格式化</param>
        /// <returns></returns>
        public static IServiceCollection AddApiResultInvalidModelState(this IServiceCollection services, Func<KeyValuePair<string, ModelStateEntry>, string> messageFormate)
        {
            return services.PostConfigure<ApiBehaviorOptions>(c =>
            {
                c.InvalidModelStateResponseFactory = context =>
                {
                    var keyValue = context.ModelState.FirstOrDefault(item => item.Value.Errors.Count > 0);
                    var message = messageFormate != null ? messageFormate(keyValue) : $"参数{keyValue.Key}验证失败：{keyValue.Value.Errors[0].ErrorMessage}";
                    var apiResult = ApiResult.ParameterError<object>(message);
                    return new ObjectResult(apiResult);
                };
            });
        }
    }
}
