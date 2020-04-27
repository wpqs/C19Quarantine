using System;
using System.Collections.Generic;
using System.Linq;

namespace C19QCalcLib
{
    public class IndexForm : MxFormProc
    {
        public const string SampleDateTime = "14-04-20 10:45 AM";
        public const string DateTimeFormat = "dd-MM-yy h:mm tt";

        public const double DegreesCelsiusMin = 24.0;
        public const double DegreesCelsiusMax = 43.0;
        public const double DegreesCelsiusInvalid= 0.0;

        public const string TemperatureKey = "Temperature";
        public const string StartIsolationKey = "StartIsolation";
        public const string StartSymptomsKey = "StartSymptoms";

        public DateTime StartIsolation { get; private set; }
        public DateTime? StartSymptoms { get; private set; }
        public double Temperature { get; private set; }

        public string TimeZoneId { get; private set;  }
        public string CultureName { get; private set; }

        public DateTime NowUtc { get; private set; }
        public DateTime NowLocal { get; private set; }

        
        public IndexForm(string timeZoneIdName, string cultureName) 
        {
            TimeZoneId = timeZoneIdName;
            CultureName = cultureName;
            NowUtc = DateTime.UtcNow;
            NowLocal = NowUtc.ConvertUtcToLocalTime(timeZoneIdName);

            StartIsolation = Extensions.DateTimeError;
            Temperature = DegreesCelsiusInvalid;
            StartSymptoms = null;
        }
        
        public override Dictionary<string, string> Validate(KeyValuePair<string, object>[] props)
        {
            Dictionary<string, string> rc = null;
            
            Errors.Clear();

            if (ValidatePropKeys(props))
            {
                var error = ValidateTemperature((string)props.FirstOrDefault(x => x.Key == TemperatureKey).Value);
                if (error != null)
                    AddError(TemperatureKey, error);
                error = ValidateStartIsolation((string)props.FirstOrDefault(x => x.Key == StartIsolationKey).Value);
                if (error != null)
                    AddError(StartIsolationKey, error);
                error = ValidateStartSymptoms((string)props.FirstOrDefault(x => x.Key == StartSymptomsKey).Value);
                if (error != null)
                    AddError(StartSymptomsKey, error);
                rc = Errors;
            }
            return rc;
        }

        protected override bool ValidatePropKeys(KeyValuePair<string, object>[] props)
        {
            var rc = false;

            if (base.ValidatePropKeys(props) && (props?.Length == 4))
            {
                if (Keys.Contains(TemperatureKey) && Keys.Contains(StartIsolationKey) && Keys.Contains(StartSymptomsKey))
                    rc = true;
            }
            return rc;
        }

        public override bool IsValid() 
        { 
            return ((Temperature.CompareTo(DegreesCelsiusInvalid) != 0) && (StartIsolation.CompareTo(Extensions.DateTimeError) != 0) && (StartSymptoms?.CompareTo(Extensions.DateTimeError) ?? 1) != 0); 
        }
        public string ValidateTemperature(string temperature)
        {
            string rc = null;

            Temperature = DegreesCelsiusInvalid;

            if (string.IsNullOrEmpty(TimeZoneId) || (string.IsNullOrEmpty(CultureName)))
                rc = $"{ProgramErrorMsg} 200. Please report this problem.";
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(temperature))
                        rc = $"This value is required";
                    else
                    {
                        if (double.TryParse(temperature.Trim(), out var bodyTemp) == false)
                            rc = $"Please try again with a valid number like 37.0";
                        else
                        {
                            if ((bodyTemp < DegreesCelsiusMin) || (bodyTemp > DegreesCelsiusMax))
                                rc = $"Please enter a temperature in the range {DegreesCelsiusMin}-{DegreesCelsiusMax}";
                            else
                            {
                                Temperature = bodyTemp;  //no errors
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc = $"{ProgramErrorMsg} 201. {e.Message}";
                }
            }
            return rc;
        }

        public string ValidateStartIsolation(string startQuarantineLocal)
        {
            string rc = null;

            StartIsolation = Extensions.DateTimeError;

            if (string.IsNullOrEmpty(TimeZoneId) || (string.IsNullOrEmpty(CultureName)))
                rc = $"{ProgramErrorMsg} 210. Please report this problem.";
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(startQuarantineLocal))
                        rc = $"This value is required";
                    else
                    {
                        var isolation = startQuarantineLocal.ConvertLocalTimeToUtc(TimeZoneId, CultureName);
                        if (isolation.IsError())
                            rc = $"Please try again with a valid date/time like {SampleDateTime}";
                        else
                        {
                            var span = NowUtc - isolation;
                            if (span.TotalSeconds < 0)
                                rc = $"This value is after the current time. Please try again with value before {NowLocal.ToString(DateTimeFormat)}";
                            else
                            {
                                StartIsolation = isolation;  //no errors
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc = $"{ProgramErrorMsg} 211. {e.Message}";
                }
            }
            return rc;
        }

        public string ValidateStartSymptoms(string startSymptomsLocal)
        {
            string rc = null;

            StartSymptoms = Extensions.DateTimeError;

            if (string.IsNullOrEmpty(TimeZoneId) || (string.IsNullOrEmpty(CultureName)))
                rc = $"{ProgramErrorMsg} 220. Please report this problem.";
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(startSymptomsLocal))
                        StartSymptoms = null;  //Ok, as value is optional
                    else
                    {
                        var symptoms = startSymptomsLocal.ConvertLocalTimeToUtc(TimeZoneId, CultureName);
                        if (symptoms.IsError())
                            rc = $"Please try again with a valid date/time like {SampleDateTime}";
                        else
                        {
                            var span = NowUtc - symptoms;
                            if (span.TotalSeconds < 0)
                                rc = $"This value is after the current time. Please try again with value before {NowLocal.ToString(DateTimeFormat)}";
                            else
                            {
                                if (StartIsolation.CompareTo(Extensions.DateTimeError) == 0)
                                    rc = $"You must set the start of your self-isolation before giving the start of your symptoms";
                                else
                                {
                                    span = symptoms - StartIsolation;
                                    if (span.TotalSeconds < 0)
                                        rc = $"This value is before the start of your self-isolation. If your symptoms started before you entered self-isolation then enter {StartIsolation.ConvertUtcToLocalTime(TimeZoneId).ToString(DateTimeFormat)} as the start of your symptoms and try again";
                                    else
                                    {
                                        StartSymptoms = symptoms;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc = $"{ProgramErrorMsg} 221. {e.Message}";
                }
            }
            return rc;
        }
    }
}
