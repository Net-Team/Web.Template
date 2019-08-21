using System.ComponentModel.DataAnnotations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Core.Menus
{
    /// <summary>
    /// 表示菜单信息
    /// </summary>
    public class MenuItem : IEquatable<MenuItem>
    {
        /// <summary>
        /// 获取或设置分组名称
        /// </summary>
        [Required]
        public string GroupName { get; set; }

        /// <summary>
        /// 或取或设置名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置类名
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 获取或设置相对Uri
        /// </summary>
        [Required]
        public string RelativePath { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 与目标是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals([AllowNull] MenuItem other)
        {
            return other != null && other.RelativePath == this.RelativePath;
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.RelativePath == null ? 0 : this.RelativePath.GetHashCode();
        }
    }
}
