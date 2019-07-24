using Exceptionless;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Host.Startups
{
    /// <summary>
    /// ExceptionLess扩展
    /// </summary>
    public static class ExceptionLessExtensions
    {
        /// <summary>
        /// 初始化ExceptionLess客户端
        /// </summary>      
        /// <param name="configurationSection"></param>
        public static void BindDefaultExceptionLess(this IConfigurationSection configurationSection)
        {
            configurationSection.Bind(ExceptionlessClient.Default.Configuration);
            ExceptionlessClient.Default.Startup();
        }
    }
}
