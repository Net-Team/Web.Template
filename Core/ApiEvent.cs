using System;

namespace Core
{
    /// <summary>
    /// 表示Api事件
    /// </summary>
    public class ApiEvent
    {
        /// <summary>
        /// 请求的uri
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// 请求的内容
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
