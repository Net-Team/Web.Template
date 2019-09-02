using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Linq;

namespace Core.Web.Swaggers
{
    /// <summary>
    /// 配置SwaggerUIOptions的SwaggerEndpoint
    /// </summary>
    public class SwaggerUIOptionsConfigureOptions : IConfigureOptions<SwaggerUIOptions>
    {
        private readonly IApiDescriptionGroupCollectionProvider apiExplorer;
        private readonly ServiceOptions thisService;

        /// <summary>
        /// 配置SwaggerUIOptions的SwaggerEndpoint
        /// </summary>
        /// <param name="apiExplorer"></param>
        /// <param name="thisService"></param>
        public SwaggerUIOptionsConfigureOptions(IApiDescriptionGroupCollectionProvider apiExplorer, IOptions<ServiceOptions> thisService)
        {
            this.apiExplorer = apiExplorer;
            this.thisService = thisService.Value;
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerUIOptions options)
        {
            options.RoutePrefix = $"swagger/{thisService.Name}";
            options.DocumentTitle = $"{thisService.Name}的openApi文档";

            foreach (var group in apiExplorer.ApiDescriptionGroups.Items.OrderBy(item => item.GroupName))
            {
                options.SwaggerEndpoint($"/swagger/{thisService.Name}/{group.GroupName}/swagger.json", group.GroupName);
            }
        }
    }
}
