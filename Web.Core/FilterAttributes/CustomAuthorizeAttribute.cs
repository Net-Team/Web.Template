using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Web.Core.FilterAttributes
{
    /// <summary>
    /// 表示自定义授权验证特性
    /// </summary>
    public abstract class CustomAuthorizeAttribute : Attribute, IAuthorizeData, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// 获取或设置授权体系
        /// </summary>
        string IAuthorizeData.AuthenticationSchemes { get; set; } = "Bearer";

        /// <summary>
        /// 获取或设置策略名称
        /// </summary>
        string IAuthorizeData.Policy { get; set; }

        /// <summary>
        /// 获取或设置角色
        /// </summary>
        string IAuthorizeData.Roles { get; set; }

        /// <summary>
        /// 授权验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task OnAuthorizationAsync(AuthorizationFilterContext context);
    }
}
