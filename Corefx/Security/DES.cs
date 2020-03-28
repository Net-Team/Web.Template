using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace System.Security
{
    /// <summary>
    /// DES加解密提供者
    /// </summary>
    public class DES
    {
        /// <summary>
        /// 默认RgbKey
        /// </summary>
        private static readonly byte[] defaultRgbKey = { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };

        /// <summary>
        /// 默认RgbIV
        /// </summary>
        private static readonly byte[] defaultRgbIV = { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };

        /// <summary>
        /// RgbKey
        /// </summary>
        private readonly byte[] rgbKey;

        /// <summary>
        /// RgbIV
        /// </summary>
        private readonly byte[] rgbIV;

        /// <summary>
        /// des加解密提供者
        /// </summary>
        private readonly DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

        /// <summary>
        /// 内置的默认实例
        /// </summary>
        public static DES Default { get; } = new DES(defaultRgbKey, defaultRgbIV);


        /// <summary>
        /// DES加解密提供者
        /// </summary>
        /// <param name="rgbKey"></param>
        /// <param name="rgbIV"></param>
        public DES(byte[] rgbKey, byte[] rgbIV)
        {
            if (rgbKey == null || rgbKey.Length != 8)
            {
                throw new ArgumentException($"{nameof(rgbKey)}必须为8个字节");
            }
            if (rgbIV == null || rgbIV.Length != 8)
            {
                throw new ArgumentException($"{nameof(rgbIV)}必须为8个字节");
            }

            this.rgbKey = rgbKey;
            this.rgbIV = rgbIV;
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="value">待加密的字符串</param>
        /// <returns></returns>
        public string Encrypt(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return value;
            }

            try
            {
                DecryptCore(value);
                return value;
            }
            catch (Exception)
            {
                return EncryptCore(value);
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="value">待解密的字符串</param>    
        /// <returns></returns>
        public string Decrypt(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return value;
            }

            try
            {
                return DecryptCore(value);
            }
            catch (Exception)
            {
                return value;
            }
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="value">待加密的字符串</param>
        /// <returns></returns>
        private string EncryptCore(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value.NullThenEmpty());
            using var stream = new MemoryStream();
            using var cryptoStream = new CryptoStream(stream, desProvider.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(stream.ToArray());
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="value">待解密的字符串</param>    
        /// <returns></returns>
        private string DecryptCore(string value)
        {
            var bytes = Convert.FromBase64String(value);
            using var stream = new MemoryStream();
            using var cryptoStream = new CryptoStream(stream, desProvider.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
