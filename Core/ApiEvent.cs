using System;

namespace Core
{
    /// <summary>
    /// 定义表示Api事件的接口
    /// </summary>
    public interface IApiEvent
    {
        /// <summary>
        /// 请求的uri
        /// </summary>
        Uri Uri { get; set; }

        /// <summary>
        /// 请求的内容
        /// </summary>
        object Content { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 表示Api事件
    /// </summary>
    public class ApiEvent : IApiEvent
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


    /// <summary>
    /// 表示Api事件
    /// </summary>
    /// <typeparam name="TContent"></typeparam>
    public class ApiEvent<TContent> : IApiEvent
    {
        /// <summary>
        /// 请求的uri
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// 请求的内容
        /// </summary>
        public TContent Content { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 接口显式实现
        /// </summary>
        object IApiEvent.Content
        {
            get => this.Content;
            set => this.Content = (TContent)value;
        }
    }
}
