using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;

namespace Application.UserCenters
{
    /// <summary>
    /// 用户中心用户接口
    /// </summary>
    [Gateway]
    public interface IUserApi : IHttpApi
    {
    }
}
