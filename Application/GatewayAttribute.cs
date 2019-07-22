using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    /// <summary>
    /// 标记http接口配置网关的域名
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class GatewayAttribute : Attribute
    {
    }
}
