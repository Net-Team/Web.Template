using Core;
using Core.HttpApis;
using Core.Web;
using Core.Web.Options;
using Core.Web.Startups;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
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
            services
                .AddMemoryCache()
                .AddHttpContextAccessor()
                .AddHttpApis(this.GetType().Assembly) // 添加httpApi
                .AddDependencyServices(this.GetType().Assembly) // ApplicationService服务
                .AddConfigureOptions(this.Configuration.GetSection("Options"), typeof(ApiResult).Assembly, typeof(ApiController).Assembly, this.GetType().Assembly)
                .AddJwtParser() // 添加认证配置                
                .AddApiResultInvalidModelState() // 模型验证转换为ApiResult输出
                .AddSwaggerJwtAuth() // 添加swagger的Bearer token
                .AddSwaggerGen(c =>
                {
                    c.IgnoreObsoleteActions();
                    c.IgnoreObsoleteProperties();
                    c.EnableAnnotations();
                    c.SchemaFilter<SwaggerRequiredSchemaFilter>(true);
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{thisService.Name} Api", Version = "v1" });
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Core.xml")); 
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{this.GetType().Assembly.GetName().Name}.xml"));
                });

            // 添加控制器
            var mvc = services.AddControllers(c =>
            {
                c.Filters.Add<ApiExceptionFilterAttribute>();
                c.Conventions.Add(new ServiceTemplateConvention(thisService.Name));
            });

            if (Environment.IsDevelopment())
            {
                mvc.AddNewtonsoftJson(o => o.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented);
            }

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
