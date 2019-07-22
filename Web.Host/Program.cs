using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.IO;

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
            return Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
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
                            .WriteTo.File(Path.Combine("logs", @"error.txt"), restrictedToMinimumLevel: LogEventLevel.Error, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                        );
                });
        }


    }
}
