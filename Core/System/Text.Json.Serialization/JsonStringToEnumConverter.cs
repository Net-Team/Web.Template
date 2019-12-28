using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// string转enum的Json转换器
    /// </summary>
    public class JsonStringToEnumConverter : JsonConverterFactory
    {
        /// <summary>
        /// 获取默认实例
        /// </summary>
        public static JsonStringToEnumConverter Default { get; } = new JsonStringToEnumConverter();

        /// <summary>
        /// 是否支持转换
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        /// <summary>
        /// 创建转换器
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Lambda.CreateCtorFunc<JsonConverter>(typeof(JsonEnumConverter<>).MakeGenericType(typeToConvert))();
        }

        /// <summary>
        /// 枚举转换器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class JsonEnumConverter<T> : JsonConverter<T> where T : struct, Enum
        {
            private static readonly TypeCode typeCode = Type.GetTypeCode(typeof(T));

            /// <summary>
            /// 是否支持转换
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public override bool CanConvert(Type type)
            {
                return type.IsEnum;
            }

            /// <summary>
            /// 读取枚举
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="typeToConvert"></param>
            /// <param name="options"></param>
            /// <returns></returns>
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var token = reader.TokenType;
                if (token == JsonTokenType.String)
                {
                    var enumString = reader.GetString();
                    return Enum.Parse<T>(enumString, ignoreCase: true);
                }

                if (token == JsonTokenType.Number)
                {
                    switch (typeCode)
                    {
                        case TypeCode.Int32:
                            if (reader.TryGetInt32(out int int32))
                            {
                                return Unsafe.As<int, T>(ref int32);
                            }
                            break;
                        case TypeCode.UInt32:
                            if (reader.TryGetUInt32(out uint uint32))
                            {
                                return Unsafe.As<uint, T>(ref uint32);
                            }
                            break;
                        case TypeCode.UInt64:
                            if (reader.TryGetUInt64(out ulong uint64))
                            {
                                return Unsafe.As<ulong, T>(ref uint64);
                            }
                            break;
                        case TypeCode.Int64:
                            if (reader.TryGetInt64(out long int64))
                            {
                                return Unsafe.As<long, T>(ref int64);
                            }
                            break;

                        case TypeCode.SByte:
                            if (reader.TryGetSByte(out sbyte sbyteValue))
                            {
                                return Unsafe.As<sbyte, T>(ref sbyteValue);
                            }
                            break;
                        case TypeCode.Byte:
                            if (reader.TryGetByte(out byte byteValue))
                            {
                                return Unsafe.As<byte, T>(ref byteValue);
                            }
                            break;
                        case TypeCode.Int16:
                            if (reader.TryGetInt16(out short int16))
                            {
                                return Unsafe.As<short, T>(ref int16);
                            }
                            break;
                        case TypeCode.UInt16:
                            if (reader.TryGetUInt16(out ushort uint16))
                            {
                                return Unsafe.As<ushort, T>(ref uint16);
                            }
                            break;
                    }
                }

                throw new NotSupportedException($"无法将{reader.TokenType}转换为{typeToConvert}");
            }

            /// <summary>
            /// 写入
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="value"></param>
            /// <param name="options"></param>
            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                switch (typeCode)
                {
                    case TypeCode.Int32:
                        writer.WriteNumberValue(Unsafe.As<T, int>(ref value));
                        break;
                    case TypeCode.UInt32:
                        writer.WriteNumberValue(Unsafe.As<T, uint>(ref value));
                        break;
                    case TypeCode.UInt64:
                        writer.WriteNumberValue(Unsafe.As<T, ulong>(ref value));
                        break;
                    case TypeCode.Int64:
                        writer.WriteNumberValue(Unsafe.As<T, long>(ref value));
                        break;
                    case TypeCode.Int16:
                        writer.WriteNumberValue(Unsafe.As<T, short>(ref value));
                        break;
                    case TypeCode.UInt16:
                        writer.WriteNumberValue(Unsafe.As<T, ushort>(ref value));
                        break;
                    case TypeCode.Byte:
                        writer.WriteNumberValue(Unsafe.As<T, byte>(ref value));
                        break;
                    case TypeCode.SByte:
                        writer.WriteNumberValue(Unsafe.As<T, sbyte>(ref value));
                        break;
                    default:
                        throw new NotSupportedException($"不支持非枚举类型{typeof(T)}");
                }
            }
        }
    }
}
