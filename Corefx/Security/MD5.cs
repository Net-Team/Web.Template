using System.IO;
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
        /// <param name="str">源字符串</param>
        /// <param name="upperCase">是否使用大写</param>
        /// <returns></returns>
        public static string ComputeHashString(string str, bool upperCase = true)
        {
            if (str == null)
            {
                return null;
            }

            using var md5 = new MD5CryptoServiceProvider();
            var format = upperCase ? "X2" : "x2";
            var data = md5.ComputeHash(Encoding.UTF8.GetBytes(str)).Select(item => item.ToString(format)).ToArray();
            return string.Join(string.Empty, data);
        }


        /// <summary>
        /// 获取Md5
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="upperCase">是否使用大写</param>
        /// <returns></returns>
        public static string ComputeHashString(Stream stream, bool upperCase = true)
        {
            if (stream == null)
            {
                return null;
            }

            using var md5 = new MD5CryptoServiceProvider();
            var format = upperCase ? "X2" : "x2";
            var data = md5.ComputeHash(stream).Select(item => item.ToString(format)).ToArray();
            return string.Join(string.Empty, data);
        }

        /// <summary>
        /// 获取Md5
        /// </summary>
        /// <param name="byteArray">数据</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="upperCase">是否使用大写</param>
        /// <returns></returns>
        public static string ComputeHashString(byte[] byteArray, int offset, int count, bool upperCase = true)
        {
            if (byteArray == null)
            {
                return null;
            }

            using var md5 = new MD5CryptoServiceProvider();
            var format = upperCase ? "X2" : "x2";
            var data = md5.ComputeHash(byteArray, offset, count).Select(item => item.ToString(format)).ToArray();
            return string.Join(string.Empty, data);
        }
    }
}
