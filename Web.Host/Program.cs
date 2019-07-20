using Exceptionless;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using Web.Core.Configs;

namespace Web.Host
{
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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            InitExceptionLess(configuration);

            return Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls(configuration.GetValue<string>($"{nameof(ServiceInfo)}:{nameof(ServiceInfo.Listen)}"))
                        .UseStartup<Startup>()
                        .UseSerilog((hosting, logger) => logger
                            .Enrich.FromLogContext()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .WriteTo.Debug()
                            .WriteTo.Console()
                            .WriteTo.Exceptionless(restrictedToMinimumLevel: LogEventLevel.Error)
                            .WriteTo.File(Path.Combine("logs", @"error.txt"), restrictedToMinimumLevel: LogEventLevel.Error, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                        );
                });
        }

        /// <summary>
        /// 初始化ExceptionLess客户端
        /// </summary>
        /// <param name="configuration"></param>
        private static void InitExceptionLess(IConfigurationRoot configuration)
        {
            ExceptionlessClient.Default.Configuration.ApiKey = configuration.GetValue<string>("Logger:ExceptionLess:ApiKey");
            ExceptionlessClient.Default.Configuration.ServerUrl = configuration.GetValue<string>("Logger:ExceptionLess:ServerUrl");
            ExceptionlessClient.Default.Startup();
        }
    }
}
