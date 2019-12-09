using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Web.JsonPatchs
{
    /// <summary>
    /// 表示操作选项
    /// </summary>
    public class JsonPatchOperation
    {
        /// <summary>
        /// replace
        /// </summary>      
        [JsonPropertyName("op")]
        public string Op { get; set; }

        /// <summary>
        /// /{propertyName}
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>        
        [JsonPropertyName("value")]
        public JsonElement Value { get; set; }

        /// <summary>
        /// 是否为Replace的op
        /// </summary>
        /// <returns></returns>
        public bool IsReplace()
        {
            return string.Equals(this.Op, "replace");
        }

        /// <summary>
        /// 应用选项到目标
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">目标</param>
        /// <exception cref="NotSupportedException"></exception>
        public void ApplyTo<T>(T target)
        {
            if (this.IsReplace() == false)
            {
                throw new NotSupportedException($"not supported op: {this.Op}");
            }

            if (target == null)
            {
                return;
            }

            var name = this.Path.NullThenEmpty().TrimStart('/');
            var property = Property<T>.GetProperty(name);
            if (property != null)
            {
                var value = this.GetValue(property.Info.PropertyType);
                property.SetValue(target, value);
            }
        }

        /// <summary>
        /// 获取值 
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private object GetValue(Type type)
        {
            switch (this.Value.ValueKind)
            {
                case JsonValueKind.False:
                    return false;

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.Null:
                    return null;

                case JsonValueKind.Number:
                    var number = this.Value.GetRawText();
                    return Converter.ConvertToType(number, type);

                case JsonValueKind.String:
                    var str = this.Value.GetString();
                    return Converter.ConvertToType(str, type);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}