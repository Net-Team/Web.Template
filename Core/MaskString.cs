﻿namespace Core
{
    /// <summary>
    /// 表示掩码字符串
    /// </summary>
    public class MaskString
    {
        /// <summary>
        /// 获取掩码字符串
        /// </summary>
        public string MaskValue { get; }

        /// <summary>
        /// 获取原始字符串
        /// </summary>
        public string SourceValue { get; }

        /// <summary>
        /// 掩码字符串
        /// </summary>
        /// <param name="sourceValue">原始值</param>
        /// <param name="skipLeft">左边跳过的字数</param>
        /// <param name="skipRight">右边跳过的字数</param>
        /// <param name="mask">掩码字符</param>
        public MaskString(string sourceValue, int skipLeft, int skipRight, char mask = '*')
        {
            this.SourceValue = sourceValue;
            if (sourceValue?.Length > skipLeft + skipRight)
            {
                var left = sourceValue.Substring(0, skipLeft);
                var right = sourceValue.Substring(sourceValue.Length - skipRight);
                var maskValue = new string(mask, sourceValue.Length - skipLeft - skipRight);
                this.MaskValue = $"{left}{maskValue}{right}";
            }
            else
            {
                this.MaskValue = sourceValue;
            }
        }
    }
}
