using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Core.Web.Swaggers
{
    /// <summary>
    /// SwaggerOptions的RouteTemplate配置
    /// </summary>
    public class SwaggerOptionsConfigureOptions : IConfigureOptions<SwaggerOptions>
    {
        private readonly ServiceOptions thisService;

        /// <summary>
        /// SwaggerOptions的RouteTemplate配置
        /// </summary>
        /// <param name="thisService"></param>
        public SwaggerOptionsConfigureOptions(IOptions<ServiceOptions> thisService)
        {
            this.thisService = thisService.Value;
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerOptions options)
        {
            options.RouteTemplate = $"/swagger/{thisService.Name}/{{documentName}}/swagger.json";
        }
    }
}
