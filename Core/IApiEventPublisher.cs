using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 定义Api消息发布器的接口
    /// </summary>
    public interface IApiEventPublisher
    {
        /// <summary>
        /// 发布Api请求消息
        /// </summary>
        /// <param name="channel">通道</param>
        /// <param name="apiEvent">Api事件</param>
        /// <returns></returns>
        Task PulishAsync(string channel, IApiEvent apiEvent);
    }
}
