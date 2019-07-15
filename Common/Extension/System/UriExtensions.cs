using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// Uri扩展
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// 合并URL
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativeUri">相对地址</param>
        /// <returns></returns>
        public static Uri Combine(this Uri baseUrl, string relativeUri)
        {
            if (baseUrl == null || baseUrl.IsAbsoluteUri == false)
            {
                throw new NotSupportedException();
            }
            Uri fullUrl;
            Uri.TryCreate(baseUrl, relativeUri, out fullUrl);
            return fullUrl;
        }

        /// <summary>
        /// 更换scheme
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="scheme">新的scheme</param>
        /// <returns></returns>
        public static Uri ChangeScheme(this Uri uri, string scheme)
        {
            if (string.IsNullOrEmpty(scheme))
            {
                return uri;
            }

            var value = Regex.Replace(uri.ToString(), @"^" + uri.Scheme, scheme);
            return new Uri(value, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// 更换host
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="host">新的host</param>
        /// <returns></returns>
        public static Uri ChangeHost(this Uri uri, string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                return uri;
            }

            var value = Regex.Replace(uri.ToString(), "(?<=//)" + uri.Host, host);
            return new Uri(value, UriKind.RelativeOrAbsolute);
        }
    }
}
