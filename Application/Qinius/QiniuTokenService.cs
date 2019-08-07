using Microsoft.Extensions.Options;
using Qiniu.Storage;
using Qiniu.Util;
using System;

namespace Application.Qinius
{
    /// <summary>
    /// 7牛token服务
    /// </summary>
    public class QiniuTokenService : TransientApplicationService
    {
        private readonly QiniuOptions options;

        /// <summary>
        /// 7牛token服务
        /// </summary>
        /// <param name="options"></param>
        public QiniuTokenService(IOptions<QiniuOptions> options)
        {
            this.options = options.Value;
        }

        /// <summary>
        /// 创建存储上传token
        /// </summary> 
        /// <param name="bucketName">上传的Bucket</param>
        /// <param name="expired">有效期</param>
        /// <returns></returns>      
        public string CreateUploadToken(string bucketName, TimeSpan expired)
        {
            // 简单上传凭证
            var putPolicy = new PutPolicy
            {
                Scope = bucketName
            };

            var expires = (int)expired.TotalSeconds;
            putPolicy.SetExpires(expires);

            var mac = new Mac(this.options.AccessKey, this.options.SecretKey);
            var upToken = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            return upToken;
        }
    }
}
