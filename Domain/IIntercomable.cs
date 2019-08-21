namespace Domain
{
    /// <summary>
    /// 定义可用于对讲的对象
    /// </summary>
    public interface IIntercomable
    {
        /// <summary>
        /// 对讲账号
        /// </summary>
        string IntercomUserId { get; set; }

        /// <summary>
        /// 对讲token
        /// </summary>
        string IntercomToken { get; set; }
    }
}
