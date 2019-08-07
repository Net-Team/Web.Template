namespace Domain
{
    /// <summary>
    /// 定义支持多租户的接口
    /// </summary>
    public interface ITenantable
    {
        /// <summary>
        /// 获取或设置租户Id
        /// </summary>
        string TenantaId { get; set; }
    }
}
