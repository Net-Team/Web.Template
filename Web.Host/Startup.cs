using Application;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.IO;
using Web.Core.FilterAttributes;
using Web.Core.Options;
using Web.Core.Startups;
using Web.Host.Startups;

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
            thisService = Configuration.GetSection($"{nameof(ServiceOptions)}").Get<ServiceOptions>();
        }


        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="services"></param>  
        public void ConfigureServices(IServiceCollection services)
        {
            // 添加IOptions<ServiceInfo>       
            services.Configure<ServiceOptions>(Configuration.GetSection(nameof(ServiceOptions)));

            // 添加缓存和数据库
            services.AddMemoryCache();
            services.AddDbContext<SqlDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlDbContext"));
            });

            // 添加Redis连接
            services.AddSingleton<IConnectionMultiplexer>(p =>
            {
                var connectionString = Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connectionString);
            }).AddTransient(p =>
            {
                return p.GetService<IConnectionMultiplexer>().GetDatabase();
            });

            // 添加mongodb
            services.AddSingleton(p =>
            {
                var connectionString = Configuration.GetConnectionString("Mongodb");
                return new MongoDbContext(connectionString);
            });

            // 添加httpApi与applicationService服务
            services.AddHttpApis(typeof(ApplicationService).Assembly);
            services.AddDependencyServices(typeof(ApplicationService).Assembly);

            // 添加认证配置
            services.AddJwtParser();

            // 添加swagger的Bearer token
            services.AddSwaggerJwtAuth();

            // 模型验证转换为ApiResult输出
            services.AddApiResultInvalidModelState();

            // 添加swagger文档
            services.AddSwaggerGen(c =>
            {
                c.IgnoreObsoleteActions();
                c.EnableAnnotations();
                c.SchemaFilter<SwaggerRequiredSchemaFilter>(true);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{thisService.Name} Api", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Core.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Domain.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Application.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Web.Core.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{this.GetType().Assembly.GetName().Name}.xml"));
            });

            // 添加控制器
            var mvc = services.AddControllers(c =>
            {
                c.Filters.Add<ApiGlobalExceptionFilter>();
                c.Conventions.Add(new ServiceTemplateConvention(thisService.Name));
            });

            if (Environment.IsDevelopment())
            {
                mvc.AddNewtonsoftJson(o => o.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented);
            }

            // 添加心跳检测
            services.AddHealthChecks();              
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

            app.UseSwagger(c =>
            {
                c.RouteTemplate = $"/swagger/{thisService.Name}/{{documentName}}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = $"{thisService.Name}的openApi文档";
                c.RoutePrefix = $"swagger/{thisService.Name}";
                c.SwaggerEndpoint($"/swagger/{thisService.Name}/v1/swagger.json", "v1 doc");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(thisService.HealthRoute);
            });
        }
    }
}
