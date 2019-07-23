using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace Web.Host.Startups
{
    /// <summary>
    /// swagger的授权信息扩展
    /// </summary>
    public static class SwaggerJwtAuthExtenstions
    {
        /// <summary>
        /// 添加swagger的Bearer token
        /// </summary>
        /// <param name="services"></param>    
        public static void AddSwaggerJwtAuth(this IServiceCollection services)
        {
            services.Configure<SwaggerGenOptions>(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请在下面输入token值",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}
