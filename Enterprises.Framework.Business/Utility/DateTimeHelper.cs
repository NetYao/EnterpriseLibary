using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 时间帮助类
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// 获取日期yyyy-MM-dd字符串
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public static string ToShortDateString(this DateTime? dateValue)
        {
            try
            {
                if (dateValue == null)
                {
                    return "";
                }

                return dateValue.GetValueOrDefault().ToString("yyyy-MM-dd");
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取日期yyyy-MM-dd HH:mm:ss字符串
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime? dateValue)
        {
            try
            {
                if (dateValue == null)
                {
                    return "";
                }

                return dateValue.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取日期HH:mm:ss字符串
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public static string ToTimeString(this DateTime? dateValue)
        {
            try
            {
                if (dateValue == null)
                {
                    return "";
                }

                DateTime dt = dateValue.GetValueOrDefault();
                return string.Format("{0}:{1}:{2}", dt.Hour, dt.Minute, dt.Second);
            }
            catch
            {
                return string.Empty;
            }


        }

        /// <summary>
        /// 获取日期yyyy-MM-dd HH:mm字符串
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public static string ToDateTimeNoSecondsString(this DateTime? dateValue)
        {
            try
            {
                if (dateValue == null)
                {
                    return "";
                }

                return dateValue.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm");
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取今天起始时间（yyyy-MM-dd 00:00:00）
        /// </summary>
        /// <returns></returns>
        public static DateTime GetTodayStartTime()
        {
            DateTime dt = DateTime.Now;
            var str = string.Format("{0}-{1}-{2} 00:00:00", dt.Year, dt.Month, dt.Day);
            return Convert.ToDateTime(str);
        }

        /// <summary>
        /// 获取这周的第一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetWeekFristDay(this DateTime dateTime)
        {
            return dateTime.AddDays(Convert.ToDouble((0 - Convert.ToInt16(dateTime.DayOfWeek))));

        }

        /// <summary>
        /// 获取这周的最后一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetWeekLastDay(this DateTime dateTime)
        {
            return dateTime.AddDays(Convert.ToDouble((6 - Convert.ToInt16(dateTime.DayOfWeek))));
        }

        private static readonly string[] Day = new string[] {"星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"};

        /// <summary>
        /// 获取中午星期几
        /// </summary>
        /// <returns></returns>
        public static string ToCnWeekDay(this DateTime dateTime)
        {
            //return CaculateWeekDay(dateTime.Year, dateTime.Month, dateTime.Day);
            return Day[Convert.ToInt16(dateTime.DayOfWeek)];
        }

        /// <summary>
        /// 获取当月第一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMonthFristDay(this DateTime dateTime)
        {
            var dateStr = dateTime.ToString("yyyy-MM-01");
            return DateTime.Parse(dateStr);
        }

        /// <summary>
        /// 获取当月最后一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMonthLastDay(this DateTime dateTime)
        {
            return DateTime.Parse(dateTime.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 获取当前时间当年第一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetYearFristDay(this DateTime dateTime)
        {
            var dateStr = dateTime.ToString("yyyy-01-01");
            return DateTime.Parse(dateStr);
        }

        /// <summary>
        /// 获取当前时间当年最后一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetYearLastDay(this DateTime dateTime)
        {
            return DateTime.Parse(dateTime.ToString("yyyy-01-01")).AddYears(1).AddDays(-1);
        }

        /// <summary>
        /// 获取当前时间季度第一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetQuarterFristDay(this DateTime dateTime)
        {
            var dateStr = dateTime.AddMonths(0 - ((dateTime.Month - 1)%3)).ToString("yyyy-MM-01");
            return DateTime.Parse(dateStr);
        }

        /// <summary>
        /// 获取当前时间季度最后一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetQuarterLastDay(this DateTime dateTime)
        {
            return DateTime.Parse(dateTime.AddMonths(3 - ((dateTime.Month - 1)%3)).ToString("yyyy-MM-01")).AddDays(-1);
        }

        /// <summary>
        /// 获取时间月份的天数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int GetMonthDays(this DateTime dateTime)
        {
            int m = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return m;
        }


        /// <summary>
        /// 获取某月的实际工作日(即不包括周六日)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int GetDays(this DateTime dateTime)
        {
            int m = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            int mm = 0;
            for (int i = 1; i <= m; i++)
            {
                DateTime date = Convert.ToDateTime(dateTime.Year + "-" + dateTime.Month + "-" + i);
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Friday:
                        mm = mm + 1;
                        break;
                }
            }
            return mm;
        }

        /// <summary>
        /// 获取两个时间段的实际工作日(即不包括周六日)
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static int GetDays(DateTime date1, DateTime date2)
        {
            string m = DateDiff(DateInterval.Day, date1, date2).ToString("f0");
            int mm = 0;
            for (int i = 0; i <= Convert.ToInt32(m); i++)
            {
                DateTime date = Convert.ToDateTime(date1.AddDays(i));
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Friday:
                        mm = mm + 1;
                        break;
                }
            }
            return mm;
        }

        /// <summary>
        /// 获取两个时间的间隔
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static long DateDiff(DateInterval interval, DateTime startDate, DateTime endDate)
        {
            long lngDateDiffValue = 0;
            var ts = new TimeSpan(endDate.Ticks - startDate.Ticks);
            switch (interval)
            {
                case DateInterval.Second:
                    lngDateDiffValue = (long) ts.TotalSeconds;
                    break;
                case DateInterval.Minute:
                    lngDateDiffValue = (long) ts.TotalMinutes;
                    break;
                case DateInterval.Hour:
                    lngDateDiffValue = (long) ts.TotalHours;
                    break;
                case DateInterval.Day:
                    lngDateDiffValue = (long) ts.Days;
                    break;
                case DateInterval.Week:
                    lngDateDiffValue = (long) (ts.Days/7);
                    break;
                case DateInterval.Month:
                    lngDateDiffValue = (long) (ts.Days/30);
                    break;
                case DateInterval.Quarter:
                    lngDateDiffValue = (long) ((ts.Days/30)/3);
                    break;
                case DateInterval.Year:
                    lngDateDiffValue = (long) (ts.Days/365);
                    break;
            }

            return (lngDateDiffValue);
        }

        /// <summary>
        /// 获得本周的周六和周日
        /// </summary>
        /// <param name="date"></param>
        /// <param name="firstdate"></param>
        /// <param name="lastdate"></param>
        public static void ConvertDateToWeek(DateTime date, out DateTime firstdate, out DateTime lastdate)
        {
            DateTime first = DateTime.Now;
            DateTime last = DateTime.Now;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    first = date.AddDays(-1);
                    last = date.AddDays(5);
                    break;
                case DayOfWeek.Tuesday:
                    first = date.AddDays(-2);
                    last = date.AddDays(4);
                    break;
                case DayOfWeek.Wednesday:
                    first = date.AddDays(-3);
                    last = date.AddDays(3);
                    break;
                case DayOfWeek.Thursday:
                    first = date.AddDays(-4);
                    last = date.AddDays(2);
                    break;
                case DayOfWeek.Friday:
                    first = date.AddDays(-5);
                    last = date.AddDays(1);
                    break;
                case DayOfWeek.Saturday:
                    first = date.AddDays(-6);
                    last = date;
                    break;
                case DayOfWeek.Sunday:
                    first = date;
                    last = date.AddDays(6);
                    break;
            }

            firstdate = first;
            lastdate = last;
        }

        /// <summary>
        /// 获得当前日期是该年度的第几周
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int GetWeeks(this DateTime dateTime)
        {
            int weeks = dateTime.DayOfYear / 7 + 1;
            return weeks;
        }
		
        /// <summary>
        /// 获取本月的第几周
        /// </summary>
        /// <param name="daytime"></param>
        /// <returns></returns>
		public static int GetWeekNumInMonth(DateTime daytime)
		{

			int dayInMonth = daytime.Day;

			//本月第一天

			DateTime firstDay = daytime.AddDays(1 - daytime.Day);

			//本月第一天是周几

			int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;

			//本月第一周有几天

			int firstWeekEndDay = 7 - (weekday-1);

			//当前日期和第一周之差

			int diffday = dayInMonth - firstWeekEndDay;

			diffday = diffday > 0 ? diffday : 1;

			//当前是第几周,如果整除7就减一天

			int WeekNumInMonth = ((diffday % 7) == 0 

			 ? (diffday / 7-1)

			 :(diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);

			return WeekNumInMonth;

		}

        /// <summary>
        /// Adds one month to the specified date.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing one month after of the specified date.</returns>
        public static DateTime AddMonth(this DateTime value)
        {
            switch (value.Month)
            {
                case 1:
                    if (value.IsLeapYear() == true & value.Day > 29)
                    {
                        return new DateTime(value.Year, 2, 29);
                    }
                    else if (value.Day > 28)
                    {
                        return new DateTime(value.Year, 2, 28);
                    }
                    else
                    {
                        return new DateTime(value.Year, 2, value.Day);
                    }

                case 2:
                    return new DateTime(value.Year, 3, value.Day);
                case 3:
                case 5:
                case 8:
                case 10:
                    if (value.Day > 30)
                    {
                        return new DateTime(value.Year, value.Month + 1, 30);
                    }
                    else
                    {
                        return new DateTime(value.Year, value.Month + 1, value.Day);
                    }

                case 4:
                case 6:
                case 7:
                case 9:
                case 11:
                    return new DateTime(value.Year, value.Month + 1, value.Day);
                case 12:
                    return new DateTime(value.Year + 1, 1, value.Day);
            }

            return value;
        }

        /// <summary>
        /// Adds one week to the specified <see cref="DateTime">DateTime</see> value.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing one weeek after the specified <see cref="DateTime">DateTime</see>.</returns>
        public static DateTime AddWeek(this DateTime value)
        {
            return value.AddWeeks(1);
        }

        /// <summary>
        /// Adds the specified number of weeks to the specified <see cref="DateTime">DateTime</see> value.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="numberOfWeeks">The number of weeks to add to the specified value.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing the specified number of weeks after the specified <see cref="DateTime">DateTime</see>.</returns>
        public static DateTime AddWeeks(this DateTime value, int numberOfWeeks)
        {
            return value.Add(new TimeSpan(7 * numberOfWeeks, 0, 0, 0));
        }

        /// <summary>
        /// Gets the number of days since the specified date.
        /// </summary>
        /// <param name="value">The date to compute.</param>
        /// <returns>The number of days since the specified date.</returns>
        public static int DaysSince(this DateTime value)
        {
            return DateTime.Now.Subtract(value).Days;
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> representing 23:59:59 on the specified date.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing 23:59:59 on the specified date.</returns>
        public static DateTime EndOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> object representing the first calendar day in the fiscal year for the specified date.
        /// </summary>
        /// <param name="value">The date to be evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> object representing the first calendar day in the fiscal year for the specified date.</returns>
        public static DateTime FirstCalendarDayOfCurrentFiscalYear(this DateTime value)
        {
            return new DateTime(value.FiscalYear() - 1, 10, 1);
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> object representing the first calendar day in the previous fiscal year for the specified date.
        /// </summary>
        /// <param name="value">The date to be evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> object representing the first calendar day in the previous fiscal year for the specified date.</returns>
        public static DateTime FirstCalendarDayOfPreviousFiscalYear(this DateTime value)
        {
            if (value.FiscalYear() == DateTime.Now.Year)
            {
                return new DateTime(value.FiscalYear() - 2, 10, 1);
            }
            else
            {
                return new DateTime(value.FiscalYear() - 3, 10, 1);
            }
        }

        /// <summary>
        /// Gets the first day of the current quarter.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>The first day of the current quarter.</returns>
        public static DateTime FirstDayOfCurrentQuarter(this DateTime value)
        {
            switch (value.Month)
            {
                case 1:
                case 2:
                case 3:
                    return new DateTime(value.Year, 1, 1);
                case 4:
                case 5:
                case 6:
                    return new DateTime(value.Year, 4, 1);
                case 7:
                case 8:
                case 9:
                    return new DateTime(value.Year, 7, 1);
                case 10:
                case 11:
                case 12:
                    return new DateTime(value.Year, 10, 1);
            }

            return value;
        }

        /// <summary>
        /// Gets the first day in the month for the given date.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing the first day in the month for the specified date.</returns>
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return value.AddDays(1 - value.Day);
        }

        /// <summary>
        /// Gets the first instance of the specified day in the specified month.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="dayOfWeek">The day of week to return.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing the specified first day of the week in the month for the specified date.</returns>
        public static DateTime FirstDayOfMonth(this DateTime value, DayOfWeek dayOfWeek)
        {
            switch (value.FirstDayOfMonth().DayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.FirstDayOfMonth();
                        case System.DayOfWeek.Monday:
                            return value.FirstDayOfMonth().AddDays(1);
                        case System.DayOfWeek.Tuesday:
                            return value.FirstDayOfMonth().AddDays(2);
                        case System.DayOfWeek.Wednesday:
                            return value.FirstDayOfMonth().AddDays(3);
                        case System.DayOfWeek.Thursday:
                            return value.FirstDayOfMonth().AddDays(4);
                        case System.DayOfWeek.Friday:
                            return value.FirstDayOfMonth().AddDays(5);
                        case System.DayOfWeek.Saturday:
                            return value.FirstDayOfMonth().AddDays(6);
                    }

                    break;
                case System.DayOfWeek.Monday:
                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.FirstDayOfMonth().AddDays(6);
                        case System.DayOfWeek.Monday:
                            return value.FirstDayOfMonth();
                        case System.DayOfWeek.Tuesday:
                            return value.FirstDayOfMonth().AddDays(1);
                        case System.DayOfWeek.Wednesday:
                            return value.FirstDayOfMonth().AddDays(2);
                        case System.DayOfWeek.Thursday:
                            return value.FirstDayOfMonth().AddDays(3);
                        case System.DayOfWeek.Friday:
                            return value.FirstDayOfMonth().AddDays(4);
                        case System.DayOfWeek.Saturday:
                            return value.FirstDayOfMonth().AddDays(5);
                    }

                    break;
                case System.DayOfWeek.Tuesday:
                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.FirstDayOfMonth().AddDays(5);
                        case System.DayOfWeek.Monday:
                            return value.FirstDayOfMonth().AddDays(6);
                        case System.DayOfWeek.Tuesday:
                            return value.FirstDayOfMonth();
                        case System.DayOfWeek.Wednesday:
                            return value.FirstDayOfMonth().AddDays(1);
                        case System.DayOfWeek.Thursday:
                            return value.FirstDayOfMonth().AddDays(2);
                        case System.DayOfWeek.Friday:
                            return value.FirstDayOfMonth().AddDays(3);
                        case System.DayOfWeek.Saturday:
                            return value.FirstDayOfMonth().AddDays(4);
                    }

                    break;
                case System.DayOfWeek.Wednesday:

                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.FirstDayOfMonth().AddDays(4);
                        case System.DayOfWeek.Monday:
                            return value.FirstDayOfMonth().AddDays(5);
                        case System.DayOfWeek.Tuesday:
                            return value.FirstDayOfMonth().AddDays(6);
                        case System.DayOfWeek.Wednesday:
                            return value.FirstDayOfMonth();
                        case System.DayOfWeek.Thursday:
                            return value.FirstDayOfMonth().AddDays(1);
                        case System.DayOfWeek.Friday:
                            return value.FirstDayOfMonth().AddDays(2);
                        case System.DayOfWeek.Saturday:
                            return value.FirstDayOfMonth().AddDays(3);
                    }

                    break;
                case System.DayOfWeek.Thursday:

                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.FirstDayOfMonth().AddDays(3);
                        case System.DayOfWeek.Monday:
                            return value.FirstDayOfMonth().AddDays(4);
                        case System.DayOfWeek.Tuesday:
                            return value.FirstDayOfMonth().AddDays(5);
                        case System.DayOfWeek.Wednesday:
                            return value.FirstDayOfMonth().AddDays(6);
                        case System.DayOfWeek.Thursday:
                            return value.FirstDayOfMonth();
                        case System.DayOfWeek.Friday:
                            return value.FirstDayOfMonth().AddDays(1);
                        case System.DayOfWeek.Saturday:
                            return value.FirstDayOfMonth().AddDays(2);
                    }

                    break;
                case System.DayOfWeek.Friday:
                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.FirstDayOfMonth().AddDays(2);
                        case System.DayOfWeek.Monday:
                            return value.FirstDayOfMonth().AddDays(3);
                        case System.DayOfWeek.Tuesday:
                            return value.FirstDayOfMonth().AddDays(4);
                        case System.DayOfWeek.Wednesday:
                            return value.FirstDayOfMonth().AddDays(5);
                        case System.DayOfWeek.Thursday:
                            return value.FirstDayOfMonth().AddDays(6);
                        case System.DayOfWeek.Friday:
                            return value.FirstDayOfMonth();
                        case System.DayOfWeek.Saturday:
                            return value.FirstDayOfMonth().AddDays(1);
                    }

                    break;
                case System.DayOfWeek.Saturday:

                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.FirstDayOfMonth().AddDays(1);
                        case System.DayOfWeek.Monday:
                            return value.FirstDayOfMonth().AddDays(2);
                        case System.DayOfWeek.Tuesday:
                            return value.FirstDayOfMonth().AddDays(3);
                        case System.DayOfWeek.Wednesday:
                            return value.FirstDayOfMonth().AddDays(4);
                        case System.DayOfWeek.Thursday:
                            return value.FirstDayOfMonth().AddDays(5);
                        case System.DayOfWeek.Friday:
                            return value.FirstDayOfMonth().AddDays(6);
                        case System.DayOfWeek.Saturday:
                            return value.FirstDayOfMonth();
                    }

                    break;
            }

            return value;
        }

        /// <summary>
        /// Gets the first day of the previous quarter.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>The first day of the previous government's fiscal quarter.</returns>
        public static DateTime FirstDayOfPreviousQuarter(this DateTime value)
        {
            switch (value.Month)
            {
                case 1:
                case 2:
                case 3:
                    return new DateTime(value.Year - 1, 10, 1);
                case 4:
                case 5:
                case 6:
                    return new DateTime(value.Year, 1, 1);
                case 7:
                case 8:
                case 9:
                    return new DateTime(value.Year, 4, 1);
                case 10:
                case 11:
                case 12:
                    return new DateTime(value.Year, 7, 1);
            }

            return value;
        }

        /// <summary>
        /// Gets the first day of the week (Sunday) for the given date.
        /// </summary>
        /// <param name="value">The date to use for determining the first day of the week.</param>
        /// <returns>The first day of the week for the given date.</returns>
        /// <remarks>Assumes that Sunday is the first day of the week.</remarks>
        public static DateTime FirstDayOfWeek(this DateTime value)
        {
            // Variable Declarations
            System.DateTime dteReturnvalue = default(System.DateTime);

            // Determine the First day of the specified week
            try
            {
                switch (value.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        dteReturnvalue = value;
                        break;
                    case DayOfWeek.Monday:
                        dteReturnvalue = value.Subtract(new TimeSpan(1, 0, 0, 0));
                        break;
                    case DayOfWeek.Tuesday:
                        dteReturnvalue = value.Subtract(new TimeSpan(2, 0, 0, 0));
                        break;
                    case DayOfWeek.Wednesday:
                        dteReturnvalue = value.Subtract(new TimeSpan(3, 0, 0, 0));
                        break;
                    case DayOfWeek.Thursday:
                        dteReturnvalue = value.Subtract(new TimeSpan(4, 0, 0, 0));
                        break;
                    case DayOfWeek.Friday:
                        dteReturnvalue = value.Subtract(new TimeSpan(5, 0, 0, 0));
                        break;
                    case DayOfWeek.Saturday:
                        dteReturnvalue = value.Subtract(new TimeSpan(6, 0, 0, 0));
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dteReturnvalue;
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> object representing the first work day in the fiscal year for the specified date.
        /// </summary>
        /// <param name="value">The date to be evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> object representing the first work day in the fiscal year for the specified date.</returns>
        public static DateTime FirstWorkDayOfCurrentFiscalYear(this DateTime value)
        {
            switch (value.FirstCalendarDayOfCurrentFiscalYear().DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return value.FirstCalendarDayOfCurrentFiscalYear().AddDays(1);
                case DayOfWeek.Saturday:
                    return value.FirstCalendarDayOfCurrentFiscalYear().AddDays(2);
                default:
                    return value.FirstCalendarDayOfCurrentFiscalYear();
            }
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> object representing the first work day in the previous fiscal year for the specified date.
        /// </summary>
        /// <param name="value">The date to be evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> object representing the first work day in the previous fiscal year for the specified date.</returns>
        public static DateTime FirstWorkDayOfPreviousFiscalYear(this DateTime value)
        {
            switch (value.FirstCalendarDayOfPreviousFiscalYear().DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return value.FirstCalendarDayOfPreviousFiscalYear().AddDays(1);
                case DayOfWeek.Saturday:
                    return value.FirstCalendarDayOfPreviousFiscalYear().AddDays(2);
                default:
                    return value.FirstCalendarDayOfPreviousFiscalYear();
            }
        }

        /// <summary>
        /// Gets the government's fiscal year for the specified date.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>The government's fiscal year.</returns>
        public static short FiscalYear(this DateTime value)
        {
            // Variable Declarations
            short shtReturnvalue = 0;

            // Determine the fiscal year
            try
            {
                if (value.Month > 9)
                {
                    shtReturnvalue = Convert.ToInt16(value.Year + 1);
                }
                else
                {
                    shtReturnvalue = Convert.ToInt16(value.Year);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return shtReturnvalue;
        }

        /// <summary>
        /// Attempts the supplied date/time <see cref="string">string</see> value.
        /// </summary>
        /// <param name="value">The date to format.</param>
        /// <param name="includeTime">If set to <c>True</c> the time will be included in the formatted <see cref="string">string</see>.</param>
        /// <returns>A date formatted as dd-MMM-yyyy HH:mm:ss.</returns>
        /// <remarks>If the passed in <paramref name="value">value</paramref> is not a date, the method returns "N/A".</remarks>
        public static string FormatDateTime(this string value, bool includeTime)
        {
            if (string.IsNullOrEmpty(value))
            {
                return DateTime.Parse(value).ToDODString(includeTime);
            }
            else
            {
                return "N/A";
            }
        }

        /// <summary>
        /// Converts a fractional hour value like 1.25 to 1:15  hours:minutes format
        /// </summary>
        /// <param name="hours">Decimal hour value</param>
        /// <returns>The resulting string.</returns>
        public static string FractionalHoursToString(this decimal hours)
        {
            return hours.FractionalHoursToString(null);
        }

        /// <summary>
        /// Converts a fractional hour value to the specified format
        /// </summary>
        /// <param name="hours">Decimal hour value</param>
        /// <param name="format">An optional format string where {0} is hours and {1} is minutes.</param>
        /// <returns>The resulting string.</returns>
        public static string FractionalHoursToString(this decimal hours, string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "{0}:{1}";
            }

            TimeSpan tspan = TimeSpan.FromHours((double)hours);

            return string.Format(format, tspan.Hours, tspan.Minutes);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTime">DateTime</see> is in a leap year.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is in a leap year; otherwise, <c>false</c>.</returns>
        public static bool IsLeapYear(this DateTime value)
        {
            return System.DateTime.IsLeapYear(value.Year);
        }

        /// <summary>
        /// Returns whether the DateTime falls on a weekday.
        /// </summary>
        /// <param name="date">The date to be processed.</param>
        /// <returns>Whether the specified date occurs on a weekday.</returns>
        public static bool IsWeekDay(this DateTime date)
        {
            return !date.IsWeekend();
        }

        /// <summary>
        /// Lets you easily figure out if value of this instance a date value that is a weekend.
        /// </summary>
        /// <param name="value">DateTime to inspect.</param>
        /// <returns>Result of inspection.</returns>
        public static bool IsWeekend(this DateTime value)
        {
            return value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday;
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> object representing the first calendar day in the fiscal year for the specified date.
        /// </summary>
        /// <param name="value">The date to be evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> object representing the first calendar day in the fiscal year for the specified date.</returns>
        public static DateTime LastCalendarDayOfCurrentFiscalYear(this DateTime value)
        {
            return new DateTime(value.FiscalYear(), 9, System.DateTime.DaysInMonth(value.FiscalYear(), 9));
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> object representing the first calendar day in the previous fiscal year for the specified date.
        /// </summary>
        /// <param name="value">The date to be evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> object representing the last calendar day in the previous fiscal year for the specified date.</returns>
        public static DateTime LastCalendarDayOfPreviousFiscalYear(this DateTime value)
        {
            return new DateTime(value.FiscalYear() - 1, 9, System.DateTime.DaysInMonth(value.FiscalYear() - 1, 9));
        }

        /// <summary>
        /// Gets the last day in the month for the given date.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing the last day in the month for the specified date.</returns>
        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return value.FirstDayOfMonth().AddDays(DateTime.DaysInMonth(value.Year, value.Month) - 1);
        }

        /// <summary>
        /// Gets the last specified day of the week in the month for the given date.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="dayOfWeek">The day of week to return.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing the specified last day of the week in the month for the specified date.</returns>
        public static DateTime LastDayOfMonth(this DateTime value, DayOfWeek dayOfWeek)
        {
            switch (value.LastDayOfMonth().DayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.LastDayOfMonth();
                        case System.DayOfWeek.Monday:
                            return value.LastDayOfMonth().AddDays(-6);
                        case System.DayOfWeek.Tuesday:
                            return value.LastDayOfMonth().AddDays(-5);
                        case System.DayOfWeek.Wednesday:
                            return value.LastDayOfMonth().AddDays(-4);
                        case System.DayOfWeek.Thursday:
                            return value.LastDayOfMonth().AddDays(-3);
                        case System.DayOfWeek.Friday:
                            return value.LastDayOfMonth().AddDays(-2);
                        case System.DayOfWeek.Saturday:
                            return value.LastDayOfMonth().AddDays(-1);
                    }

                    break;
                case System.DayOfWeek.Monday:

                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.LastDayOfMonth().AddDays(-1);
                        case System.DayOfWeek.Monday:
                            return value.LastDayOfMonth();
                        case System.DayOfWeek.Tuesday:
                            return value.LastDayOfMonth().AddDays(-6);
                        case System.DayOfWeek.Wednesday:
                            return value.LastDayOfMonth().AddDays(-5);
                        case System.DayOfWeek.Thursday:
                            return value.LastDayOfMonth().AddDays(-4);
                        case System.DayOfWeek.Friday:
                            return value.LastDayOfMonth().AddDays(-3);
                        case System.DayOfWeek.Saturday:
                            return value.LastDayOfMonth().AddDays(-2);
                    }

                    break;
                case System.DayOfWeek.Tuesday:

                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.LastDayOfMonth().AddDays(-2);
                        case System.DayOfWeek.Monday:
                            return value.LastDayOfMonth().AddDays(-1);
                        case System.DayOfWeek.Tuesday:
                            return value.LastDayOfMonth();
                        case System.DayOfWeek.Wednesday:
                            return value.LastDayOfMonth().AddDays(-6);
                        case System.DayOfWeek.Thursday:
                            return value.LastDayOfMonth().AddDays(-5);
                        case System.DayOfWeek.Friday:
                            return value.LastDayOfMonth().AddDays(-4);
                        case System.DayOfWeek.Saturday:
                            return value.LastDayOfMonth().AddDays(-3);
                    }

                    break;
                case System.DayOfWeek.Wednesday:

                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.LastDayOfMonth().AddDays(-3);
                        case System.DayOfWeek.Monday:
                            return value.LastDayOfMonth().AddDays(-2);
                        case System.DayOfWeek.Tuesday:
                            return value.LastDayOfMonth().AddDays(-1);
                        case System.DayOfWeek.Wednesday:
                            return value.LastDayOfMonth();
                        case System.DayOfWeek.Thursday:
                            return value.LastDayOfMonth().AddDays(-6);
                        case System.DayOfWeek.Friday:
                            return value.LastDayOfMonth().AddDays(-5);
                        case System.DayOfWeek.Saturday:
                            return value.LastDayOfMonth().AddDays(-4);
                    }

                    break;
                case System.DayOfWeek.Thursday:

                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.LastDayOfMonth().AddDays(-4);
                        case System.DayOfWeek.Monday:
                            return value.LastDayOfMonth().AddDays(-3);
                        case System.DayOfWeek.Tuesday:
                            return value.LastDayOfMonth().AddDays(-2);
                        case System.DayOfWeek.Wednesday:
                            return value.LastDayOfMonth().AddDays(-1);
                        case System.DayOfWeek.Thursday:
                            return value.LastDayOfMonth();
                        case System.DayOfWeek.Friday:
                            return value.LastDayOfMonth().AddDays(-6);
                        case System.DayOfWeek.Saturday:
                            return value.LastDayOfMonth().AddDays(-5);
                    }

                    break;
                case System.DayOfWeek.Friday:

                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.LastDayOfMonth().AddDays(-5);
                        case System.DayOfWeek.Monday:
                            return value.LastDayOfMonth().AddDays(-4);
                        case System.DayOfWeek.Tuesday:
                            return value.LastDayOfMonth().AddDays(-3);
                        case System.DayOfWeek.Wednesday:
                            return value.LastDayOfMonth().AddDays(-2);
                        case System.DayOfWeek.Thursday:
                            return value.LastDayOfMonth().AddDays(-1);
                        case System.DayOfWeek.Friday:
                            return value.LastDayOfMonth();
                        case System.DayOfWeek.Saturday:
                            return value.LastDayOfMonth().AddDays(-6);
                    }

                    break;
                case System.DayOfWeek.Saturday:

                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            return value.LastDayOfMonth().AddDays(-6);
                        case System.DayOfWeek.Monday:
                            return value.LastDayOfMonth().AddDays(-5);
                        case System.DayOfWeek.Tuesday:
                            return value.LastDayOfMonth().AddDays(-4);
                        case System.DayOfWeek.Wednesday:
                            return value.LastDayOfMonth().AddDays(-3);
                        case System.DayOfWeek.Thursday:
                            return value.LastDayOfMonth().AddDays(-2);
                        case System.DayOfWeek.Friday:
                            return value.LastDayOfMonth().AddDays(-1);
                        case System.DayOfWeek.Saturday:
                            return value.LastDayOfMonth();
                    }

                    break;
            }

            return value;
        }

        /// <summary>
        /// Gets the last day of the previous government's fiscal quarter.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>The last day of the previous government's fiscal quarter.</returns>
        public static DateTime LastDayOfPreviousQuarter(this DateTime value)
        {
            switch (value.Month)
            {
                case 1:
                case 2:
                case 3:
                    return new DateTime(value.Year - 1, 12, 31);
                case 4:
                case 5:
                case 6:
                    return new DateTime(value.Year, 3, 31);
                case 7:
                case 8:
                case 9:
                    return new DateTime(value.Year, 6, 30);
                case 10:
                case 11:
                case 12:
                    return new DateTime(value.Year, 9, 30);
            }

            return value;
        }

        /// <summary>
        /// Gets the <see cref="DateTime">DateTime</see> that represents the last day of the week (Saturday) for the given date.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>The <see cref="DateTime">DateTime</see> that represents the last day of the week (Saturday) for the given date.</returns>
        public static DateTime LastDayOfWeek(this DateTime value)
        {
            // Variable Declarations
            System.DateTime dteReturnvalue = default(System.DateTime);

            // Determine the last day of the week
            try
            {
                switch (value.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        dteReturnvalue = value.Add(new TimeSpan(6, 0, 0, 0));
                        break;
                    case DayOfWeek.Monday:
                        dteReturnvalue = value.Add(new TimeSpan(5, 0, 0, 0));
                        break;
                    case DayOfWeek.Tuesday:
                        dteReturnvalue = value.Add(new TimeSpan(4, 0, 0, 0));
                        break;
                    case DayOfWeek.Wednesday:
                        dteReturnvalue = value.Add(new TimeSpan(3, 0, 0, 0));
                        break;
                    case DayOfWeek.Thursday:
                        dteReturnvalue = value.Add(new TimeSpan(2, 0, 0, 0));
                        break;
                    case DayOfWeek.Friday:
                        dteReturnvalue = value.Add(new TimeSpan(1, 0, 0, 0));
                        break;
                    case DayOfWeek.Saturday:
                        dteReturnvalue = value;
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dteReturnvalue;
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> object representing the first work day in the fiscal year for the specified date.
        /// </summary>
        /// <param name="value">The date to be evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> object representing the first work day in the fiscal year for the specified date.</returns>
        public static DateTime LastWorkDayOfCurrentFiscalYear(this DateTime value)
        {
            switch (value.LastCalendarDayOfCurrentFiscalYear().DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return value.LastCalendarDayOfCurrentFiscalYear().Subtract(new TimeSpan(2, 0, 0, 0));
                case DayOfWeek.Saturday:
                    return value.LastCalendarDayOfCurrentFiscalYear().Subtract(new TimeSpan(1, 0, 0, 0));
                default:
                    return value.LastCalendarDayOfCurrentFiscalYear();
            }
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> object representing the first work day in the previous fiscal year for the specified date.
        /// </summary>
        /// <param name="value">The date to be evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> object representing the last work day in the previous fiscal year for the specified date.</returns>
        public static DateTime LastWorkDayOfPreviousFiscalYear(this DateTime value)
        {
            switch (value.LastCalendarDayOfPreviousFiscalYear().DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return value.LastCalendarDayOfPreviousFiscalYear().Subtract(new TimeSpan(2, 0, 0, 0));
                case DayOfWeek.Saturday:
                    return value.LastCalendarDayOfPreviousFiscalYear().Subtract(new TimeSpan(1, 0, 0, 0));
                default:
                    return value.LastCalendarDayOfPreviousFiscalYear();
            }
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> representing midnight on the specified date.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing midnight on the specified date.</returns>
        public static DateTime Midnight(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day);
        }

        /// <summary>
        /// Gets the first date following the specified date which falls on the specified day of the week.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="dayOfWeek">The day of week to return.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing the next specified day of week that falls after after the specified date.</returns>
        public static DateTime NextDayOfMonth(this DateTime value, DayOfWeek dayOfWeek)
        {
            int intOffsetDays = dayOfWeek - value.DayOfWeek;

            if (intOffsetDays <= 0)
            {
                intOffsetDays += 7;
            }

            return value.AddDays(intOffsetDays);
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> representing noon on the specified date.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing noon on the specified date.</returns>
        public static DateTime Noon(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 12, 0, 0);
        }

        /// <summary>
        /// Sets the time of the specified date with hour precision.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="hour">The hour to set the return value to.</param>
        /// <returns>The specified date set to hour precision.</returns>
        public static DateTime SetTime(this DateTime value, int hour)
        {
            return SetTime(value, hour, 0);
        }

        /// <summary>
        /// Sets the time of the specified date with minute precision.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="hour">The hour to set the return value to.</param>
        /// <param name="minute">The minute to set the return value to.</param>
        /// <returns>The specified date set to minute precision.</returns>
        public static DateTime SetTime(this DateTime value, int hour, int minute)
        {
            return SetTime(value, hour, minute, 0, 0);
        }

        /// <summary>
        /// Sets the time of the specified date with second precision.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="hour">The hour to set the return value to.</param>
        /// <param name="minute">The minute to set the return value to.</param>
        /// <param name="second">The second to set the return value to.</param>
        /// <returns>The specified date set to second precision.</returns>
        public static DateTime SetTime(this DateTime value, int hour, int minute, int second)
        {
            return SetTime(value, hour, minute, second, 0);
        }

        /// <summary>
        /// Sets the time of the specified date with millisecond precision.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="hour">The hour to set the return value to.</param>
        /// <param name="minute">The minute to set the return value to.</param>
        /// <param name="second">The second to set the return value to.</param>
        /// <param name="millisecond">The millisecond to set the return value to.</param>
        /// <returns>The specified date set to millisecond precision.</returns>
        public static DateTime SetTime(this DateTime value, int hour, int minute, int second, int millisecond)
        {
            return new DateTime(value.Year, value.Month, value.Day, hour, minute, second, millisecond);
        }

        /// <summary>
        /// Returns a new DateTime representing the exact start of the day for the specified instance.
        /// </summary>
        /// <param name="value">DateTime for which to retrieve the start of the day.</param>
        /// <returns>A new DateTime representing the start of the day.</returns>
        public static DateTime StartOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, 0);
        }

        /// <summary>
        /// Subtracts the specified number of days from the value of this instance.
        /// </summary>
        /// <param name="dt">DateTime to subtract days from.</param>
        /// <param name="value">A number of whole and fractional days.  The parameter value can be positive or negative.</param>
        /// <returns>The resulting date.</returns>
        public static DateTime SubtractDays(this DateTime dt, double value)
        {
            return dt.AddDays((-1 * value));
        }

        /// <summary>
        /// Subtracts one month from the specified date.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing one month previous of the specified date.</returns>
        public static DateTime SubtractMonth(this DateTime value)
        {
            switch (value.Month)
            {
                case 1:

                    return new DateTime(value.Year - 1, 12, value.Day);

                case 2:
                case 6:
                case 8:
                case 9:
                case 11:

                    return new DateTime(value.Year, value.Month - 1, value.Day);

                case 3:

                    if (value.IsLeapYear() == true & value.Day > 29)
                    {
                        return new DateTime(value.Year, 2, 29);
                    }
                    else if (value.Day > 28)
                    {
                        return new DateTime(value.Year, 2, 28);
                    }
                    else
                    {
                        return new DateTime(value.Year, 2, value.Day);
                    }

                case 4:

                    return new DateTime(value.Year, value.Month - 1, value.Day);

                case 5:
                case 7:
                case 10:
                case 12:

                    if (value.Day > 30)
                    {
                        return new DateTime(value.Year, value.Month - 1, 30);
                    }
                    else
                    {
                        return new DateTime(value.Year, value.Month - 1, value.Day);
                    }
            }

            return value;
        }

        /// <summary>
        /// Subtracts one week from the specified <see cref="DateTime">DateTime</see> value.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing one weeek from the specified <see cref="DateTime">DateTime</see>.</returns>
        public static DateTime SubtractWeek(this DateTime value)
        {
            return value.SubtractWeeks(1);
        }

        /// <summary>
        /// Subtracts the specified number of weeks from the specified <see cref="DateTime">DateTime</see> value.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="numberOfWeeks">The number of weeks to subtract from the specified value.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> representing the specified number of weeks from the specified <see cref="DateTime">DateTime</see>.</returns>
        public static DateTime SubtractWeeks(this DateTime value, int numberOfWeeks)
        {
            return value.Subtract(new TimeSpan(7 * numberOfWeeks, 0, 0, 0));
        }

        /// <summary>
        /// Formats past dates in a more user friendly way.  For example,
        /// "12 hours ago".
        /// </summary>
        /// <param name="value">DateTime to compare to the current DateTime.</param>
        /// <returns>The resulting string.</returns>
        public static string ToAgoString(this DateTime value)
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = now - value;

            if (ts.Days == 0)
            {
                double totalMinutes = Math.Round(ts.TotalMinutes);

                if (totalMinutes < 60)
                {
                    if (totalMinutes == 1)
                    {
                        return string.Format("{0} minute ago", totalMinutes.ToString());
                    }
                    else
                    {
                        return string.Format("{0} minutes ago", totalMinutes.ToString());
                    }
                }

                if (ts.Hours == 1)
                {
                    return string.Format("{0} hour ago", ts.Hours.ToString());
                }
                else
                {
                    return string.Format("{0} hours ago", ts.Hours.ToString());
                }
            }
            else if (ts.Days == 1)
            {
                return "Yesterday";
            }
            else if (ts.Days >= 2 && ts.Days <= 5)
            {
                return string.Format("{0} days ago", ts.Days.ToString());
            }
            else if (ts.Days >= 6 && ts.Days <= 12)
            {
                return "1 week ago";
            }
            else if (ts.Days >= 13 && ts.Days <= 19)
            {
                return "2 weeks ago";
            }

            return value.ToFriendlyString();
        }

        /// <summary>
        /// Formats the specified date as a properly DOD formatted <see cref="String">String</see>.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <param name="includeTime">If set to <c>True</c> the time will be included in the formatted <see cref="String">String</see>.</param>
        /// <returns>If includeTime is false, a date formatted as 31-DEC-1971.  Otherwise, a date formatted as 31-DEC-1971 12:30:00.</returns>
        public static string ToDODString(this DateTime value, bool includeTime)
        {
            if (includeTime == true)
            {
                return value.ToString("dd-MMM-yyyy HH:mm:ss").ToUpper();
            }
            else
            {
                return value.ToString("dd-MMM-yyyy").ToUpper();
            }
        }

        /// <summary>
        /// Represents dates in a more user friendly way.
        /// </summary>
        /// <param name="value">DateTime to use when generating the string.</param>
        /// <returns>Friendlier, more readable date string.</returns>
        public static string ToFriendlyString(this DateTime value)
        {
            string s = string.Empty;

            if (value.Date == DateTime.Today)
            {
                s = "Today";
            }
            else if (value.Date == DateTime.Today.AddDays(-1))
            {
                s = "Yesterday";
            }
            else if (value.Date > DateTime.Today.AddDays(-6))
            {
                s = value.ToString("dddd").ToString();
            }
            else
            {
                s = value.ToString("MMMM dd, yyyy");
            }

            s += string.Format(" @ {0}", value.ToString("t").ToLower());

            return s;
        }

        /// <summary>
        /// Gets a <see cref="DateTime">DateTime</see> that represents the current hour.
        /// </summary>
        /// <param name="value">The date to evaluate.</param>
        /// <returns>A <see cref="DateTime">DateTime</see> that represents the current hour.</returns>
        public static DateTime ToHour(this DateTime value)
        {
            return value.SetTime(value.Hour);
        }

        /// <summary>
        /// Converts a regular DateTime to a RFC822 date string.
        /// </summary>
        /// <param name="value">DateTime to use when generating the string.</param>
        /// <returns>The specified date formatted as a RFC822 date string.</returns>
        public static string ToRFC822DateString(this DateTime value)
        {
            int offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;

            string timeZone = string.Format("+{0}", offset.ToString().PadLeft(2, '0'));

            if (offset < 0)
            {
                int i = offset * -1;

                timeZone = string.Format("-{0}", i.ToString().PadLeft(2, '0'));
            }

            return value.ToString(
                string.Format("ddd, dd MMM yyyy HH:mm:ss {0}", timeZone.PadRight(5, '0')),
                System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

        /// <summary>
        /// Formats a friendly string indicating the amound of time until a date.
        /// </summary>
        /// <param name="value">Date to use in calculation.</param>
        /// <returns>Friendly, compact string (e.g. 2d 4h 7m).</returns>
        public static string ToUntilString(this DateTime value)
        {
            TimeSpan diff = value - DateTime.Now;

            int days = diff.Days;
            int hours = diff.Hours;
            int minutes = diff.Minutes;

            return string.Format("{0}d, {1}h, {2}m", diff.Days, diff.Hours, diff.Minutes);
        }

     
        
    }
}

/// <summary>
/// 时间间隔
/// </summary>
public enum DateInterval
{
    /// <summary>
    /// 秒
    /// </summary>
    Second,
    /// <summary>
    /// 分钟
    /// </summary>
    Minute,
    /// <summary>
    /// 小时
    /// </summary>
    Hour,
    /// <summary>
    /// 天
    /// </summary>
    Day,
    /// <summary>
    /// 周
    /// </summary>
    Week,
    /// <summary>
    /// 月份
    /// </summary>
    Month,
    /// <summary>
    /// 季度
    /// </summary>
    Quarter,
    /// <summary>
    /// 年
    /// </summary>
    Year,
}

