/*====================================================================
 *-- QQ:1072892917
 *====================================================================
 * 文件名称：DateTimeHelper.cs
 * 项目名称：常用方法实用工具集
 * 创建时间：2016年10月11日10时59分
 * 创建人员：赵杰迪
 * 负 责 人：赵杰迪
 ===================================================================
*/

using System;
using System.Globalization;
using System.Linq;

namespace RemoteSenseCommLib.Tools
{
    public static class DateTimeHelper
    {
        #region 基础的私有方法
        /// <summary>
        /// 是否是有效的年
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static bool IsValidYear(int year)
        {
            return year >= 0 && year <= 9999;
        }
        /// <summary>
        /// 是否是有效的季度
        /// </summary>
        /// <param name="quarter"></param>
        /// <returns></returns>
        private static bool IsValidQuarter(int quarter)
        {
            return quarter >= 1 && quarter <= 4;
        }
        /// <summary>
        /// 是否是有效的月
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        private static bool IsValidMonth(int month)
        {
            return month >= 1 && month <= 12;
        }
        /// <summary>
        /// 是否是有效的旬
        /// </summary>
        /// <param name="ten"></param>
        /// <returns></returns>
        private static bool IsValidTen(int ten)
        {
            return ten >= 1 && ten <= 3;
        }
        #endregion
        #region 年级别的函数

        /// <summary>
        /// 获取年的开始时间
        /// </summary>
        /// <param name="year">年</param>
        /// <exception cref="ArgumentOutOfRangeException">year无效值</exception>
        /// <returns>日期</returns>
        public static DateTime GetStartOfYear(int year)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            return new DateTime(year, 1, 1);
        }
        /// <summary>
        /// 获取年的开始时间
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfYear(DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }

