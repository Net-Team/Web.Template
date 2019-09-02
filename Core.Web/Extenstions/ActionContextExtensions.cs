using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Core.Web
{
    /// <summary>
    /// 提供ActionContext扩展
    /// </summary>
    public static class ActionContextExtensions
    {
        /// <summary>
        /// 获取对应的Api描述
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ApiDescription GetApiDescription(this ActionContext context)
        {
            if (context.ActionDescriptor.Properties.TryGetValue(typeof(ApiDescription), out var value))
            {
                if (value is ApiDescription description)
                {
                    return description;
                }
            }

            var apiExplorer = context.HttpContext.RequestServices.GetService<IApiDescriptionGroupCollectionProvider>();
            var apiDescription = apiExplorer.ApiDescriptionGroups.Items.SelectMany(item => item.Items).FirstOrDefault(item => item.ActionDescriptor.Id == context.ActionDescriptor.Id);

            context.ActionDescriptor.Properties.TryAdd(typeof(ApiDescription), apiDescription);
            return apiDescription;
        }
    }
}
