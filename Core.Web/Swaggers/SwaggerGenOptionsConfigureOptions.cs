using Core.Web;
using Core.Web.Conventions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Web.Swaggers
{
    /// <summary>
    /// 配置SwaggerGenOptions的SwaggerDoc
    /// </summary>
    public class SwaggerGenOptionsConfigureOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiDescriptionGroupCollectionProvider apiExplorer;
        private readonly ServiceOptions thisService;

        /// <summary>
        /// 配置SwaggerGenOptions的SwaggerDoc
        /// </summary>
        /// <param name="apiExplorer"></param>
        /// <param name="thisService"></param>
        public SwaggerGenOptionsConfigureOptions(IApiDescriptionGroupCollectionProvider apiExplorer, IOptions<ServiceOptions> thisService)
        {
            this.apiExplorer = apiExplorer;
            this.thisService = thisService.Value;
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var group in apiExplorer.ApiDescriptionGroups.Items)
            {
                options.SwaggerDoc(group.GroupName, new OpenApiInfo
                {
                    Title = $"{thisService.Name}",
                    Version = "v" + ApiExplorerGroupNameConvention.GetApiVersion(group.GroupName).ToString()
                });
            }
        }
    }
}
