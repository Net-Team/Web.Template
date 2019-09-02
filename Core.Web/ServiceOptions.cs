namespace Core.Web
{
    /// <summary>
    /// 表示服务信息
    /// </summary>
    public class ServiceOptions : IConfigureOptions
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 健康检查的路由
        /// </summary>
        public string HealthRoute { get; set; }
    }
}
