namespace Core.Apk
{
    /// <summary>
    /// Apk信息
    /// </summary>
    public class ApkMedia
    {
        /// <summary>
        /// 主版本
        /// </summary>
        public int VersionCode { get; set; }

        /// <summary>
        /// 版本名
        /// </summary>
        public string VersionName { get; set; }

        /// <summary>
        /// 包名
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// 文件Md5加密
        /// </summary>
        public string FileMd5 { get; set; }
    }
}
