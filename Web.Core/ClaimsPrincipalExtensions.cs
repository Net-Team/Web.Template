using System.Security.Claims;
using Web.Core.ClaimsPrincipals;

namespace Web.Core
{
    /// <summary>
    /// ClaimsPrincipal扩展
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// 转换为用户模型
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="user"></param>
        /// <returns></returns>
        public static TUser As<TUser>(this ClaimsPrincipal user)
        {
            if (user is IJwtClaimsPrincipal jwtUser)
            {
                return jwtUser.As<TUser>();
            }
            return default(TUser);
        }
    }
}
