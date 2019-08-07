using System.Net.Http;
using WebApiClient;
using WebApiClient.Attributes;

namespace Application.UserCenters
{
    /// <summary>
    /// 用户中心用户接口
    /// </summary>
    [Gateway("api/usercenter/users")]
    public interface IUserApi : IHttpApi
    {
        [HttpPost]
        ITask<HttpResponseMessage> AddAsync();
    }
}
