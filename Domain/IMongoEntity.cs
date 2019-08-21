using System;

namespace Domain
{
    /// <summary>
    /// 定义Mongodb实体接口
    /// </summary>
    public interface IMongoEntity : IStringIdable
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }
    }
}
