using Core.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;
using Web.Core.Controllers;

namespace Web.Host.Controllers
{
    /// <summary>
    /// 菜单控制器
    /// </summary>
    [Route("api/[controller]")]
    public class MenusController : ApiController
    {
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <param name="apiExplorer"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings()]
        public IList<MenuItem> Get([FromServices]IApiDescriptionGroupCollectionProvider apiExplorer)
        {
            MenuItemAttribute GetMenuItemAttribute(ApiDescription description)
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

            var menus = new List<MenuItem>();
            foreach (var item in apiExplorer.ApiDescriptionGroups.Items)
            {
                foreach (var api in item.Items)
                {
                    var attribute = GetMenuItemAttribute(api);
                    if (attribute != null)
                    {
                        var menuItem = new MenuItem
                        {
                            Name = attribute.Name,
                            GroupName = attribute.GroupName,

                            HttpMethod = api.HttpMethod,
                            RelativePath = api.RelativePath
                        };
                        menus.Add(menuItem);
                    }
                }
            }

            return menus;
        }
    }
}
