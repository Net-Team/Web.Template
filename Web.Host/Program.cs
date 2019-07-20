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
        /// �������
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// ����host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var listen = configuration.GetValue<string>($"{nameof(ServiceInfo)}:{nameof(ServiceInfo.Listen)}");

            return Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls(listen)
                        .UseStartup<Startup>()
                        .UseSerilog((hosting, logger) => logger
                            .Enrich.FromLogContext()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .WriteTo.Debug()
                            .WriteTo.Console()
                            .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day,outputTemplate: "{NewLine}{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{NewLine}{Message:lj}{NewLine}{Exception}")
                        );
                });
        }
    }
}
