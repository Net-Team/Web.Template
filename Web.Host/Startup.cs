using Core;
using Core.HttpApis;
using Core.Web;
using Core.Web.Conventions;
using Core.Web.ModelBinding;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json.Serialization;

namespace Web.Host
{
    /// <summary>
    /// 启动页
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 获取本服务的配置信息
        /// </summary>
        private readonly ServiceOptions thisService;

        /// <summary>
        /// 获取配置
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 环境
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>     
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

            configuration.GetSection("ExceptionLess").BindDefaultExceptionLess();
            thisService = Configuration.GetSection($"Options:{nameof(ServiceOptions)}").Get<ServiceOptions>();
        }


        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="services"></param>  
        public void ConfigureServices(IServiceCollection services)
        {
            foreach (var assembily in AssemblyLoadContext.Default.Assemblies)
            {
                if (assembily.FullName?.StartsWith("System") == false && assembily.FullName?.StartsWith("Microsoft") == false)
                {
                    services.AddDependencyServices(assembily);
                    services.AddHttpApis(assembily, Configuration.GetSection("HttpApi"));
                    services.AddConfigureOptions(assembily, this.Configuration.GetSection("Options"));
                }
            }

            // 数据库相关
            services
                .AddDbContext<SqlDbContext>(options => // 关系数据库
                {
                    options.UseSqlServer(Configuration.GetConnectionString("SqlDbContext"), c =>
                    {
                        c.UseNetTopologySuite();
                    });
                })
                .AddSingleton<IConnectionMultiplexer>(p => // Redis连接
                {
                    var connectionString = Configuration.GetConnectionString("Redis");
                    return ConnectionMultiplexer.Connect(connectionString);
                })
                .AddTransient(p =>  // Redis IDatabase
                {
                    return p.GetService<IConnectionMultiplexer>().GetDatabase();
                })
                .AddSingleton(p => // mongodb
                {
                    var connectionString = Configuration.GetConnectionString("Mongodb");
                    return new MongoDbContext(connectionString);
                });

            services
                .AddMemoryCache()
                .AddHttpContextAccessor()
                .AddJwtParser()                  // 添加认证配置                
                .AddApiResultInvalidModelState() // 模型验证转换为ApiResult输出
                .AddNamespaceApiVersioning()     // 添加版本控制
                .AddSwaggerJwtAuth()             // 添加swagger的Bearer token
                .AddSwaggerDocUIAndEndpoints()
                .AddSwaggerGen(c =>
                {
                    c.IgnoreObsoleteActions();
                    c.IgnoreObsoleteProperties();
                    c.EnableAnnotations();
                    c.AddRequiredFilters();
                    c.AddApiVersionHeaderFilter();
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Core.xml"));
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Domain.xml"));
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{this.GetType().Assembly.GetName().Name}.xml"));
                });

            // 添加控制器
            services.AddControllers(c =>
            {
                c.Conventions.Add(new ApiExplorerGroupNameConvention());
                c.Conventions.Add(new ServiceTemplateConvention(thisService.Name));
                c.ModelBinderProviders.Insert(0, new StringPropertyTrimModelBinderProvider(c, p => p.IsDefined(typeof(StringTrimFlagAttribute))));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(JsonStringToEnumConverter.Default);
                options.JsonSerializerOptions.Converters.Add(JsonStringToNumberConverter.Default);
                options.JsonSerializerOptions.Converters.Add(JsonLocalDateTimeConverter.Default);
            });

            // 添加心跳检测
            services.AddHealthChecks();

            // 路由小写
            services.AddRouting(c => c.LowercaseUrls = true);
        }

        /// <summary>
        /// 配置中间件
        /// </summary>
        /// <param name="app"></param>      
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();

            app.UseJwtParser();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(thisService.HealthRoute);
            });
        }
    }
}
