using WebApiClient;
using WebApiClient.Attributes;

namespace Application.Baidu
{
    public interface IBaiduApi : IHttpApi
    {
        [HttpGet("/")]
        ITask<string> GetAsync();
    }
}
