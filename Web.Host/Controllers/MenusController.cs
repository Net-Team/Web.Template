using Application.Menus;
using Core.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Core.ServiceRegistration;
using Web.Core.Controllers;
using Web.Core.FilterAttributes;

namespace Web.Host.Controllers
{
    /// <summary>
    /// 菜单控制器
    /// </summary>   
    public class MenusController : ApiController
    {
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <param name="menuService"></param>
        /// <param name="apiExplorer"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings()]
        public async Task<MenuGroup[]> Get(
            [FromServices]MenuService menuService,
            [FromServices]IApiDescriptionGroupCollectionProvider apiExplorer)
        {
            var userId = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userMenus = await menuService.GetMenusAsync(userId);
            var allMenuItems = GetMenuItems(apiExplorer);

            var menuItems = from a in allMenuItems
                            join u in userMenus
                            on a.RelativePath equals u.RelativePath
                            into g
                            from item in g.DefaultIfEmpty()
                            select new MenuItem
                            {
                                Name = a.Name,
                                GroupName = a.GroupName,
                                HttpMethod = a.HttpMethod,
                                Class = a.Class,
                                RelativePath = a.RelativePath,
                                Enable = item?.Enable == true
                            };

            var groups = menuItems
                .GroupBy(item => item.GroupName)
                .Select(g => new MenuGroup
                {
                    Group = g.Key,
                    Items = g.ToArray()
                });

            return groups.ToArray();
        }

        /// <summary>
        /// 从apiExplorer获取所有菜单
        /// </summary>
        /// <param name="apiExplorer"></param>
        /// <returns></returns>
        private static MenuItem[] GetMenuItems(IApiDescriptionGroupCollectionProvider apiExplorer)
        {
            var apis = apiExplorer.ApiDescriptionGroups.Items.SelectMany(item => item.Items);
            var menus = apis
                .Select(api => new
                {
                    api,
                    attr = GetMenuItemAttribute(api)
                })
                .Where(item => item.attr != null)
                .OrderBy(item => (int)item.attr.Group)
                .ThenBy(item => item.attr.Order)
                .Select(item => new MenuItem
                {
                    Name = item.attr.Name,
                    GroupName = item.attr.Group.ToString(),
                    Class = item.attr.Class,
                    HttpMethod = item.api.HttpMethod,
                    RelativePath = item.api.RelativePath,
                    Enable = false
                }).ToArray();

            return menus;
        }


        /// <summary>
        /// 获取ApiDescription的MenuItemAttribute
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private static MenuItemAttribute GetMenuItemAttribute(ApiDescription description)
        {
            foreach (var item in description.ActionDescriptor.EndpointMetadata)
            {
                if (item is MenuItemAttribute menuItemAttribute)
                {
                    return menuItemAttribute;
                }
            }
            return null;
        }
    }
}
