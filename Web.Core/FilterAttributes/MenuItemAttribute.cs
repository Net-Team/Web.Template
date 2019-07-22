using Application.Menus;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Core.FilterAttributes
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
        /// 获取或设置排序
        /// 从小到大排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 获取或设置类名
        /// 用于辅助前端设置ico等
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 获取所在分组
        /// </summary>
        public Group Group { get; }

        /// <summary>
        /// 菜单项特性
        /// </summary>
        /// <param name="name">菜单项名称</param>
        /// <param name="group">所在分组</param>
        public MenuItemAttribute(string name, Group group)
        {
            this.Name = name;
            this.Group = group;
        }


        /// <summary>
        /// 菜单请求权限验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.User.FindFirst("sub")?.Value;
            if (userId.IsNullOrEmpty() == true)
            {
                return;
            }

            var apiExplorer = context.HttpContext.RequestServices.GetService<IApiDescriptionGroupCollectionProvider>();
            var menuService = context.HttpContext.RequestServices.GetService<MenuService>();
            var memoryCache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            // 从缓存或数据库找出当前用户对应的所有菜单
            var key = $"{nameof(Menu)}:{userId}";
            var menus = await memoryCache.GetOrCreateAsync(key, e =>
            {
                e.SetSlidingExpiration(TimeSpan.FromMinutes(1d));
                return menuService.GetMenusAsync(userId);
            });

            // 查找与当前api的RelativePath对应的菜单
            var api = apiExplorer.ApiDescriptionGroups.Items.SelectMany(item => item.Items).FirstOrDefault(item => item.ActionDescriptor.Id == context.ActionDescriptor.Id);
            var enable = menus.FirstOrDefault(item => item.RelativePath == api.RelativePath)?.Enable == true;

            // 找不到菜单或菜单设置为禁用
            if (enable == false)
            {
                context.Result = new UnauthorizedObjectResult($"接口{api.RelativePath}未对用户{userId}授权");
            }
        }
    }
}
