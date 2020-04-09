using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Web
{
    /// <summary>
    /// Api版本控制扩展
    /// </summary>
    public static class ApiVersioningExtensions
    {
        /// <summary>
        /// 添加以namespace来查找Api版本信息 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="apiVersionHeaderName">api版本请求头名称</param>
        /// <returns></returns>
        public static IServiceCollection AddNamespaceApiVersioning(this IServiceCollection services, string apiVersionHeaderName = "x-api-version")
        {
            return services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.Conventions.Add(new VersionByNamespaceConvention());
                o.ApiVersionReader = new HeaderApiVersionReader(apiVersionHeaderName);
            });
        }
    }
}
