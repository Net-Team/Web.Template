using AndroidXml;
using System;
using System.Diagnostics.CodeAnalysis;
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
        /// <param name="apkFile">文件路径</param>
        /// <exception cref="BadImageFormatException"></exception>
        /// <returns></returns>
        public static ApkMedia ReadMedia([NotNull] string apkFile)
        {
            using var fileStream = new FileStream(apkFile, FileMode.Open, FileAccess.Read);
            var zip = new ZipArchive(fileStream, ZipArchiveMode.Read);
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

            if (fileStream.CanSeek == true)
            {
                fileStream.Seek(0L, SeekOrigin.Begin);
            }
            media.FileMd5 = MD5.ComputeHashString(fileStream, false);
            return media;
        }
    }
}
