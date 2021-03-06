﻿using Core.Web.Conventions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace Core.Web.Swaggers
{
    /// <summary>
    /// 为swagger的api增加x-api-version请求头
    /// </summary>
    public class SwaggerApiVersionHeaderFilter : IOperationFilter
    {
        /// <summary>
        /// 获取请求头名 
        /// </summary>
        public string HeaderName { get; }

        /// <summary>
        /// swagger的api增加x-api-version请求头
        /// </summary>
        /// <param name="headerName"></param>
        public SwaggerApiVersionHeaderFilter(string headerName = "x-api-version")
        {
            this.HeaderName = headerName ?? throw new ArgumentNullException(nameof(headerName));
        }

        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            string GetVersionName()
                => ApiExplorerGroupNameConvention.GetApiVersion(context.ApiDescription.GroupName).ToString();

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            var apiversion = new OpenApiParameter
            {
                In = ParameterLocation.Header,
                Required = false,
                Name = this.HeaderName,
                Style = ParameterStyle.Simple,
                Description = "api版本，不填则对应1.0",
                Schema = new OpenApiSchema { Type = "string", Default = new OpenApiString(GetVersionName()) }
            };
            operation.Parameters.Add(apiversion);
        }
    }
}
