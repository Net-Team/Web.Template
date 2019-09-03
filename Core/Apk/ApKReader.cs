using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
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
            using var zip = new ZipArchive(apkStream, ZipArchiveMode.Read);
            const string manifestName = "AndroidManifest.xml";
            var manifest = zip.Entries.FirstOrDefault(item => item.Name.EqualsIgnoreCase(manifestName));
            if (manifest == null)
            {
                throw new BadImageFormatException($"找不到{manifestName}，文件可能不是有效有apk格式");
            }

            var manifestStream = new MemoryStream();
            manifest.Open().CopyTo(manifestStream);
            var manifestBytes = manifestStream.ToArray();

            var xml = ManifestReader.ReadAsXmlString(manifestBytes);
            var doc = XDocument.Parse(xml);

            var versionCode = doc.Root.Attribute("versionCode").Value;
            var versionName = doc.Root.Attribute("versionName").Value;
            var package = doc.Root.Attribute("package").Value;

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
