using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Web.Startups
{
    /// <summary>
    /// swagger不可空值类型参数或模型属性标记为Required
    /// </summary>
    public static class SwaggerRequiredExtensions
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
    }
}
