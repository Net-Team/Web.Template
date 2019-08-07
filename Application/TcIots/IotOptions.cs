using Core;
using System;
using System.Net.Http.Headers;
using System.Security;
using WebApiClient.Parameterables;

namespace Application.TcIots
{
    /// <summary>
    /// 太川IOT选项
    /// </summary>
    public class IotOptions : IConfigureOptions
    {
        /// <summary>
        /// 应用id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 应用token
        /// </summary>
        public string AppToken { get; set; }

        /// <summary>
        /// 默认域名
        /// </summary>
        public Uri DefaultHttpHost { get; set; }

        /// <summary>
        /// 转换为授权请求头
        /// </summary>
        /// <returns></returns>
        public AuthenticationHeaderValue ToAuthorization()
        {
            var userName = this.AppId;
            var password = GetPassword(this.AppToken);
            return new BasicAuth(userName, password).ToAuthenticationHeaderValue();
        }

        /// <summary>
        /// 获取密码
        /// </summary>
        /// <param name="appToken">appToken</param>
        /// <returns></returns>
        private static string GetPassword(string appToken)
        {
            var expir = DateTimeOffset.UtcNow.AddDays(1d).ToUnixTimeSeconds().ToString();
            var md5 = MD5.ComputeHashString(appToken + expir);
            return $"expir={expir};md5={md5}";
        }
    }
}
