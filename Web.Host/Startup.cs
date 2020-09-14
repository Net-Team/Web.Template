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
            foreach (var assembily in AssemblyLoadContext.Default.Assemblies)
            {
                if (assembily.FullName?.StartsWith("System") == false && assembily.FullName?.StartsWith("Microsoft") == false)
                {
                    services.AddDependencyServices(assembily);
                    services.AddHttpApis(assembily, Configuration.GetSection("HttpApi"));
                    services.AddConfigureOptions(assembily, this.Configuration.GetSection("Options"));
                }
            }

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
                .AddJwtParser()                  // �����֤����                
                .AddApiResultInvalidModelState() // ģ����֤ת��ΪApiResult���
                .AddNamespaceApiVersioning()     // ��Ӱ汾����
                .AddSwaggerJwtAuth()             // ���swagger��Bearer token
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
                c.ModelBinderProviders.Insert(0, new StringPropertyTrimModelBinderProvider(c, p => p.IsDefined(typeof(StringTrimFlagAttribute))));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(JsonStringToEnumConverter.Default);
                options.JsonSerializerOptions.Converters.Add(JsonStringToNumberConverter.Default);
                options.JsonSerializerOptions.Converters.Add(JsonLocalDateTimeConverter.Default);
            });

            // ����������
            services.AddHealthChecks();

            // ·��Сд
            services.AddRouting(c => c.LowercaseUrls = true);
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
