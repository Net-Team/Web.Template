using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.IO;

namespace Web.Admin
{
    /// <summary>
    /// Ӧ�ó���
    /// </summary>
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
            return Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
                // ʹ��$ǰ׺�Ļ����������ø���appsettings.json������@connectionStrings:redis=somevalue   
                // ע�⣬����������Ҫ��launchSettings.json����ϵͳ��������������ֱ�����ò���ϵͳ������docker����������������
                .ConfigureAppConfiguration((h, c) => c.AddEnvironmentVariables("@"))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseSerilog((hosting, logger) =>
                        {
                            var serilog = logger.Enrich.FromLogContext()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                .WriteTo.Exceptionless(restrictedToMinimumLevel: LogEventLevel.Error)
                                .WriteTo.File(Path.Combine("logs", @"Error.txt"), restrictedToMinimumLevel: LogEventLevel.Error, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}");

                            if (hosting.HostingEnvironment.IsDevelopment())
                            {
                                serilog.WriteTo.Debug().WriteTo.Console();
                            }
                        });
                });
        }
    }
}
