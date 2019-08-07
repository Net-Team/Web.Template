using Core.Jwt;
using System.Security.Claims;

namespace Core
{
    /// <summary>
    /// ClaimsPrincipal扩展
    /// </summary>
    public static class JwtClaimsPrincipalExtensions
    {
        /// <summary>
        /// 转换为用户模型
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="user"></param>
        /// <returns></returns>
        public static TUser As<TUser>(this ClaimsPrincipal user)
        {
            if (user is JwtClaimsPrincipal jwtUser)
            {
                return jwtUser.As<TUser>();
            }
            return default(TUser);
        }
    }
}
