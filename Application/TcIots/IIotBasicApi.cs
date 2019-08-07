using WebApiClient;
using WebApiClient.Attributes;

namespace Application.TcIots
{
    /// <summary>
    /// IOT平台接口
    /// </summary>
    [IotAuth]
    [TraceFilter(OutputTarget = OutputTarget.LoggerFactory)]
    public interface IIotBasicApi : IHttpApi
    {
        /// <summary>
        /// 设备测试
        /// </summary>
        [HttpPost("v1/Device/SysTest")]
        ITask<IotResult<string>> SysTestAsync(string num, [FormContent]string data);
    }
}
