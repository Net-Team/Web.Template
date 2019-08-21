﻿using System.Linq;
using System.Text;
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
        /// 如果为null则返回String.Empty
        /// </summary>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static string NullToEmpty(this string source)
        {
            return source.NullThenEmpty();
        }

        /// <summary>
        /// 如果为null则返回String.Empty
        /// </summary>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static string NullThenEmpty(this string source)
        {
            return source == null ? string.Empty : source;
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
        /// 当值与when条件相同则返回then值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="when"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        public static string WhenThen(this string source, string when, string then)
        {
            return source == when ? then : source;
        }

        /// <summary>
        /// 转换为小写
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToLowerIfNoNull(this string source)
        {
            return string.IsNullOrEmpty(source) ? source : source.ToLower();
        }

        /// <summary>
        /// 转换为大写
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUpperIfNoNull(this string source)
        {
            return string.IsNullOrEmpty(source) ? source : source.ToUpper();
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
            source = source.NullThenEmpty();
            pattert = pattert.NullThenEmpty();
            return Regex.IsMatch(source, pattert);
        }

        /// <summary>
        /// 匹配正则表示式
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="pattert">regex参数</param>
        /// <returns></returns>
        public static string Match(this string source, string pattert)
        {
            source = source.NullThenEmpty();
            pattert = pattert.NullThenEmpty();
            return Regex.Match(source, pattert).Value;
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
        /// 以GB2312编码获取字符串的长度
        /// 一个中文占两个长度 英文一个长度
        /// </summary>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static int LengthGB2312(this string source)
        {
            return Encoding.GetEncoding("gb2312").GetByteCount(source.NullThenEmpty());
        }

        /// <summary>
        /// 以GB2312编码切割字符串
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="lengthGB2312">保留的长度</param>
        /// <returns></returns>
        public static string SubstringGB2312(this string source, int lengthGB2312)
        {
            var encoding = Encoding.GetEncoding("gb2312");
            source = source.NullThenEmpty();

            if (lengthGB2312 >= source.LengthGB2312())
            {
                return source;
            }

            int index = 0;
            int length = 0;
            var chars = source.ToCharArray();

            while (length < lengthGB2312)
            {
                length += encoding.GetByteCount(chars, index, 1);
                if (length == lengthGB2312)
                {
                    return new string(chars, 0, index + 1);
                }
                else if (length > lengthGB2312)
                {
                    return new string(chars, 0, index);
                }
                index++;
            }
            return string.Empty;
        }

        /// <summary>
        /// 以GB2312编码切割字符串
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="lengthGB2312">保留的长度</param>
        /// <param name="padChar">填充字符</param>
        /// <returns></returns>
        public static string SubstringGB2312(this string source, int lengthGB2312, char padChar)
        {
            source = source.NullThenEmpty().SubstringGB2312(lengthGB2312);
            int count = lengthGB2312 - source.LengthGB2312();
            if (count == 0)
            {
                return source;
            }
            return string.Concat(source, padChar.Repeat(count));
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
                var span = sourceValue.ToCharArray();
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
        /// 省略
        /// </summary>
        /// <param name="source"></param>
        /// <param name="lengthGB2312"></param>
        /// <returns></returns>
        public static string EllipsisGB2312(this string source, int lengthGB2312)
        {
            if (source.LengthGB2312() > lengthGB2312)
            {
                return source.SubstringGB2312(lengthGB2312) + " ..";
            }
            return source;
        }

        /// <summary>
        /// 获取和目标字符串的相似度
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        public static decimal GetSimilarityWith(this string source, string target)
        {
            var Kq = 2m;
            var Kr = 1m;
            var Ks = 1m;

            var sourceArray = source.ToCharArray();
            var destArray = target.ToCharArray();

            //获取交集数量
            var q = sourceArray.Intersect(destArray).Count();
            var s = sourceArray.Length - q;
            var r = destArray.Length - q;
            return Kq * q / (Kq * q + Kr * r + Ks * s);
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

            var charArray = source.ToCharArray();
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