using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Core.Web.Startups
{
    /// <summary>
    /// 表示swagger的值类型自动Required标记过滤器
    /// </summary>
    public class SwaggerRequiredSchemaFilter : ISchemaFilter
    {
        private readonly CamelCasePropertyNamesContractResolver camelCaseContractResolver;

        /// <summary>
        /// swagger的值类型自动Required标记过滤器
        /// </summary>
        /// <param name="camelCasePropertyNames"></param>
        public SwaggerRequiredSchemaFilter(bool camelCasePropertyNames)
        {
            this.camelCaseContractResolver = camelCasePropertyNames ? new CamelCasePropertyNamesContractResolver() : null;
        }

        /// <summary>
        /// 获取属性名
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns></returns>
        private string GetPropertyName(PropertyInfo property)
        {
            return camelCaseContractResolver?.GetResolvedPropertyName(property.Name) ?? property.Name;
        }

        /// <summary>
        /// 应用过滤器
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            foreach (var property in context.Type.GetProperties())
            {
                var schemaPropertyName = this.GetPropertyName(property);
                if (schema.Properties?.ContainsKey(schemaPropertyName) == true)
                {
                    var propertyType = property.PropertyType;
                    if (propertyType.IsValueType == true)
                    {
                        var isNullable = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
                        if (isNullable == false && property.IsDefined(typeof(RequiredAttribute)) == false)
                        {
                            if (schema.Required == null)
                            {
                                schema.Required = new SortedSet<string>();
                            }
                            schema.Required.Add(schemaPropertyName);
                        }
                    }
                }
            }
        }
    }
}
