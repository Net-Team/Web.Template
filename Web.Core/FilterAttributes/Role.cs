using System;

namespace Web.Core.FilterAttributes
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
}