        /// <summary>
        ///  获取年的结束时间
        /// </summary>
        /// <param name="year">年</param>
        /// <exception cref="ArgumentOutOfRangeException">year无效</exception>
        /// <returns>日期</returns>
        public static DateTime GetEndOfYear(int year)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-4, and your transfer value is" + year);
            }
            return new DateTime(year, 12, DateTime.DaysInMonth(year, 12), 23, 59, 59, 999);
        }
        /// <summary>
        /// 获取年的结束时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetEndOfYear(DateTime dt)
        {
            return new DateTime(dt.Year, 12, DateTime.DaysInMonth(dt.Year, 12), 23, 59, 59, 999);
        }

        /// <summary>
        /// 获取当前年的开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfCurrentYear()
        {
            return GetStartOfYear(DateTime.Now);
        }
        /// <summary>
        /// 获取当前年的结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfCurrentYear()
        {
            return GetEndOfYear(DateTime.Now);
        }
        /// <summary>
        /// 判断年是否是闰年
        /// </summary>
        /// <param name="year">年</param>
        /// <returns>布尔值</returns>
        public static bool IsLeapOfYear(int year)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            return DateTime.IsLeapYear(year);
        }
        /// <summary>
        /// 判断年是否是闰年
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>布尔值</returns>
        public static bool IsLeapOfYear(DateTime dt)
        {
            return DateTime.IsLeapYear(dt.Year);
        }
        /// <summary>
        /// 判断当前年是否是闰年
        /// </summary>
        /// <returns>布尔值</returns>
        public static bool IsLeapOfCurrentYear()
        {
            return IsLeapOfYear(DateTime.Now);
        }
        /// <summary>
        /// 获取某年的所有天数
        /// </summary>
        /// <param name="year">年</param>
        /// <returns>天数</returns>
        public static int GetTotalDaysOfYear(int year)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            return IsLeapOfYear(new DateTime(year, 1, 1)) ? 366 : 365;
        }
        /// <summary>
        /// 获取某年的所有天数
        /// </summary>
        /// <param name="dt">年</param>
        /// <returns>天数</returns>
        public static int GetTotalDaysOfYear(DateTime dt)
        {
            return IsLeapOfYear(dt) ? 366 : 365;
        }
        /// <summary>
        /// 获取某年的所有天数
        /// </summary>
        /// <returns>天数</returns>
        public static int GetTotalDaysOfCurrentYear()
        {
            return GetTotalDaysOfYear(DateTime.Now);
        }
        #endregion
        #region 季度级别的函数
        /// <summary>
        /// 获取指定年指定季度的开始时间
        /// </summary>
        /// <param name="year">年有效值（0-9999）</param>
        /// <param name="quarter">季度（有效值1,2,3,4）</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfQuarter(int year, int quarter)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            if (!IsValidQuarter(quarter))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-4, and your transfer value is" + quarter);
            }
            switch (quarter)
            {
                case 1:
                    return new DateTime(year, 1, 1, 0, 0, 0, 0);
                case 2:
                    return new DateTime(year, 4, 1, 0, 0, 0, 0);
                case 3:
                    return new DateTime(year, 7, 1, 0, 0, 0, 0);
                case 4:
                    return new DateTime(year, 10, 1, 0, 0, 0, 0);
                default:
                    throw new ArgumentOutOfRangeException("Valid values are 1-4, and your transfer value is" + quarter);
            }
        }

        /// <summary>
        /// 获取指定年指定季度的结束时间
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="quarter"></param>
        /// <returns>可空的时间</returns>
        public static DateTime GetEndOfQuarter(int year, int quarter)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            if (!IsValidQuarter(quarter))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-4, and your transfer value is" + quarter);
            }
            switch (quarter)
            {
                case 1:
                    return new DateTime(year, 3,
                        DateTime.DaysInMonth(year, 3), 23, 59, 59, 999);
                case 2:
                    return new DateTime(year, 6,
                        DateTime.DaysInMonth(year, 6), 23, 59, 59, 999);
                case 3:
                    return new DateTime(year, 9,
                        DateTime.DaysInMonth(year, 9), 23, 59, 59, 999);
                case 4: return new DateTime(year, 12,
                    DateTime.DaysInMonth(year, 12), 23, 59, 59, 999);
                default:
                    throw new ArgumentOutOfRangeException("Valid values are 1-4, and your transfer value is" + quarter);
            }
        }

        /// <summary>
        /// 获取月份对应的季度
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns>季度</returns>
        public static int GetQuarter(int month)
        {
            if (!IsValidMonth(month))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-12, and your transfer value is" + month);
            }
            return (month - 1) / 3 + 1;
        }
        public static int GetQuarter(DateTime dt)
        {
            return (dt.Month - 1) / 3 + 1;
        }
        /// <summary>
        /// 获取当前季度的开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfCurrentQuarter()
        {
            return GetStartOfQuarter(DateTime.Now.Year, DateTime.Now.Month);
        }
        /// <summary>
        /// 获取当前季度的开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfCurrentQuarter()
        {
            return GetEndOfQuarter(DateTime.Now.Year, DateTime.Now.Month);
        }
        #endregion
        #region 月级别的函数

        /// <summary>
        /// 获取某年某月的开始日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <exception cref="ArgumentOutOfRangeException">输入无效</exception>
        /// <returns>日期</returns>
        public static DateTime GetStartOfMonth(int year, int month)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            if (!IsValidMonth(month))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-12, and your transfer value is" + month);
            }
            return new DateTime(year, month, 1, 0, 0, 0, 0);
        }
        /// <summary>
        /// 根据指定日期所在的月份的第一天日期
        /// </summary>
        /// <param name="dt">指定的日期</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfMonth(DateTime dt)
        {
            return GetStartOfMonth(dt.Year, dt.Month);
        }

        /// <summary>
        /// 获取某年某月的结束日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <exception cref="ArgumentOutOfRangeException">输入无效</exception>
        /// <returns>日期</returns>
        public static DateTime GetEndOfMonth(int year, int month)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            if (!IsValidMonth(month))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-12, and your transfer value is" + month);
            }
            return new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59, 999);
        }
        /// <summary>
        /// 根据指定日期所在的月份的最后一天日期
        /// </summary>
        /// <param name="dt">指定的日期</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfMonth(DateTime dt)
        {
            return GetEndOfMonth(dt.Year, dt.Month);
        }
        /// <summary>
        /// 获取当前日期所在的月份的第一天日期
        /// </summary>
        /// <returns>日期</returns>
        public static DateTime GetStartOfCurrentMonth()
        {
            return GetStartOfMonth(DateTime.Now);
        }
        /// <summary>
        /// 获取当前日期所在的月份的最后一天日期
        /// </summary>
        /// <returns>日期</returns>
        public static DateTime GetEndOfCurrentMonth()
        {
            return GetEndOfMonth(DateTime.Now);
        }
        /// <summary>
        /// 获取给定年月的月份总天数
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns>天数</returns>
        public static int GetTotalDayOfMonth(int year, int month)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            if (!IsValidMonth(month))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-12, and your transfer value is" + month);
            }
            return DateTime.DaysInMonth(year, month);
        }
        /// <summary>
        /// 获取给定日期的所在月的总天数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>天数</returns>
        public static int GetTotalDayOfMonth(DateTime dt)
        {
            return DateTime.DaysInMonth(dt.Year, dt.Month);
        }
        #endregion
        #region 旬级别的函数
        /// <summary>
        /// 获取指定年，指定月的上旬的开始日期
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfTenFirst(DateTime dt)
        {
            return GetStartOfTen(dt.Year, dt.Month, 1);
        }
        /// <summary>
        /// 获取指定年，指定月的中旬的结束日期
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfTenSecond(DateTime dt)
        {
            return GetStartOfTen(dt.Year, dt.Month, 2);
        }
        /// <summary>
        /// 获取指定年，指定月的下旬的开始日期
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfTenThird(DateTime dt)
        {
            return GetStartOfTen(dt.Year, dt.Month, 3);
        }
        /// <summary>
        /// 获取指定年，指定月的上旬的结束日期
        /// </summary>
        /// <param name="dt">年</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfTenFirst(DateTime dt)
        {
            return GetEndOfTen(dt.Year, dt.Month, 1);
        }
        /// <summary>
        /// 获取指定年，指定月的中旬的结束日期
        /// </summary>
        /// <param name="dt">年</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfTenSecond(DateTime dt)
        {
            return GetEndOfTen(dt.Year, dt.Month, 2);
        }
        /// <summary>
        /// 获取指定年，指定月的下旬的结束日期
        /// </summary>
        /// <param name="dt">年</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfTenThird(DateTime dt)
        {
            return GetEndOfTen(dt.Year, dt.Month, 3);
        }
        /// <summary>
        /// 获取指定年，指定月的上旬的开始日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfTenFirst(int year, int month)
        {
            return GetStartOfTen(year, month, 1);
        }
        /// <summary>
        /// 获取指定年，指定月的中旬的结束日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfTenSecond(int year, int month)
        {
            return GetStartOfTen(year, month, 2);
        }
        /// <summary>
        /// 获取指定年，指定月的下旬的开始日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfTenThird(int year, int month)
        {
            return GetStartOfTen(year, month, 3);
        }
        /// <summary>
        /// 获取指定年，指定月的上旬的结束日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfTenFirst(int year, int month)
        {
            return GetEndOfTen(year, month, 1);
        }
        /// <summary>
        /// 获取指定年，指定月的中旬的结束日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfTenSecond(int year, int month)
        {
            return GetEndOfTen(year, month, 2);
        }
        /// <summary>
        /// 获取指定年，指定月的下旬的结束日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfTenThird(int year, int month)
        {
            return GetEndOfTen(year, month, 3);
        }
        public static DateTime GetEndOfCurrentTen()
        {
            DateTime dt = DateTime.Now.Date;
            int ten = GetTen(dt);
            return GetEndOfTen(dt.Year, dt.Month, ten);
        }
        public static DateTime GetStartOfCurrentTen()
        {
            DateTime dt = DateTime.Now.Date;
            int ten = GetTen(dt);
            return GetStartOfTen(dt.Year, dt.Month, ten);
        }
        /// <summary>
        /// 获取指定年，指定月， 指定旬的开始日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="ten">日</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfTen(int year, int month, int ten)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            if (!IsValidMonth(month))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-12, and your transfer value is" + month);
            }
            if (!IsValidTen(ten))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-3, and your transfer value is" + ten);
            }
            if (ten == 1)
            {
                return new DateTime(year, month, 1, 0, 0, 0, 0);
            }
            else if (ten == 2)
            {
                return new DateTime(year, month, 11, 0, 0, 0, 0);
            }
            else
            {
                return new DateTime(year, month, 21, 0, 0, 0, 0);
            }
        }
        /// <summary>
        /// 获取指定年，指定月， 指定旬的结束日期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="ten">旬</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfTen(int year, int month, int ten)
        {
            if (!IsValidYear(year))
            {
                throw new ArgumentOutOfRangeException("Valid values are 0-9999, and your transfer value is" + year);
            }
            if (!IsValidMonth(month))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-12, and your transfer value is" + month);
            }
            if (!IsValidTen(ten))
            {
                throw new ArgumentOutOfRangeException("Valid values are 1-3, and your transfer value is" + ten);
            }
            if (ten == 1)
            {
                return new DateTime(year, month, 10, 23, 59, 59, 999);
            }
            else if (ten == 2)
            {
                return new DateTime(year, month, 20, 23, 59, 59, 999);
            }
            else
            {
                return new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59, 999);
            }
        }
        /// <summary>
        /// 获取指定日期所在旬的开始日期
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfTen(DateTime dt)
        {
            return GetStartOfTen(dt.Year, dt.Month, GetTen(dt));
        }
        /// <summary>
        /// 获取指定日期所在旬的结束日期
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfTen(DateTime dt)
        {
            return GetEndOfTen(dt.Year, dt.Month, GetTen(dt));
        }
        /// <summary>
        /// 获取指定日期所在的旬
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>旬</returns>
        public static int GetTen(DateTime dt)
        {
            return GetTen(dt.Day);
        }
        /// <summary>
        /// 获取某月第几天所在的旬
        /// </summary>
        /// <param name="day">某个日期所在月的第几天</param>
        /// <returns>旬</returns>
        public static int GetTen(int day)
        {
            int ten = (day - 1) / 10 + 1;
            return ten > 3 ? 3 : ten;
        }
        #endregion
        #region 周级别的函数
        public static DateTime GetStartOfWeek(DateTime dt)
        {
            int daysToSubtract = (int)dt.DayOfWeek;
            DateTime resultDateTime = DateTime.Now.Subtract(System.TimeSpan.FromDays(daysToSubtract));
            return resultDateTime.Date;
        }
        public static DateTime GetEndOfWeek(DateTime dt)
        {
            DateTime resultDateTime = GetStartOfWeek(dt).AddDays(6);
            return new DateTime(resultDateTime.Year, resultDateTime.Month, resultDateTime.Day, 23, 59, 59, 999);
        }
        /// <summary>
        /// 获取当前周的开始日期
        /// </summary>
        /// <returns>日期</returns>
        public static DateTime GetStartOfCurrentWeek()
        {
            return GetStartOfWeek(DateTime.Now);
        }
        /// <summary>
        /// 获取当前周的结束日期
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfCurrentWeek()
        {
            return GetEndOfWeek(DateTime.Now);
        }
        #endregion
        #region 日级别的函数

        /// <summary>
        /// 获取指定日期的开始时刻
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>日期</returns>
        public static DateTime GetStartOfDay(DateTime dt)
        {
            return dt.Date;
        }
        /// <summary>
        /// 获取指定日期的结束时刻
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>日期</returns>
        public static DateTime GetEndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }
        /// <summary>
        /// 获取当前日期的开始时刻
        /// </summary>
        /// <returns>日期</returns>
        public static DateTime GetStartOfCurrentDay()
        {
            return GetStartOfDay(DateTime.Now);
        }
        /// <summary>
        /// 获取当前日期的结束时刻
        /// </summary>
        /// <returns>日期</returns>
        public static DateTime GetEndOfCurrentDay()
        {
            return GetEndOfDay(DateTime.Now);
        }
        /// <summary>
        /// 获取指定日期是周几
        /// </summary>
        /// <returns>日期</returns>
        public static int GetWeekdayOfCurrentDay()
        {
            return GetWeekdayOfDay(DateTime.Now);
        }
        /// <summary>
        /// 获取指定日期是周几
        /// </summary>
        /// <returns>周几</returns>
        public static int GetWeekdayOfDay(DateTime dt)
        {
            int week = Convert.ToInt32(dt.DayOfWeek.ToString("d"));
            return week;
        }
        #endregion
        #region 格式控制的
        /// <summary>
        /// 返回指定时间的标准日期格式
        /// </summary>
        /// <returns>yyyy-MM-dd</returns>
        public static string GetDateString(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 返回指定时间的标准时间格式string
        /// </summary>
        /// <returns>HH:mm:ss</returns>
        public static string GetTimeString(DateTime dt)
        {
            return dt.ToString("HH:mm:ss");
        }
        /// <summary>
        /// 返回指定时间的标准时间格式string
        /// </summary>
        /// <returns>yyyy-MM-dd HH:mm:ss</returns>
        public static string GetDateTimeString(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        ///  返回指定时间的标准全时间格式string
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>yyyy-MM-dd HH:mm:ss:fffffff</returns>
        public static string GetDateTimeFullString(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }
        /// <summary>
        /// 返回指定时间的标准日期格式
        /// </summary>
        /// <returns>yyyy-MM-dd</returns>
        public static string GetDateStringOfCurrentDay()
        {
            return GetDateString(DateTime.Now);
        }
        /// <summary>
        /// 返回指定时间的标准时间格式string
        /// </summary>
        /// <returns>HH:mm:ss</returns>
        public static string GetTimeStringOfCurrentDay()
        {
            return GetTimeString(DateTime.Now);
        }
        /// <summary>
        /// 返回指定时间的标准时间格式string
        /// </summary>
        /// <returns>yyyy-MM-dd HH:mm:ss</returns>
        public static string GetDateTimeStringOfCurrentDay()
        {
            return GetDateTimeString(DateTime.Now);
        }
        /// <summary>
        ///  返回指定时间的标准全时间格式string
        /// </summary>
        /// <returns>yyyy-MM-dd HH:mm:ss:fffffff</returns>
        public static string GetDateTimeFullStringOfCurrentDay()
        {
            return GetDateTimeFullString(DateTime.Now);
        }
        #endregion
        #region 日期比较的
        /// <summary>
        /// 获取多个日期的最大值（支持可变参数）
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMax(params DateTime[] dts)
        {
            return dts.Max();
        }
        /// <summary>
        ///  获取多个日期的最小值（支持可变参数）
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMin(params DateTime[] dts)
        {
            return dts.Min();
        }
        #endregion
        #region 日期差值
        /// <summary>
        /// 获取两个日期的差值
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan GetDifference(DateTime dt1, DateTime dt2)
        {
            return dt1 - dt2;
        }
        /// <summary>
        /// 获取2个日期相差的总天数（double）
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns>总天数（double)</returns>
        public static double GetDifferenceOfDay(DateTime dt1, DateTime dt2)
        {
            return GetDifference(dt1, dt2).TotalDays;
        }
        /// <summary>
        /// 获取2个日期相差的总天数（double）
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns>总天数（double)</returns>
        public static double GetDifferenceOfHours(DateTime dt1, DateTime dt2)
        {
            return GetDifference(dt1, dt2).TotalHours;
        }
        /// <summary>
        /// 获取2个日期相差的总分钟数（double）
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns>总分钟数（double)</returns>
        public static double GetDifferenceOfMinutes(DateTime dt1, DateTime dt2)
        {
            return GetDifference(dt1, dt2).TotalMinutes;
        }
        /// <summary>
        /// 获取2个日期相差的总秒数（double）
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns>总秒数（double)</returns>
        public static double GetDifferenceOfSeconds(DateTime dt1, DateTime dt2)
        {
            return GetDifference(dt1, dt2).TotalSeconds;
        }
        /// <summary>
        /// 获取2个日期相差的总毫秒（double）
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns>总毫秒数（double)</returns>
        public static double GetDifferenceOfMilliseconds(DateTime dt1, DateTime dt2)
        {
            return GetDifference(dt1, dt2).TotalMilliseconds;
        }
        #endregion
        #region 解析日期扩展

        /// <summary>
        /// 解析日期扩展， 支持以下几种解析（通用的使用datetime.parse解析）
        /// 20160504,2016年05月04日，2016年05月04，2016年5月04日, 2016年5月4, yyyy年05月5日, yyyy年05月4日
        /// </summary>
        /// <param name="yyyymmdd"></param>
        /// <exception cref="ArgumentException">无法解析</exception>
        /// <returns></returns>
        public static DateTime ParseExt(string yyyymmdd)
        {
            string[] format = { "yyyyMMdd", "yyyy年MM月dd日", "yyyy年MM月dd", "yyyy年M月dd日", "yyyy年M月d", "yyyy年MM月d日", "yyyy年MM月d日" };
            DateTime dt;
            if (DateTime.TryParseExact(yyyymmdd, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt;
            }
            throw new ArgumentException("字符串错误");
        }

        #endregion
    }
}
