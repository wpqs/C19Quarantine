using System;
using System.Globalization;

namespace C19QCalcLib
{
    public static class Extensions
    {
        public static readonly DateTime DateTimeError = DateTime.MaxValue;
        public static readonly TimeSpan TimeSpanError = TimeSpan.MaxValue;
        
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
        //typically installed are:
        //  "US Eastern Standard Time"
        //  "Pacific Standard Time"
        //  "GMT Standard Time"
        //  "Central European Standard Time"

        public static DateTime ConvertUtcToLocalTime(this DateTime source, string timeZoneIdName, bool ignoreDayLightSavings = false)
        {
            var rc = DateTimeError;

            try
            {
                if ((timeZoneIdName != null) && (source.Kind == DateTimeKind.Utc))
                {
                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneIdName);
                    if ((ignoreDayLightSavings) || (timeZone.IsDaylightSavingTime(source) == false))
                    {
                        rc = TimeZoneInfo.ConvertTime(source, timeZone);
                    }
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

        public static string ConvertUtcToLocalTime(this DateTime source, string format, string timeZoneIdName, bool ignoreDayLightSavings=false) //"Central Standard Time" 
        {
            var rc = "[Error]";

            try
            {
                if ((timeZoneIdName != null) && (format != null) && (source.Kind == DateTimeKind.Utc))
                {
                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneIdName);
                    if ((ignoreDayLightSavings) || (timeZone.IsDaylightSavingTime(source) == false))
                    {
                        var now = TimeZoneInfo.ConvertTime(source, timeZone);
                        rc = now.ToString(format);
                    }
                    else
                    {
                        DateTimeOffset offset = new DateTimeOffset(source);
                        var now = TimeZoneInfo.ConvertTime(offset, timeZone);
                        rc = now.ToString(format);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return rc;
        }

        public static DateTime ConvertLocalTimeToUtc(this string source, string timeZoneIdName, string cultureName, bool ignoreDayLightSavings = false) //"Central Standard Time", "en-GB" 
        {
            DateTime rc = DateTimeError;

            try
            {
                if ((string.IsNullOrEmpty(source) == false) && (timeZoneIdName != null) && (cultureName != null))
                {
                    var text = source;
                    var index = text.IndexOf("noon", StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                        text = text.Substring(0, index) + "PM";
                    else
                    {
                        index = text.IndexOf("midnight", StringComparison.OrdinalIgnoreCase);
                        if (index >= 0)
                            text = text.Substring(0, index) + "AM";
                    }
                    if ((index < 0) || (text.Contains("12:00")))
                    {
                        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneIdName);
                        if (DateTime.TryParse(text, CultureInfo.GetCultureInfo(cultureName), DateTimeStyles.None, out var time))
                        {
                            if ((timeZone.IsDaylightSavingTime(time)) && (ignoreDayLightSavings))
                            {
                                var adjustmentRules = timeZone.GetAdjustmentRules();
                                if (adjustmentRules.Length > 0)
                                    time += adjustmentRules[0].DaylightDelta;
                            }
                            rc = TimeZoneInfo.ConvertTimeToUtc(time, timeZone);
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

        public static string ToStringDaysHours(this TimeSpan source)
        {
            var rc = "0 minutes";

            if (source.TotalMilliseconds > 1)
            {
                var hours = (source.Minutes >= 30) ? source.Hours + 1 : source.Hours;
                var days = (hours >= 12) ? source.Days+1 : source.Days;

                if (days < 1)
                    rc = (source.Hours < 1) ? "less than an hour" : (hours == 1) ? "1 hour" : $"{hours} hours"; //hours cannot be 23+1 as days == 0
                else
                {
                    rc = (days == 1) ? $"1 day" : $"{days} days";
                    if ((hours > 0) && (hours < 12))
                        rc += (hours == 1) ? " 1 hour" : (hours > 23) ? "23 hours" : $" {hours} hours";
                }
            }
            return rc;
        }
    }
}
