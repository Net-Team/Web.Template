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
        /// 转换为
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <returns></returns>
        public static T As<T>(this ClaimsPrincipal user)
        {
            if (user is IJwtClaimsPrincipal jwtUser)
            {
                return jwtUser.As<T>();
            }
            return default(T);
        }
    }
}
