using Application;
using Core;
using Domain;
using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Web.Core.Configs;
using Web.Core.FilterAttributes;
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
        }


        /// <summary>
        /// ��ӷ���
        /// </summary>
        /// <param name="services"></param>  
        public void ConfigureServices(IServiceCollection services)
        {
            // ���ð�
            services.Configure<KongInfo>(Configuration.GetSection(nameof(KongInfo)));
            services.Configure<ServiceInfo>(Configuration.GetSection(nameof(ServiceInfo)));

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

            // ���swagger�ĵ�
            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<RequiredSchemaFilter>(true);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Core.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Domain.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Application.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Web.Core.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Web.Host.xml"));
            });

            // ��ӿ�����
            var mvcBuilder = services
                .AddControllers(c =>
                {
                    c.Filters.Add<ApiGlobalExceptionFilter>();
                }).ConfigureApiBehaviorOptions(c =>
                {
                    c.InvalidModelStateResponseFactory = context =>
                    {
                        var keyValue = context.ModelState.FirstOrDefault(item => item.Value.Errors.Count > 0);
                        var message = $"����{keyValue.Key}��֤ʧ�ܣ�{keyValue.Value.Errors[0].ErrorMessage}";

                        var apiResult = ApiResult.ParameterError<object>(message);
                        return new ObjectResult(apiResult);
                    };
                });

            if (Environment.IsDevelopment())
            {
                mvcBuilder.AddNewtonsoftJson(o => o.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented);
            }

            // ����������
            services.AddHealthChecks();
            services.AddExceptionLess(this.Configuration.GetSection("ExceptionLess"));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseJwtParser();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "swagger doc");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(Configuration.GetValue<string>($"{nameof(ServiceInfo)}:{nameof(ServiceInfo.HealthRoute)}"));
            });
        }
    }
}
