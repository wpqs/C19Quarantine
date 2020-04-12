﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace C19QCalcLib
{
    public class UiValidation
    {
        public const string SampleDateTime = "14-04-20 10:45 AM";
        public const string DateTimeFormat = "dd-MM-yy HH:mm tt";
        public const string DegreesCelsiusSymbol = "oC";
        public const double DegreesCelsiusMin = 24.0;
        public const double DegreesCelsiusMax = 43.0;

        public DateTime SelfIsolationTime { get; set; }
        public DateTime? SymptomsTime { get; set; }
        public double TemperatureValue { get; set; }

        public  string GetFormErrors(string startQuarantine, string startSymptoms, string temperature)
        {
            string rc = null;

            try
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, cstZone);

                if (string.IsNullOrEmpty(temperature) || (temperature.Contains(DegreesCelsiusSymbol) == false))
                    rc = $"User error 001: Temperature value is invalid. It is empty or doesn't end with {DegreesCelsiusSymbol}. Please try again with a correct value like 37.0 {DegreesCelsiusSymbol}";
                else
                {
                    var index = temperature.IndexOf(DegreesCelsiusSymbol, StringComparison.InvariantCulture);
                    if (double.TryParse(temperature.Substring(0, (index > 0) ? index : 0), out var temp) == false)
                        rc = $"User error 002: Temperature value {temperature} is invalid. It is not a valid number. Please try again with a correct value like 37.0 {DegreesCelsiusSymbol}";
                    else
                    {
                        if ((temp < DegreesCelsiusMin) || (temp > DegreesCelsiusMax))
                            rc = $"User error 003: Temperature value {temperature} is invalid. It is outside the range compatible with life. Please try again with a value in the range {DegreesCelsiusMin}-{DegreesCelsiusMax} {DegreesCelsiusSymbol}";
                        else
                        {
                            TemperatureValue = temp;
                            if (string.IsNullOrEmpty(startQuarantine))
                                rc = $"User error 004: Start self-isolation value is invalid. It is empty. Please try again with a correct value like {SampleDateTime}";
                            else
                            {
                                if (DateTime.TryParse(startQuarantine, CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var isolation) == false)
                                    rc = $"User error 005: Start self-isolation value {startQuarantine} is invalid. It is not a valid date, time. Please try again with a correct value like {SampleDateTime}";
                                else
                                {
                                    var span = now.DateTime - isolation;
                                    if (span.TotalSeconds < 0)
                                        rc = $"User error 006: Start self-isolation value {isolation.ToString(DateTimeFormat)} is invalid. It is after the current date, time. Please try again with value before {now.ToString(DateTimeFormat)}";
                                    else
                                    {
                                        SelfIsolationTime = isolation;
                                        if (string.IsNullOrEmpty(startSymptoms))
                                            SymptomsTime = null;
                                        else
                                        {
                                            if (DateTime.TryParse(startSymptoms, CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var symptoms) == false)
                                                rc = $"User error 007: Start symptoms value {startSymptoms} is invalid. It is not a valid date, time. Please try again with correct value like {SampleDateTime}";
                                            else
                                            {
                                                span = symptoms - SelfIsolationTime;
                                                if (span.TotalSeconds < 0)
                                                    rc = $"User error 008: Start symptoms value {startSymptoms} is invalid. It is before the start of your self-isolation. Please try again with value after {SelfIsolationTime.ToString(DateTimeFormat)}";
                                                else
                                                {
                                                    SymptomsTime = symptoms;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                rc = $"Program error 103: An internal error has been detected. {e.Message}. Please report this problem and try again";
            }
            return rc;
        }
    }
}
