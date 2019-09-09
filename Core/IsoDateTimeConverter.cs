using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core
{
    /// <summary>
    /// ISO时间格式转换器
    /// </summary>
    public class IsoDateTimeConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// 读取DateTime
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDateTime();
        }

        /// <summary>
        /// 写入DateTime
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var dateTimeString = value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffZ");
            writer.WriteStringValue(dateTimeString);
        }
    }
}
