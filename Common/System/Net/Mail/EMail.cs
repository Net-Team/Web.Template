using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.Net.Mail
{
    /// <summary>
    /// 表示邮件
    /// </summary>
    public class EMail
    {
        /// <summary>
        /// smtp端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// smtp
        /// </summary>
        public string Smtp { get; private set; }

        /// <summary>
        /// 发送者邮箱
        /// </summary>
        public string From { get; private set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// 表示邮件
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="password">密码</param>
        /// <param name="smtp">smtp</param>
        public EMail(string from, string password, string smtp)
            : this(from, password, smtp, 25)
        {
        }

        /// <summary>
        /// 表示邮件
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="password">密码</param>
        /// <param name="smtp">smtp</param>
        /// <param name="port">端口</param>
        public EMail(string from, string password, string smtp, int port)
        {
            this.From = from;
            this.Password = password;
            this.Smtp = smtp;
            this.Port = port;
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="to">接收者邮箱</param>
        /// <param name="title">标题</param>
        /// <param name="htmlBody">内容</param>
        /// <param name="ssl">ssl</param>
        /// <returns></returns>
        public async Task SendAsync(string to, string title, string htmlBody, bool ssl = false)
        {
            await this.SendAsync(new[] { to }, title, htmlBody, ssl);
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="to">接收者邮箱</param>
        /// <param name="title">标题</param>
        /// <param name="htmlBody">内容</param>
        /// <param name="ssl">ssl</param>
        /// <returns></returns>
        public async Task SendAsync(IEnumerable<string> to, string title, string htmlBody, bool ssl = false)
        {
            var msg = new MailMessage
            {
                From = new MailAddress(this.From),
                Subject = title,
                SubjectEncoding = Encoding.UTF8,
                Body = htmlBody,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
            };

            foreach (var item in to.Distinct())
            {
                if (item.IsNullOrEmpty() == false && Regex.IsMatch(item, @"^\w+(\.\w*)*@\w+\.\w+$"))
                {
                    msg.To.Add(item);
                }
            }

            if (msg.To.Count == 0)
            {
                return;
            }

            using (var client = new SmtpClient())
            {
                client.Credentials = new NetworkCredential(this.From, this.Password);
                client.Port = this.Port;
                client.Host = this.Smtp;
                client.EnableSsl = ssl;
                await client.SendMailAsync(msg);
            }
        }
    }
}

