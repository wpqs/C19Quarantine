using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace C19QCalcLib
{
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public static class Extensions
    {
        public static readonly DateTimeOffset DateTimeOffsetError = DateTimeOffset.MaxValue;
        public static readonly DateTime DateTimeError = DateTime.MaxValue;
        public static readonly TimeSpan TimeSpanError = TimeSpan.MaxValue;

        public static bool IsError(this DateTimeOffset source)
        {
            return (source.CompareTo(DateTimeOffset.MaxValue) == 0);
        }

        public static bool IsError(this DateTime source)
        {
            return (source.CompareTo(DateTime.MaxValue) == 0);
        }

        public static bool IsError(this TimeSpan source)
        {
            return (source.CompareTo(TimeSpan.MaxValue) == 0);
        }

        //timeZoneIdName - is the name of the time zone defined on the local computer
        //
        //var list = new List<string>();
        //ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
        //
        //typically installed are: - see https://www.C19isolate.org/TimeZones for list of TimeZones on Azure; timeZoneIdName is # ID: [IdName]
        //  "US Eastern Standard Time"
        //  "Pacific Standard Time"
        //  "GMT Standard Time"
        //  "Central European Standard Time"

        public static bool IsInvalidTimeSafe(this TimeZoneInfo source, DateTime time)
        {
            bool rc;

            switch (time.Kind) 
            {
                case DateTimeKind.Utc:
                    rc = false;
                    break;
                case DateTimeKind.Unspecified:
                    rc = source.IsInvalidTime(time);
                    break;
                // ReSharper disable once RedundantCaseLabel
                case DateTimeKind.Local:
                default:
                    rc = source.IsInvalidTime(new DateTime(time.Ticks, DateTimeKind.Unspecified));
                    break;
            }
            return rc;
        }
        public static DateTime ConvertUtcToLocalTime(this DateTime source, string timeZoneIdName)
        {
            var rc = DateTimeError;

            try
            {
                if ((string.IsNullOrEmpty(timeZoneIdName) == false) && (source.Kind == DateTimeKind.Utc))
                {
                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneIdName);
                    if (timeZone.IsDaylightSavingTime(source) == false)
                        rc = TimeZoneInfo.ConvertTime(source, timeZone);
                    else
                    {
                        DateTimeOffset offset = new DateTimeOffset(source);
                        rc = TimeZoneInfo.ConvertTime(offset, timeZone).DateTime;
                    }
                }
            }
            catch (Exception)
            {
                //ignore
            }
            return rc;
        }

        public static string ConvertUtcToLocalTimeString(this DateTime source, string format, string timeZoneIdName) 
        {
            var rc = "[Error]";

            try
            {
                if ((string.IsNullOrEmpty(timeZoneIdName) == false) && (string.IsNullOrEmpty(format) == false) && (source.Kind == DateTimeKind.Utc))
                {
                    var tim = source.ConvertUtcToLocalTime(timeZoneIdName);
                    if (tim.IsError() == false)
                        rc = tim.ToString(format);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return rc;
        }

        public static DateTime ConvertLocalTimeToUtc(this DateTime source, string timeZoneIdName) //"Central Standard Time"
        {
            var rc = DateTimeError;

            if (string.IsNullOrEmpty(timeZoneIdName) == false)
            {
                try
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneIdName);
                    if ((source.IsError() == false) && (timeZone != null))
                    {
                        var utc = TimeZoneInfo.ConvertTimeToUtc(source, timeZone);
                        if (utc.IsError() == false)
                            rc = utc;
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }
            return rc;
        }

        public static DateTime ConvertLocalTimeToUtcDateTime(this string source, string timeZoneIdName, string cultureName, string midnight = "midnight", string noon = "noon") //"Central Standard Time", "en-GB" 
        {
            var rc = DateTimeError;

            try
            {
                if ((string.IsNullOrEmpty(source) == false) && (string.IsNullOrEmpty(timeZoneIdName) == false) && (string.IsNullOrEmpty(cultureName) == false) && (string.IsNullOrEmpty(midnight) == false) && (string.IsNullOrEmpty(noon) == false))
                {
                    var text = source;
                    var index = text.IndexOf(noon, StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                        text = text.Substring(0, index) + "PM";
                    else
                    {
                        index = text.IndexOf(midnight, StringComparison.OrdinalIgnoreCase);
                        if (index >= 0)
                            text = text.Substring(0, index) + "AM";
                    }
                    if ((index < 0) || (text.Contains("12:00")))
                    {
                        if (DateTime.TryParse(text, CultureInfo.GetCultureInfo(cultureName), DateTimeStyles.None, out var local))
                        {
                            var utc = local.ConvertLocalTimeToUtc(timeZoneIdName);
                            if (utc.IsError() == false)
                                    rc = utc;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return rc;
        }

        public static string ToStringRemainingDaysHours(this TimeSpan source, string lessHour="less than an hour", string day="day", string days="days", string hour="hour", string hours="hours")
        {
            var rc = "[error]";

            if ((string.IsNullOrEmpty(lessHour) == false) && (string.IsNullOrEmpty(day) == false) && (string.IsNullOrEmpty(days) == false) && (string.IsNullOrEmpty(hour) == false) && (string.IsNullOrEmpty(hours) == false))
            {
                rc = "0";
                if (source.TotalMilliseconds > 1)
                {
                    var remHours = (source.Minutes >= 30) ? source.Hours + 1 : source.Hours;
                    var remDays = (remHours >= 12) ? source.Days + 1 : source.Days;

                    if (remDays < 1)
                        rc = (source.Hours < 1) ? lessHour : (remHours == 1) ? $"1 {hour}" : $"{remHours} {hours}"; //hours cannot be 23+1 as days == 0
                    else
                    {
                        rc = (remDays == 1) ? $"1 {day}" : $"{remDays} {days}";
                        if ((remHours > 0) && (remHours < 12))
                            rc += (remHours == 1) ? $" 1 {hour}" : (remHours > 23) ? $" 23 {hours}" : $" {remHours} {hours}";
                    }
                }
            }
            return rc;
        }
    }
}
