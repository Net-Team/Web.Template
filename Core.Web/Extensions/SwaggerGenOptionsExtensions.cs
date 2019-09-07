using Core.Web.Swaggers;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Web
{
    /// <summary>
    /// swagger不可空值类型参数或模型属性标记为Required
    /// </summary>
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// 添加Required标记相关的Filters
        /// </summary>
        /// <param name="options"></param>
        /// <param name="camelCasePropertyNames"></param>
        public static void AddRequiredFilters(this SwaggerGenOptions options, bool camelCasePropertyNames = true)
        {
            options.SchemaFilter<SwaggerRequiredSchemaFilter>(camelCasePropertyNames);
            options.ParameterFilter<SwaggerRequiredParameterFilter>();
        }

        /// <summary>
        /// 添加Api版本请求参数相关Filter
        /// </summary>
        /// <param name="options"></param>
        /// <param name="headerName"></param>
        public static void AddApiVersionHeaderFilter(this SwaggerGenOptions options, string headerName = "x-api-version")
        {
            options.OperationFilter<SwaggerApiVersionHeaderFilter>(headerName);
        }
    }
}
