namespace Web.Core.ClaimsPrincipals
{
    /// <summary>
    /// jwt凭据信息接口
    /// </summary>
    public interface IJwtClaimsPrincipal
    {
        /// <summary>
        /// 转换为T类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>();
    }
}
