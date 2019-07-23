namespace Core
{
    /// <summary>
    /// 表示Api业务状态码
    /// </summary>
    public enum Code
    {
        /// <summary>
        /// 无错误 
        /// </summary>
        NoError = 0,

        /// <summary>
        /// 参数错误
        /// 参数验证不通过
        /// </summary>
        ParameterError = 1,

        /// <summary>
        /// 服务器处理异常
        /// </summary>
        ServiceError = 2,

        /// <summary>
        /// 其它错误
        /// </summary>
        Other = 99
    }
}
