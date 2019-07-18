using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Web.Core.Filters
{
    /// <summary>
    /// 表示菜单项特性
    /// 并提供菜单访问的验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MenuItemAttribute : Attribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// 获取菜单项名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取或设置所在分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 菜单项特性
        /// </summary>
        /// <param name="name">菜单项名称</param>
        public MenuItemAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 菜单请求权限验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var apiExplorer = context.HttpContext.RequestServices.GetService(typeof(IApiDescriptionGroupCollectionProvider)) as IApiDescriptionGroupCollectionProvider;
            var api = apiExplorer.ApiDescriptionGroups.Items.SelectMany(item => item.Items).FirstOrDefault(item => item.ActionDescriptor.Id == context.ActionDescriptor.Id);
            var apiRelativePath = api.RelativePath;

        }
    }
}
