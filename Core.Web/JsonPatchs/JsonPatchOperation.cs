using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;

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
        public string op { get; set; }

        /// <summary>
        /// /{propertyName}
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>        
        public JsonElement value { get; set; }

        /// <summary>
        /// 是否为Replace的op
        /// </summary>
        /// <returns></returns>
        public bool IsReplace()
        {
            return string.Equals(this.op, "replace");
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
                throw new NotSupportedException($"not supported op: {this.op}");
            }

            if (target == null)
            {
                return;
            }

            var name = this.path.NullThenEmpty().TrimStart('/');
            var property = Property<T>.Properties.FirstOrDefault(item => item.Name.EqualsIgnoreCase(name));
            if (property != null)
            {
                var objValue = this.GetValue(property.Info.PropertyType);
                property.SetValue(target, objValue);
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
            switch (this.value.ValueKind)
            {
                case JsonValueKind.False:
                    return false;

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.Null:
                    return null;

                case JsonValueKind.Number:
                    var number = this.value.GetRawText();
                    return Converter.ConvertToType(number, type);

                case JsonValueKind.String:
                    var str = this.value.GetString();
                    return Converter.ConvertToType(str, type);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}