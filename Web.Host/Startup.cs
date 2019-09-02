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
            thisService = Configuration.GetSection($"Options:{nameof(ServiceOptions)}").Get<ServiceOptions>();
        }


        /// <summary>
        /// ��ӷ���
        /// </summary>
        /// <param name="services"></param>  
        public void ConfigureServices(IServiceCollection services)
        {
            // ���ݿ����
            services
                .AddDbContext<SqlDbContext>(options => // ��ϵ���ݿ�
                {
                    options.UseSqlServer(Configuration.GetConnectionString("SqlDbContext"), c =>
                    {
                        c.UseNetTopologySuite();
                    });
                })
                .AddSingleton<IConnectionMultiplexer>(p => // Redis����
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
                .AddHttpApis(typeof(ApplicationService).Assembly) // ���httpApi
                .AddDependencyServices(typeof(ApplicationService).Assembly) // ApplicationService����
                .AddConfigureOptions(this.Configuration.GetSection("Options"), typeof(ApiResult).Assembly, typeof(ApplicationService).Assembly, typeof(ApiController).Assembly, this.GetType().Assembly)
                .AddJwtParser() // �����֤����                
                .AddApiResultInvalidModelState() // ģ����֤ת��ΪApiResult���
                .AddSwaggerJwtAuth() // ���swagger��Bearer token
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

            // ��ӿ�����
            services.AddControllers(c =>
            {
                c.Conventions.Add(new ApiExplorerGroupNameConvention());
                c.Conventions.Add(new ServiceTemplateConvention(thisService.Name));
            });

            // ����������
            services.AddHealthChecks();

            // ·��Сд
            services.AddRouting(c => c.LowercaseUrls = true);

            // api�汾����
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.Conventions.Add(new VersionByNamespaceConvention());
                o.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("x-api-version"));
            });
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
