using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// int类型扩展
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// 转换分页页码
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ToPageIndex(this int source)
        {
            if (source <= 0)
                return 0;
            return source - 1;
        }
    }
}
