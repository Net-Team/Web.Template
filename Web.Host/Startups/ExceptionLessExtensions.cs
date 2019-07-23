using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public static void SetDefaultExceptionLess(this IConfigurationSection configurationSection)
        {
            ExceptionlessClient.Default.Configuration.ApiKey = configurationSection.GetValue<string>("ApiKey");
            ExceptionlessClient.Default.Configuration.ServerUrl = configurationSection.GetValue<string>("ServerUrl");
            ExceptionlessClient.Default.Startup();
        }
    }
}
