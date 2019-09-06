using System;

namespace Core.Xls
{
    /// <summary>
    /// 表示xls字段解析器
    /// </summary>
    public class XlsColumnParser
    {
        /// <summary>
        /// 解析字段
        /// </summary>
        /// <param name="value">从xls读出的原始值</param>
        /// <param name="targetType">目标属性类型</param>
        /// <returns></returns>
        public virtual object Parse(object value, Type targetType)
        {
            if (value == null || value == DBNull.Value)
            {
                return targetType.DefaultValue();
            }
            return Converter.ConvertToType(value, targetType);
        }
    }
}
