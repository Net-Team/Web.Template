using Application.Menus;
using Core;
using Core.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Linq;
using System.Threading.Tasks;
using Web.Admin.Menus;
using Web.Core;

namespace Web.Admin.Controllers
{
    /// <summary>
    /// 菜单控制器
    /// </summary>   
    [UserFilter(Scope.RKE, Role = Role.Admin)]
    [Route("api/[service]/menus")]
    public class MenusController : ApiController
    {
        /// <summary>
        /// 获取当前登录者所有可用的菜单
        /// </summary>
        /// <param name="menuService"></param>
        /// <param name="apiExplorer"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<MenuGroup[]>> Get(
            [FromServices]MenuService menuService,
            [FromServices]IApiDescriptionGroupCollectionProvider apiExplorer)
        {
            var userId = this.HttpContext.User.FindFirst("sub")?.Value;
            var userMenus = await menuService.GetMenusAsync(userId);
            var allMenuItems = GetMenuItems(apiExplorer);

            var menuItems = from a in allMenuItems
                            join u in userMenus
                            on a.RelativePath equals u.RelativePath
                            select new MenuItem
                            {
                                Name = a.Name,
                                GroupName = a.GroupName,
                                Class = a.Class,
                                RelativePath = a.RelativePath,
                                Enable = true
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
        /// 获取子管理员用户的所有菜单
        /// </summary>
        /// <param name="userId">子管理员用户id</param>
        /// <param name="menuService"></param>
        /// <param name="apiExplorer"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        [MenuItem(Name.功能权限, Group.基础数据)]
        public async Task<ApiResult<MenuGroup[]>> Get(
            string userId,
            [FromServices]MenuService menuService,
            [FromServices]IApiDescriptionGroupCollectionProvider apiExplorer)
        {
            var myId = this.HttpContext.User.FindFirst("sub")?.Value;
            var myMenus = await menuService.GetMenusAsync(myId);
            var userMenus = await menuService.GetMenusAsync(userId);
            var allMenuItems = GetMenuItems(apiExplorer);

            var myMenuItems =
                from m in myMenus
                join a in allMenuItems
                on m.RelativePath equals a.RelativePath
                select new MenuItem
                {
                    Name = a.Name,
                    GroupName = a.GroupName,
                    Class = a.Class,
                    RelativePath = a.RelativePath,
                    Enable = true
                };

            var userMenuItems =
                from m in myMenuItems
                join u in userMenus
                on m.RelativePath equals u.RelativePath
                into g
                from item in g.DefaultIfEmpty()
                select new MenuItem
                {
                    Name = m.Name,
                    GroupName = m.GroupName,
                    Class = m.Class,
                    RelativePath = m.RelativePath,
                    Enable = item != null
                };

            var groups = userMenuItems
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
                .OrderBy(item => item.attr.Group)
                .ThenBy(item => item.attr.Name)
                .Select(item => new MenuItem
                {
                    Name = item.attr.Name.ToString(),
                    GroupName = item.attr.Group.ToString(),
                    Class = item.attr.Class,
                    RelativePath = item.api.RelativePath
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
