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
    /// ����ҳ
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ��ȡ�������������Ϣ
        /// </summary>
        private readonly ServiceOptions thisService;

        /// <summary>
        /// ��ȡ����
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// ����
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// ������
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
        /// ��ӷ���
        /// </summary>
        /// <param name="services"></param>  
        public void ConfigureServices(IServiceCollection services)
        {
            // ���IOptions<ServiceInfo>       
            services.Configure<ServiceOptions>(Configuration.GetSection(nameof(ServiceOptions)));

            // ��ӻ�������ݿ�
            services.AddMemoryCache();
            services.AddDbContext<SqlDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlDbContext"));
            });

            // ���Redis����
            services.AddSingleton<IConnectionMultiplexer>(p =>
            {
                var connectionString = Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connectionString);
            }).AddTransient(p =>
            {
                return p.GetService<IConnectionMultiplexer>().GetDatabase();
            });

            // ���mongodb
            services.AddSingleton(p =>
            {
                var connectionString = Configuration.GetConnectionString("Mongodb");
                return new MongoDbContext(connectionString);
            });

            // ���httpApi��applicationService����
            services.AddHttpApis(typeof(ApplicationService).Assembly);
            services.AddDependencyServices(typeof(ApplicationService).Assembly);

            // �����֤����
            services.AddJwtParser();

            // ���swagger��Bearer token
            services.AddSwaggerJwtAuth();

            // ģ����֤ת��ΪApiResult���
            services.AddApiResultInvalidModelState();

            // ���swagger�ĵ�
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

            // ��ӿ�����
            var mvc = services.AddControllers(c =>
            {
                c.Filters.Add<ApiGlobalExceptionFilter>();
                c.Conventions.Add(new ServiceTemplateConvention(thisService.Name));
            });

            if (Environment.IsDevelopment())
            {
                mvc.AddNewtonsoftJson(o => o.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented);
            }

            // ����������
            services.AddHealthChecks();              
        }

        /// <summary>
        /// �����м��
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
                c.DocumentTitle = $"{thisService.Name}��openApi�ĵ�";
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
