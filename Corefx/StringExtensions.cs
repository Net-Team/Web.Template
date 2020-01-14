using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// String类扩展   
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 是否相等
        /// 不区分大小写
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string source, string value)
        {
            return string.Equals(source, value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args">参数</param>
        /// <exception cref="FormatException"></exception>
        /// <returns></returns>
        public static string Format(this string source, params object[] args)
        {
            return source == null ? null : string.Format(source, args);
        }

        /// <summary>
        /// 如果为null则返回String.Empty
        /// </summary>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static string NullThenEmpty(this string source)
        {
            return source ?? string.Empty;
        }

        /// <summary>
        /// 如果为null，则返回value的值
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string NullThen(this string source, string value)
        {
            return source.IsNullOrEmpty() ? value : source;
        }

        /// <summary>
        /// 判断字符串是否为null或Empty
        /// </summary>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// 转换为整数
        /// 失败返回默认值0
        /// </summary>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static int ToInt32(this string source)
        {
            int.TryParse(source, out int value);
            return value;
        }

        /// <summary>
        /// 转换为整数
        /// 失败则直接返回默认值default
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="defalut">默认值</param>
        /// <returns></returns>
        public static int ToInt32(this string source, int defalut)
        {
            return int.TryParse(source, out int value) ? value : defalut;
        }

        /// <summary>
        /// 是否匹配正则表达式参数
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="pattert">regex参数</param>
        /// <returns></returns>
        public static bool IsMatch(this string source, string pattert)
        {
            return Regex.IsMatch(source.NullThenEmpty(), pattert.NullThenEmpty());
        }

        /// <summary>
        /// 匹配正则表示式
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="pattert">regex参数</param>
        /// <returns></returns>
        public static string Match(this string source, string pattert)
        {
            return Regex.Match(source.NullThenEmpty(), pattert.NullThenEmpty()).Value;
        }

        /// <summary>
        /// 匹配正则表示式
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="pattert">regex参数</param>
        /// <returns></returns>
        public static string[] Matches(this string source, string pattert)
        {
            return Regex
                .Matches(source.NullThenEmpty(), pattert.NullThenEmpty())
                .Cast<Match>()
                .Select(item => item.Value)
                .ToArray();
        }

        /// <summary>
        /// 重复填充字符
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="totalLength">填充后的长度</param>
        /// <returns></returns>
        public static string Repeat(this char source, int totalLength)
        {
            return new string(source, totalLength);
        }

        /// <summary>
        /// 掩码字符串
        /// </summary>
        /// <param name="sourceValue">原始值</param>
        /// <param name="skipLeft">左边跳过的字数</param>
        /// <param name="skipRight">右边跳过的字数</param>
        /// <param name="mask">掩码字符</param>
        public static string MaskAs(this string sourceValue, int skipLeft, int skipRight, char mask = '*')
        {
            if (sourceValue?.Length > skipLeft + skipRight)
            {
                var span = sourceValue
                    .ToCharArray()
#if NETCOREAPP3_0
                    .AsSpan()
#endif
                    ;
                for (var i = skipLeft; i < span.Length - skipRight; i++)
                {
                    span[i] = mask;
                }
                return new string(span);
            }
            else
            {
                return sourceValue;
            }
        }

        /// <summary>
        /// 获取和目标字符串的相似度
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        public static decimal GetSimilarityWith(this string source, string target)
        {
            const decimal kq = 2m;
            const decimal kr = 1m;
            const decimal ks = 1m;

            var sourceArray = source.ToCharArray();
            var destArray = target.ToCharArray();

            //获取交集数量
            var q = sourceArray.Intersect(destArray).Count();
            var s = sourceArray.Length - q;
            var r = destArray.Length - q;
            return kq * q / (kq * q + kr * r + ks * s);
        }

        /// <summary>
        /// 骆驼命名
        /// </summary>
        /// <param name="source">名称</param>
        /// <returns></returns>
        public static string CamelCase(this string source)
        {
            if (string.IsNullOrEmpty(source) || char.IsUpper(source[0]) == false)
            {
                return source;
            }

            var charArray = source
                .ToCharArray()
#if NETCOREAPP3_0
                .AsSpan()
#endif
                ;
            for (int i = 0; i < charArray.Length; i++)
            {
                if (i == 1 && char.IsUpper(charArray[i]) == false)
                {
                    break;
                }

                var hasNext = (i + 1 < charArray.Length);
                if (i > 0 && hasNext && !char.IsUpper(charArray[i + 1]))
                {
                    if (char.IsSeparator(charArray[i + 1]))
                    {
                        charArray[i] = char.ToLowerInvariant(charArray[i]);
                    }
                    break;
                }
                charArray[i] = char.ToLowerInvariant(charArray[i]);
            }
            return new string(charArray);
        }
    }
}