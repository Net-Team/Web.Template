namespace Application.TcIots
{
    /// <summary>
    /// IOT数据结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IotResult<T>
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 数据内容
        /// </summary>
        public T Data { get; set; }
    }
}
