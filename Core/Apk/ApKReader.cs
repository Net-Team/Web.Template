using AndroidXml;
using System;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Core.Apk
{
    /// <summary>
    /// Apk读取器
    /// </summary>
    public static class ApkReader
    {
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="apkFile">apk文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="BadImageFormatException"></exception>
        /// <returns></returns>
        public static ApkMedia ReadMedia(string apkFile)
        {
            if (apkFile == null)
            {
                throw new ArgumentNullException(nameof(apkFile));
            }

            using var apkStream = new FileStream(apkFile, FileMode.Open, FileAccess.Read);
            return ReadMedia(apkStream);
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="apkStream">apk流</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="BadImageFormatException"></exception>
        /// <returns></returns>
        public static ApkMedia ReadMedia(Stream apkStream)
        {
            if (apkStream == null)
            {
                throw new ArgumentNullException(nameof(apkStream));
            }

            var position = apkStream.CanSeek ? apkStream.Position : 0L;
            var zip = new ZipArchive(apkStream, ZipArchiveMode.Read);
            var entry = zip.GetEntry("AndroidManifest.xml");
            if (entry == null)
            {
                throw new BadImageFormatException("找不到AndroidManifest.xml，文件可能不是有效有apk格式");
            }

            var stream = new MemoryStream();
            entry.Open().CopyTo(stream);
            stream.Position = 0;

            var reader = new AndroidXmlReader(stream);
            var xml = XDocument.Load(reader).ToString();
            Func<string, Match> getAttr = (attr) => Regex.Match(xml, @"(?<=" + attr + @"\=\"").*?(?=\"")", RegexOptions.IgnoreCase);

            var versionCode = getAttr("versionCode").Value;
            var versionName = getAttr("versionName").Value;
            var package = getAttr("package").Value;
            var media = new ApkMedia
            {
                VersionCode = int.Parse(versionCode),
                VersionName = versionName,
                Package = package
            };

            if (apkStream.CanSeek == true)
            {
                apkStream.Position = position;
                media.FileMd5 = MD5.ComputeHashString(apkStream, false);
            }

            return media;
        }
    }
}
