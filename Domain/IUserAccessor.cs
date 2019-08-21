namespace Domain
{
    /// <summary>
    /// 定义用户访问接口
    /// </summary>
    public interface IUserAccesstor
    {
        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        IUser User { get; }
    }
}
