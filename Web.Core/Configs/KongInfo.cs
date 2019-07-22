using System;

namespace Web.Core.Configs
{
    /// <summary>
    /// kong
    /// </summary>
    public class KongInfo
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
