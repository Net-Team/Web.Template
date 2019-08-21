namespace Domain
{
    /// <summary>
    /// 定义支持string类型的Id属性
    /// </summary>
    public interface IStringIdable
    {
        /// <summary>
        /// 获取或设置唯一标识
        /// </summary>
        string Id { get; set; }
    }
}
