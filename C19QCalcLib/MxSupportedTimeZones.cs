using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C19QCalcLib
{
    public abstract class MxSupportedTimeZones
    {
        public abstract string GetDefaultTimeZoneAcronym();
        public abstract bool GetDefaultDaylightSavingAuto();

        public const string CookieName = ".AspNetCore.MxTimeZone";
        public const int    CookieExpiryDays = 3;

        public const string DaylightSavingAutoApplyYes = "yes"; //daylight saving is applied when time is in daylight saving period, otherwise it's not - typically the default
        public const string DaylightSavingAutoApplyNo = "no";   //use if you have say a GMT clock to which daylight saving is never applied  
        
        public const string CookieTimeZoneKey = "tz=";
        public const string CookieDsAutoKey = "dsa=";
        public const string CookieKeySeparator = "|";
        public const string QueryTimeZoneKey = "timezone=";
        public const string QueryDsAutoKey = "ds-auto=";
        public const string QueryKeySeparator = "&";

        public string Selected { get; set; }
        public List<SelectListItem> Items { get; set; }

        public string[] GetSupportedTimeZones()
        {
            var list = new List<string>();
            foreach (var timezone in Items)
            {
                list.Add(timezone.Value);
            }
            return list.ToArray();
        }

        public bool IsSupported(string timeZoneAcronym)
        {
            return (string.IsNullOrEmpty(timeZoneAcronym) == false) && (Items?.Find(a => a.Value == timeZoneAcronym) != null);
        }

        public bool SetTimeZoneCookie(IHttpContextAccessor httpContextAccessor, string timeZoneAcronym, bool daylightSavingAutoApply)
        {
            return MxCookies.SetValue(httpContextAccessor, CookieName, GetTimeZoneValueToSet(timeZoneAcronym, daylightSavingAutoApply), MxSupportedTimeZones.CookieExpiryDays, true);
        }

        public string GetTimeZoneAcronymFromCookie(IHttpContextAccessor httpContextAccessor)
        {
            return GetTimeZoneAcronymFromValue(MxCookies.GetValue(httpContextAccessor, CookieName));
        }

        public bool IsDaylightSavingAuto(IHttpContextAccessor httpContextAccessor)
        {
            var rc = true;
            var value = MxCookies.GetValue(httpContextAccessor, CookieName);
            if (value != MxCookies.ErrorValue)
                rc = (GetDaylightSavingAutoFromValue(value) != DaylightSavingAutoApplyNo);
            return rc;
        }

        public string GetTimeZoneValueToSet(string timeZoneAcronym, bool daylightSavingAutoApply = true, bool getCookieValue = true)
        {

            var timeZoneKey = (getCookieValue) ? CookieTimeZoneKey : QueryTimeZoneKey;
            var keySeparator = (getCookieValue) ? CookieKeySeparator : QueryKeySeparator;
            var dsAutoKey = (getCookieValue) ? CookieDsAutoKey : QueryDsAutoKey;

            var rc = $"{timeZoneKey}{GetDefaultTimeZoneAcronym()}{keySeparator}{dsAutoKey}{GetDaylightSavingAutoValue(GetDefaultDaylightSavingAuto())}";

            if (IsSupported(timeZoneAcronym))
                rc = $"{timeZoneKey}{timeZoneAcronym ?? GetDefaultTimeZoneAcronym()}{keySeparator}{dsAutoKey}{GetDaylightSavingAutoValue(daylightSavingAutoApply)}";

            return rc;
        }

        public string GetTimeZoneAcronymFromValue(string value, bool valueIsCookie = true)
        {
            var rc = GetDefaultTimeZoneAcronym();

            var timeZoneKey = (valueIsCookie) ? CookieTimeZoneKey : QueryTimeZoneKey;
            var keySeparator = (valueIsCookie) ? CookieKeySeparator : QueryKeySeparator;

            if (string.IsNullOrEmpty(value) == false)
            {
                var start = value.IndexOf(timeZoneKey, StringComparison.Ordinal);
                var end = value.IndexOf(keySeparator, StringComparison.Ordinal);
                if ((start == 0) && (end != -1) && ((end - timeZoneKey.Length) > 0))
                {
                    var timeZoneAcronym = value.Substring(timeZoneKey.Length, end - timeZoneKey.Length);
                    if (IsSupported(timeZoneAcronym))
                        rc = timeZoneAcronym;
                }
            }
            return rc;
        }

        public string GetDaylightSavingAutoFromValue(string value, bool valueIsCookie = true)
        {
            var rc = GetDefaultDaylightSavingAuto() ? DaylightSavingAutoApplyYes : DaylightSavingAutoApplyNo;

            var dsAutoKey = (valueIsCookie) ? CookieDsAutoKey : QueryDsAutoKey;
            var keySeparator = (valueIsCookie) ? CookieKeySeparator : QueryKeySeparator;

            if (string.IsNullOrEmpty(value) == false)
            {
                var key = keySeparator + dsAutoKey;
                var start = value.IndexOf(key, StringComparison.Ordinal);
                if ((start != -1) && ((start + key.Length) < (value.Length - 1)))
                    rc = value.Substring(start + key.Length);
            }
            return rc;
        }
        
        private string GetDaylightSavingAutoValue(bool value)                                
        {
            return (value) ? DaylightSavingAutoApplyYes : DaylightSavingAutoApplyNo;
        }

        public static string GetReport(string startOfLine, string endOfLine, string tab, string endOfRecord, string timeZoneId = null)
        {
            var rc = "[Error]";

            if ((startOfLine != null) && (endOfLine != null) && (tab != null) && (endOfRecord != null))
            {
                var timeZoneIdNum = 1;
                try
                {
                    DateTimeFormatInfo dateFormats = CultureInfo.CurrentCulture.DateTimeFormat;
                    ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

                    if (timeZones.Count == 0)
                        rc = startOfLine + "No time zones found on this system" + endOfLine;
                    else
                    {
                        rc = startOfLine + $"{timeZones.Count} time zones found on this system. Report on {(timeZoneId ?? "all")}" + endOfLine;

                        foreach (TimeZoneInfo timeZone in timeZones)
                        {
                            if (timeZoneId?.Equals(timeZone.Id, StringComparison.OrdinalIgnoreCase) ?? true)
                            {
                                rc += startOfLine + $"{timeZoneIdNum++} ID: {timeZone.Id}" + endOfLine;
                                rc += startOfLine + $"{tab}Display Name: {timeZone.DisplayName}" + endOfLine;
                                rc += startOfLine + $"{tab}Standard Name: {timeZone.StandardName}" + endOfLine;
                                rc += startOfLine + $"{tab}Daylight Name: {timeZone.DaylightName} {(timeZone.SupportsDaylightSavingTime ? "(supported)" : "(not supported)")}" + endOfLine;
                                rc += startOfLine + $"{tab}Offset from UTC: {timeZone.BaseUtcOffset.Hours} hours, {timeZone.BaseUtcOffset.Minutes} minutes" + endOfLine;

                                var adjustRules = timeZone.GetAdjustmentRules();
                                rc += startOfLine + $"{tab}Number of adjustment rules: {adjustRules.Length}" + endOfLine;

                                var ruleNum = 0;
                                foreach (var rule in adjustRules)
                                {
                                    TimeZoneInfo.TransitionTime transTimeStart = rule.DaylightTransitionStart;
                                    TimeZoneInfo.TransitionTime transTimeEnd = rule.DaylightTransitionEnd;

                                    rc += startOfLine + $"{tab}Rule {ruleNum++}: {((transTimeStart.IsFixedDateRule) ? "Fixed date" : "Not fixed date")}" + endOfLine;

                                    rc += startOfLine + $"{tab}{tab}From {rule.DateStart} to {rule.DateEnd}" + endOfLine;
                                    rc += startOfLine + $"{tab}{tab}Delta: {rule.DaylightDelta}" + endOfLine;
                                    if (transTimeStart.IsFixedDateRule)
                                    {
                                        rc += startOfLine + $"{tab}{tab}{tab}Begins at {transTimeStart.TimeOfDay:t} on {transTimeStart.Day} {dateFormats.MonthNames[transTimeStart.Month - 1]}" + endOfLine;
                                        rc += startOfLine + $"{tab}{tab}{tab}Ends at {transTimeEnd.TimeOfDay:t} on {transTimeEnd.Day} {dateFormats.MonthNames[transTimeEnd.Month - 1]}" + endOfLine;
                                    }
                                    else
                                    {
                                        rc += startOfLine + $"{tab}{tab}{tab}Begins at {transTimeStart.TimeOfDay:t} on {transTimeStart.DayOfWeek} of week {transTimeStart.Week} of {dateFormats.MonthNames[transTimeStart.Month - 1]}" + endOfLine;
                                        rc += startOfLine + $"{tab}{tab}{tab}Ends at {transTimeEnd.TimeOfDay:t} on {transTimeEnd.DayOfWeek} of week {transTimeEnd.Week} of {dateFormats.MonthNames[transTimeEnd.Month - 1]}" + endOfLine;
                                    }
                                }
                                if (timeZoneIdNum < timeZones.Count)
                                    rc += endOfRecord;
                            }
                        }
                    }

                    rc += startOfLine + "Report ends" + endOfLine;
                }
                catch (Exception e)
                {
                    rc = startOfLine + $"Program Error: ID={timeZoneIdNum}: {e.Message}" + endOfLine;
                }
            }
            return rc;
        }
    }
}