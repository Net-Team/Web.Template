using WebApiClient;
using WebApiClient.Attributes;

namespace Application.Baidus
{
    public interface IBaiduApi : IHttpApi
    {
        [HttpGet("/")]
        ITask<string> GetAsync();
    }
}
