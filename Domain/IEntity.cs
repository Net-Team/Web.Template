using System;

namespace Domain
{
    /// <summary>
    /// 定义实体的接口
    /// </summary>
    public interface IEntity : IStringIdable
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }

        /// <summary>
        /// 最近更新时间
        /// </summary>
        DateTime LastModifyTime { get; set; }
    }
}
