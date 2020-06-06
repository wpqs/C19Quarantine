using System;
using System.Diagnostics.CodeAnalysis;
using NodaTime;
using NodaTime.Text;

namespace C19QCalcLib
{
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public static class ExtNodatime
    {
        public static readonly Instant InstantError = Instant.MaxValue;
        public static readonly Duration DurationError = Duration.MaxValue;

        public static bool IsError(this Instant source)
        {
            return (source.CompareTo(InstantError) == 0);
        }

        public static bool IsError(this Duration source)
        {
            return (source.CompareTo(DurationError) == 0);
        }

        public static bool IsDaylightSavingsTime(this DateTimeZone zone, Instant instant)
        {
            return zone.GetZoneInterval(instant).Savings != Offset.Zero;
        }

        public static string ToString(this Instant instant, string cultureName, DateTimeZone zone, bool withoutDaylightSaving = false, MxCultureInfo.FormatType formatType = MxCultureInfo.FormatType.DateTime, bool longFormat = false)
        {
            var rc = "[error]";

            if ((zone != null) && (String.IsNullOrEmpty(cultureName) == false))
            {
                try
                {
                    var culture = MxCultureInfo.Instance.GetCultureInfo(cultureName);
                    if (culture != null)
                    {
                        var local = instant.InZone(zone).LocalDateTime;
                        if (withoutDaylightSaving && zone.IsDaylightSavingsTime(instant))
                            local = local.Minus(Period.FromSeconds(zone.GetZoneInterval(instant).Savings.Seconds));

                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        zone.AtStrictly(local); //throws exception if ambiguous or invalid due to daylight saving transition

                        //In all formats except Date the setting of longFormat (capital letter) causes display of hours, minutes and seconds in 24 hour clock

                        //FormatType.Date:      en-GB: D=Sunday, 29 March 2020 d=29-03-2020 
                        //FormatType.Time:      en-GB: T=00:59:59 t=12:59 AM 
                        //FormatType.DateTime:  en-GB: G=29-03-2020 00:59:59 g=29-03-2020 12:59 AM
                        //FormatType.Verbose:   en-GB: F=Sunday, 29 March 2020 00:59:59 f=Sunday, 29 March 2020 12:59 AM 
                        //FormatType.Machine:   *:     r=2020-03-29T00:59:59.000000000(ISO) s=2020-03-29T00 59:59 

                        if (formatType == MxCultureInfo.FormatType.Date)
                            rc = local.Date.ToString(MxCultureInfo.GetFormatSpecifier(formatType, longFormat), culture);
                        else if (formatType == MxCultureInfo.FormatType.Time)
                            rc = local.TimeOfDay.ToString(MxCultureInfo.GetFormatSpecifier(formatType, longFormat), culture);
                        else
                            rc = local.ToString(MxCultureInfo.GetFormatSpecifier(formatType, longFormat), culture);
                    }
                }
                catch (SkippedTimeException)
                {
                    rc = "[error: invalid time]"; 
                }
                catch (AmbiguousTimeException)
                {
                    rc = "[error: ambiguous time]";
                }
                catch (Exception e)
                {
                    rc = e.Message;
                }
            }
            return rc;
        }

        public static bool ParseDateTime(this string text, DateTimeZone zone, bool withoutDaylightSaving, string cultureName, MxCultureInfo.FormatType formatType, bool longFormat, out Instant result)
        {
            var rc = false;
            result = InstantError;

            if ((zone != null) && (String.IsNullOrEmpty(cultureName) == false) && (formatType != MxCultureInfo.FormatType.Date) && (formatType != MxCultureInfo.FormatType.Time))
            {
                try
                {
                    var culture = MxCultureInfo.Instance.GetCultureInfo(cultureName);
                    if (culture != null)
                    {
                        var parseResult = LocalDateTimePattern.Create(MxCultureInfo.GetFormatSpecifier(formatType, longFormat), MxCultureInfo.Instance.GetCultureInfo(cultureName)).Parse(text);
                        //var temp = parseResult.Value;
                        if (parseResult.Success)
                        {
                            var instant = parseResult.Value.InZoneStrictly(zone).ToInstant();
                            var local = instant.InZone(zone).LocalDateTime;
                            if (withoutDaylightSaving && zone.IsDaylightSavingsTime(instant))
                                local = local.Plus(Period.FromSeconds(zone.GetZoneInterval(instant).Savings.Seconds));

                            result = local.InZoneStrictly(zone).ToInstant();
                            rc = true;
                        }
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }
            return rc;
        }

        public static bool ParseTime(this string text, DateTimeZone zone, bool withoutDaylightSaving, string cultureName, Instant givenDate, bool longFormat, out Instant result)
        {
            var rc = false;
            result = InstantError;

            if ((zone != null) && (String.IsNullOrEmpty(cultureName) == false))
            {
                try
                {
                    var culture = MxCultureInfo.Instance.GetCultureInfo(cultureName);
                    if (culture != null)
                    {
                        var parseResult = LocalTimePattern.Create(MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Time, longFormat), MxCultureInfo.Instance.GetCultureInfo(cultureName)).Parse(text);
                        if (parseResult.Success)
                        {
                            var localDate = givenDate.InZone(zone).LocalDateTime.Date;
                            var instant = parseResult.Value.On(localDate).InZoneStrictly(zone).ToInstant();
                            var local = instant.InZone(zone).LocalDateTime;
                            if (withoutDaylightSaving && zone.IsDaylightSavingsTime(instant))
                                local = local.PlusSeconds(zone.GetZoneInterval(instant).Savings.Seconds);
                            result = local.InZoneStrictly(zone).ToInstant();
                            rc = true;
                        }
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }
            return rc;
        }

        public static bool ParseDate(this string text, DateTimeZone zone, string cultureName, bool longFormat, out Instant result)
        {
            var rc = false;
            result = InstantError;

            if ((zone != null) && (String.IsNullOrEmpty(cultureName) == false)  )
            {
                try
                {
                    var culture = MxCultureInfo.Instance.GetCultureInfo(cultureName);
                    if (culture != null)
                    {
                        var parseResult = LocalDatePattern.Create(MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Date, longFormat), MxCultureInfo.Instance.GetCultureInfo(cultureName)).Parse(text);
                        if (parseResult.Success)
                        {
                            var instant = parseResult.Value.AtMidnight().InZoneStrictly(zone).ToInstant();
                            var local = instant.InZone(zone).LocalDateTime;
                            if (zone.IsDaylightSavingsTime(instant))
                                local = local.PlusSeconds(zone.GetZoneInterval(instant).Savings.Seconds);
                            result = local.InZoneStrictly(zone).ToInstant();
                            rc = true;
                        }
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }
            return rc;
        }

        public static string ToStringRemainingTime(this Duration source, string lessHour = "less than an hour", string day = "day", string days = "days", string hour = "hour", string hours = "hours")
        {
            var rc = "[error]";

            if ((String.IsNullOrEmpty(lessHour) == false) && (String.IsNullOrEmpty(day) == false) && (String.IsNullOrEmpty(days) == false) && (String.IsNullOrEmpty(hour) == false) && (String.IsNullOrEmpty(hours) == false))
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
