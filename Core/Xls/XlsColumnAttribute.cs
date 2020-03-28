using System;

namespace Core.Xls
{
    /// <summary>
    /// 表示xls的列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class XlsColumnAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置列表
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取或设置XlsColumnParser的类型
        /// </summary>
        public Type ParserType { get; set; }

        /// <summary>
        /// 获取或设置是否忽略这个列
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// xls的列
        /// </summary>
        public XlsColumnAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// xls的列
        /// </summary>
        /// <param name="name">命名</param>
        public XlsColumnAttribute(string name)
        {
            this.Name = name;
            this.ParserType = typeof(XlsColumnParser);
        }
    }
}
