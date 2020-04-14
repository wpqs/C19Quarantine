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

        public static string ToStringDaysMinSec(this TimeSpan source, bool showZero=false)
        {
            // ReSharper disable once RedundantAssignment
            var rc = "[Error]";

            if (source.Days > 0)
                rc = (source.Days == 1) ? $"1 day " : $"{source.Days} days ";
            else
                rc  = (showZero) ? "0 days " : "";
            if (source.Hours > 0)
                rc += (source.Hours == 1) ? "1 hour " : $"{source.Hours} hours ";
            else
                rc += (showZero) ? "0 hours " : "";
            //if (source.Minutes > 0)
            //    rc += (source.Minutes == 1) ? "1 minute " : $"{source.Minutes} minutes ";
            //else
            //    rc += (showZero == true) ? "0 minutes " : "";
            //if (source.Seconds > 0)
            //     rc += (source.Seconds == 1) ? "1 second " : $"{source.Seconds} seconds ";
            //else
            //     rc += (showZero == true) ? "0 seconds " : "";

            if (string.IsNullOrEmpty(rc))
                rc = (source.TotalMilliseconds < 1) ? "zero" : "less than an hour";

            return rc.TrimEnd();
        }
    }
}
