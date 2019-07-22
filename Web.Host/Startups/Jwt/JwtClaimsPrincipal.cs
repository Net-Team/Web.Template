﻿using System;
using System.Linq;
using System.Security.Claims;

namespace Web.Host.Startups.Jwt
{
    /// <summary>
    /// jwt凭据信息
    /// </summary>
    public class JwtClaimsPrincipal : ClaimsPrincipal
    {
        private readonly string payloadJson;

        /// <summary>
        /// jwt凭据信息
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="payloadJson"></param>
        public JwtClaimsPrincipal(ClaimsIdentity identity, string payloadJson)
            : base(identity)
        {
            this.payloadJson = payloadJson;
        }

        /// <summary>
        /// 是否在角色里
        /// </summary>
        /// <param name="role">角色</param>
        /// <returns></returns>
        public override bool IsInRole(string role)
        {
            return this.HasClaim("role", role);
        }

        /// <summary>
        /// 是否有Claim
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool HasClaim(string type, string value)
        {
            return this.Claims.Any(item => item.Type.EqualsIgnoreCase(type) && item.Value.Equals(value));
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
    }
}
