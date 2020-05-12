using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C19QCalcLib
{
    public class TimeZones
    {
        public string Selected { get; set; }
        public List<SelectListItem> Items { get; set; }
        public string GetDefault() { return (Items.Count > 0) ? Items[0].Value : "[error]"; }

        public TimeZones()
        {
            Items = new List<SelectListItem>
            {
                new SelectListItem("Greenwich Mean Time", "GMT", (Selected == "GMT")),
                new SelectListItem("Central European Time", "CET", (Selected == "CET")),
                new SelectListItem("Indian Standard Time", "IST", (Selected == "IST"))
            };
            Selected = GetDefault();
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
