using System.Collections.Generic;

namespace System.Data
{
    /// <summary>
    /// csv扩展
    /// </summary>
    public static class CsvExtensions
    {
        /// <summary>
        /// 转换为Csv
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        public static Csv<T> ToCsv<T>(this IEnumerable<T> models)
        {
            return new Csv<T>(models);
        }
    }
}
