using System.Security.Claims;

namespace Domain
{
    /// <summary>
    /// 定义当前登录用户信息
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 获取用户凭据
        /// </summary>
        ClaimsPrincipal Principal { get; }

        /// <summary>
        /// 获取凭据的sub类型
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 获取凭据的name类型
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取凭据的tenant_id类型
        /// </summary>
        string TenantId { get; }
    }
}
