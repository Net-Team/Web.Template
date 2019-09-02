using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Core.Web.Swaggers
{
    /// <summary>
    /// 表示swagger的值类型参数自动Required标记过滤器
    /// </summary>
    public class SwaggerRequiredParameterFilter : IParameterFilter
    {
        /// <summary>
        /// 应用参数过滤器
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (context.ApiParameterDescription.DefaultValue == null)
            {
                if (context.ApiParameterDescription.Type.CanBeNullValue() == false)
                {
                    parameter.Required = true;
                }
            }
        }
    }
}
