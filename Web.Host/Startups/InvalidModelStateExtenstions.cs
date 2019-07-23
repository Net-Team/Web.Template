using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
            return services.PostConfigure<ApiBehaviorOptions>(c =>
            {
                c.InvalidModelStateResponseFactory = context =>
                {
                    var keyValue = context.ModelState.FirstOrDefault(item => item.Value.Errors.Count > 0);
                    var message = $"参数{keyValue.Key}验证失败：{keyValue.Value.Errors[0].ErrorMessage}";

                    var apiResult = ApiResult.ParameterError<object>(message);
                    return new ObjectResult(apiResult);
                };
            });
        }
    }
}
