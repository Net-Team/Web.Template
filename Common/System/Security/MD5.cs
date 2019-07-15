using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.Security
{
    /// <summary>
    /// Md5摘要
    /// </summary>
    public static class MD5
    {
        /// <summary>
        /// 获取Md5
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static string ComputeHashString(string source)
        {
            if (source == null)
            {
                return null;
            }

            var data = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(source)).Select(item => item.ToString("X2")).ToArray();
            return string.Join(string.Empty, data);
        }
    }
}
