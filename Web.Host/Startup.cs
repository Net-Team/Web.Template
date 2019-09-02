using Application;
using Core;
using Core.HttpApis;
using Core.Web;
using Core.Web.Conventions;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.IO;

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
                .AddHttpApis(typeof(ApplicationService).Assembly) // 添加httpApi
                .AddDependencyServices(typeof(ApplicationService).Assembly) // ApplicationService服务
                .AddConfigureOptions(this.Configuration.GetSection("Options"), typeof(ApiResult).Assembly, typeof(ApplicationService).Assembly, typeof(ApiController).Assembly, this.GetType().Assembly)
                .AddJwtParser() // 添加认证配置                
                .AddApiResultInvalidModelState() // 模型验证转换为ApiResult输出
                .AddSwaggerJwtAuth() // 添加swagger的Bearer token
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
            });

            // 添加心跳检测
            services.AddHealthChecks();

            // 路由小写
            services.AddRouting(c => c.LowercaseUrls = true);

            // api版本控制
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.Conventions.Add(new VersionByNamespaceConvention());
                o.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("x-api-version"));
            });
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
