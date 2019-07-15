using System.Linq;

namespace System
{
    /// <summary>
    /// 时间戳扩展
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// 转换为中文友好名称
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string ToCNString(this TimeSpan timeSpan)
        {
            var items = new[] {
               new{ value= timeSpan.Days, format="天" },
               new{ value= timeSpan.Hours, format="小时"},
               new{ value= timeSpan.Minutes, format="分"},
               new{ value= timeSpan.Milliseconds, format="秒"}
            };

            var values = items.Where(item => item.value > 0).Select(item => item.value.ToString() + item.format);
            return string.Join(string.Empty, values);
        }
    }
}
