using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Web.Core.Startups
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
        /// <returns></returns>
        public static IServiceCollection AddSwaggerJwtAuth(this IServiceCollection services)
        {
            return services.AddSwaggerJwtAuth(null);
        }

        /// <summary>
        /// 添加swagger的Bearer token
        /// </summary>
        /// <param name="services"></param>  
        /// <param name="description">说明</param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerJwtAuth(this IServiceCollection services, string description)
        {
            return services.PostConfigure<SwaggerGenOptions>(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = description ?? "请在下面输入token值",
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
