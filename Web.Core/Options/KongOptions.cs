using System;

namespace Web.Core.Options
{
    /// <summary>
    /// kong
    /// </summary>
    public class KongOptions
    {
        /// <summary>
        /// admin接口地址
        /// </summary>
        public Uri AdminUri { get; set; }

        /// <summary>
        /// 代理接口地址
        /// </summary>
        public Uri ProxyUri { get; set; }
    }
}
