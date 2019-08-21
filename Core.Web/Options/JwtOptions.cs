namespace Core.Web.Options
{
    /// <summary>
    /// jwt选项
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// jwt的cookie名称
        /// </summary>
        public string JwtCookieName { get; set; } = "jwt";

        /// <summary>
        /// jwt的query名称
        /// </summary>
        public string JwtQueryName { get; set; } = "jwt";

        /// <summary>
        /// 角色名称
        /// 默认是role
        /// </summary>
        public string RoleClaimType { get; set; } = "role";
    }
}
