namespace Web.Host.Startups.Jwt
{
    /// <summary>
    /// jwt选项
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// jwt的cookie名称
        /// </summary>
        public string CookieName { get; set; }

        /// <summary>
        /// jwt的query名称
        /// </summary>
        public string QueryName { get; set; }
    }
}
