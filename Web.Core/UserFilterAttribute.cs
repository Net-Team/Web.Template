using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Web.Core
{
    /// <summary>
    /// 角色枚举
    /// </summary>
    [Flags]
    public enum Role
    {
        /// <summary>
        /// 不限制角色
        /// </summary>
        None = 0,

        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 0x1,
    }

    /// <summary>
    /// 资源范围
    /// </summary>
    public enum Scope
    {
        /// <summary>
        /// 云对讲
        /// </summary>
        RKE
    }

    /// <summary>
    /// 表示用户过滤特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserFilterAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// 允许访问的scope
        /// </summary>
        public Scope Scope { get; }

        /// <summary>
        /// 获取或设置允许的角色
        /// role1|role2
        /// </summary>
        public Role Role { get; set; } = Role.None;

        /// <summary>
        /// 用户过滤特性
        /// </summary>
        /// <param name="scope">允许访问的scope</param>
        public UserFilterAttribute(Scope scope)
        {
            this.Scope = scope;
        }

        /// <summary>
        /// 验证身份
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (this.Authorization(context) == false)
            {
                var name = context.HttpContext.User.FindFirst("name")?.Value;
                context.Result = new UnauthorizedObjectResult($"用户{name}被限制访问{this.Scope}");
            }
        }

        /// <summary>
        /// 验证身份
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool Authorization(AuthorizationFilterContext context)
        {
            if (this.IsRoleOk(context.HttpContext) == false)
            {
                return false;
            }

            if (this.IsScopeOk(context.HttpContext) == false)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 检测scope是否ok
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsScopeOk(HttpContext context)
        {
            return context.User.HasClaim("scope", this.Scope.ToString());
        }

        /// <summary>
        /// 角色是否ok
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsRoleOk(HttpContext context)
        {
            if (this.Role == Role.None)
            {
                return true;
            }

            foreach (var role in this.Role.GetFlagEnums<Role>())
            {
                if (role != Role.None)
                {
                    if (context.User.IsInRole(role.ToString()) == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
