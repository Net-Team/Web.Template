namespace Core
{
    /// <summary>
    /// 提供生成Api响应结果
    /// </summary>
    public static class ApiResult
    {
        /// <summary>
        /// 生成无错误的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static ApiResult<T> NoError<T>(T data)
        {
            return new ApiResult<T>
            {
                Code = Code.NoError,
                Data = data
            };
        }

        /// <summary>
        /// 生成参数错误的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">提示消息</param>
        /// <returns></returns>
        public static ApiResult<T> ParameterError<T>(string message)
        {
            return new ApiResult<T>
            {
                Code = Code.ParameterError,
                Message = message
            };
        }
    }

    /// <summary>
    /// 表示Api响应结果
    /// </summary>
    public class ApiResult<T>
    {
        /// <summary>
        /// 获取或设置状态码
        /// </summary>
        public Code Code { get; set; }

        /// <summary>
        /// 获取或设置提示消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 获取或设置数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 从data转换得到
        /// </summary>
        /// <param name="data"></param>
        public static implicit operator ApiResult<T>(T data)
        {
            return ApiResult.NoError(data);
        }
    }
}
