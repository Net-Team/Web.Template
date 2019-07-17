using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// 时间扩展
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 根据日期计算日期周数（以周日为一周的第一天）
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <returns>日期周数</returns>
        public static int WeekOfYear(this DateTime dateTime)
        {
            int day = DateTime.Parse(string.Format("{0}-1-1 0:0:0", dateTime.Year)).DayOfWeek.GetHashCode() - 1;
            int week = (dateTime.DayOfYear + day) / 7 + 1;
            return week;
        }

        /// <summary>
        /// 日期时间的所在周的第一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfWeek(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(0 - dateTime.DayOfWeek.GetHashCode());
        }

        /// <summary>
        /// 日期时间的所在周的最后一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(6 - dateTime.DayOfWeek.GetHashCode());
        }

        /// <summary>
        /// 转换为时间昵称
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToNickString(this DateTime dateTime)
        {
            if (dateTime.Date == DateTime.Now.Date)
            {
                var span = DateTime.Now.Subtract(dateTime);
                if (span.TotalMinutes < 30)
                {
                    return Math.Ceiling(span.TotalMinutes) + " 分钟前";
                }
                if (span.TotalMinutes < 60)
                {
                    return "半小时前";
                }

                return dateTime.ToString("HH:mm");
            }

            if (dateTime.AddDays(1).Date == DateTime.Now.Date)
            {
                return "昨天 " + dateTime.ToString("HH:mm");
            }
            if (dateTime.AddDays(2).Date == DateTime.Now.Date)
            {
                return "前天 " + dateTime.ToString("HH:mm");
            }

            if (dateTime.Year == DateTime.Now.Year)
            {
                return dateTime.ToString("MM月dd日");
            }
            return dateTime.ToString("yyyy年MM月dd日");
        }

        /// <summary>
        /// 转为日期格式 yyyy-MM-dd
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <returns></returns>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 转为日期格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取当前月往前连接的几个月
        /// </summary>
        /// <param name="m">共几个月</param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetMonths(this DateTime dt, int m)
        {
            var current = dt.AddDays(1 - dt.Day);
            for (var i = 0; i < m; i++)
            {
                yield return current.AddMonths(-i);
            }
        }

        /// <summary>
        /// 获取本月的经过的天
        /// </summary>
        /// <param name="td"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetDaysOfMonth(this DateTime dt)
        {
            var min = dt.Date.AddDays(1 - dt.Day);
            var max = min.AddMonths(1);
            if (max > DateTime.Today)
            {
                max = DateTime.Today;
            }
            var days = max.Subtract(min).Days;
            for (var i = 0; i <= days; i++)
            {
                yield return min.AddDays(i);
            }
        }

        /// <summary>  
        /// 获取时间毫秒戳 Timestamp
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static long ToTimeStamp(this DateTime dt)
        {
            return new DateTimeOffset(dt).ToUniversalTime().ToUnixTimeMilliseconds();
        }

        /// <summary>  
        /// 获取时间秒戳 Timestamp
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static long ToTimeStampBySeconds(this DateTime dt)
        {
            return new DateTimeOffset(dt).ToUniversalTime().ToUnixTimeSeconds();
        }
    }
}
