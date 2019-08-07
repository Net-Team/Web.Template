using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Core.Jwt
{
    /// <summary>
    /// jwt凭据信息
    /// </summary>
    public class JwtClaimsPrincipal : ClaimsPrincipal
    {
        private readonly string roleClaimType;
        private readonly string payloadJson;

        /// <summary>
        /// jwt凭据信息
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="roleClaimType"></param>
        /// <param name="payloadJson"></param>
        public JwtClaimsPrincipal(ClaimsIdentity identity, string roleClaimType, string payloadJson)
            : base(identity)
        {
            this.roleClaimType = roleClaimType;
            this.payloadJson = payloadJson;
        }

        /// <summary>
        /// 是否在角色里
        /// </summary>
        /// <param name="role">角色</param>
        /// <returns></returns>
        public override bool IsInRole(string role)
        {
            return this.HasClaim(this.roleClaimType, role);
        }

        /// <summary>
        /// 是否有Claim
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool HasClaim(string type, string value)
        {
            return this.Claims.Any(item => item.Type.EqualsIgnoreCase(type) && item.Value.EqualsIgnoreCase(value));
        }

        /// <summary>
        /// 查找第一个claim
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override Claim FindFirst(string type)
        {
            return this.Claims.FirstOrDefault(item => item.Type.EqualsIgnoreCase(type));
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.payloadJson;
        }


        /// <summary>
        /// 转换为T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>()
        {
            return JsonSerializer.Parse<T>(this.payloadJson);
        }
    }
}
