using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.IO;

namespace Web.Host
{
    /// <summary>
    /// 应用程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 创建host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
                // 使用$前缀的环境变量配置覆盖appsettings.json，比如@connectionStrings:redis=somevalue   
                // 注意，开发环境需要在launchSettings.json配置系统变量，生产环境直接配置操作系统变量或docker容器启动环境变量
                .ConfigureAppConfiguration((h, c) => c.AddEnvironmentVariables("@"))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseSerilog((hosting, logger) => logger
                            .Enrich.FromLogContext()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .WriteTo.Debug()
                            .WriteTo.Console()
                            .WriteTo.Exceptionless(restrictedToMinimumLevel: LogEventLevel.Error)
                            .WriteTo.File(Path.Combine("Logs", @"Error.txt"), restrictedToMinimumLevel: LogEventLevel.Error, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                        );
                });
        }

    }
}
