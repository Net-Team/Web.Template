using Core;

namespace Application.Qinius
{
    /// <summary>
    /// 表示7牛选项
    /// </summary>
    public class QiniuOptions : IConfigureOptions
    {
        /// <summary>
        /// AK
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// SK
        /// </summary>
        public string SecretKey { get; set; }
    }
}
