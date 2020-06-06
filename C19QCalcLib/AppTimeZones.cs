using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C19QCalcLib
{
    public class AppTimeZones
    {
        public const string CookieTzName = "C19TimeZoneSetting";
        public const string CookieWithoutDaylightSaving = "C19WithoutDaylightSaving";
        public const string CookieWithoutDaylightSavingValueYes = "yes";
        public const string CookieWithoutDaylightSavingValueNo = "no";
        public const int CookieExpiryDays = 3;

        public const string AcronymGmt = "GMT";
        public const string AcronymCet = "CET";

        public const string DefaultTzDbName = "Europe/London";
        public const string DefaultAcronym = AcronymGmt;

        public string Selected { get; set; }
        public List<SelectListItem> Items { get; set; }
        public AppTimeZones()
        {
            Items = new List<SelectListItem>
            {
                new SelectListItem("Greenwich Mean Time", AcronymGmt, (Selected == AcronymGmt)), 
                new SelectListItem("Central European Time", AcronymCet, (Selected == AcronymCet)) 
            };
            Selected = DefaultAcronym;
        }

        public static string GetDaylightSavingName(string zoneAcronym)
        {
            var rc = "British Summer Time";
            if (string.IsNullOrEmpty(zoneAcronym) == false)
            {
                if (zoneAcronym == AcronymCet)
                    rc = "Central Europe Summer Time";
            }
            return rc;
        }

        public static string GetDaylightSavingAcronym(string zoneAcronym)
        {
            var rc = "BST";
            if (string.IsNullOrEmpty(zoneAcronym) == false)
            {
                if (zoneAcronym == AcronymCet)
                    rc = "CEST";
            }
            return rc;
        }
        public static string GetTzDbName(string zoneAcronym)
        {
            var rc = DefaultTzDbName;
            if (string.IsNullOrEmpty(zoneAcronym) == false)
            {
                if (zoneAcronym == AcronymCet)
                    rc = "Europe/Paris";
            }
            return rc;
        }

        public static string GetReport(string startOfLine, string endOfLine, string tab, string endOfRecord, string timeZoneId=null)
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
