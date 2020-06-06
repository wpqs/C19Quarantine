using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace C19QCalcLib
{
    public class IndexFormProc : MxFormProc
    {
        public const string SampleDateTime = "14-04-2020 10:45 AM";
        public const string DateTimeFormat = "dd-MM-yy h:mm tt";

        public const string HasSymptomsKey = "HasSymptoms";
        public const string StartIsolationKey = "StartIsolation";
        public const string StartSymptomsKey = "StartSymptoms";

        public bool HasSymptoms { get; private set; }
        public Instant StartIsolation { get; private set; }
        public Instant? StartSymptoms { get; private set; }
        public string TzDbName { get; private set;  }
        public string CultureTag { get; private set; }
        public bool WithoutDaylightSavings { get; private set; }
        public Instant NowInstance { get; private set; }

        public IndexFormProc(IClock clock, string tzDbNameName, string cultureTag, bool withoutDaylightSavings)
        {
            TzDbName = tzDbNameName;
            CultureTag = cultureTag;
            WithoutDaylightSavings = withoutDaylightSavings;

            NowInstance = clock?.GetCurrentInstant() ?? ExtNodatime.InstantError;

            HasSymptoms = false;
            StartIsolation = ExtNodatime.InstantError;
            StartSymptoms = null;
        }
        
        public override Dictionary<string, string> Validate(KeyValuePair<string, object>[] props)
        {
            Dictionary<string, string> rc = null;
            
            Errors.Clear();

            if (ValidatePropKeys(props))
            {
                var error = ValidateHasSymptoms((string)props.FirstOrDefault(x => x.Key == HasSymptomsKey).Value);
                if (error != null)
                    AddError(HasSymptomsKey, error);
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
                if (Keys.Contains(HasSymptomsKey) && Keys.Contains(StartIsolationKey) && Keys.Contains(StartSymptomsKey))
                    rc = true;
            }
            return rc;
        }

        public override bool IsValid() 
        { 
            return ((StartIsolation.CompareTo(ExtNodatime.InstantError) != 0) && (StartSymptoms?.CompareTo(ExtNodatime.InstantError) ?? 1) != 0); 
        }

        public string ValidateHasSymptoms(string hasSymptoms)
        {
            string rc = null;

            HasSymptoms = false;

            if (string.IsNullOrEmpty(CultureTag))
                rc = $"{ProgramErrorMsg} 110. Please report this problem.";
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(hasSymptoms))
                        rc = $"This value is required";
                    else
                    {
                        if ((hasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase) == false) && (hasSymptoms.Equals("no", StringComparison.OrdinalIgnoreCase) == false))
                            rc = $"Please enter either 'yes' or 'no'";
                        else
                            HasSymptoms = (hasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase));
                    }
                }
                catch (Exception e)
                {
                    rc = $"{ProgramErrorMsg} 111. {e.Message}";
                }
            }
            return rc;
        }

        public string ValidateStartIsolation(string startQuarantineLocal)
        {
            string rc = null;

            StartIsolation = ExtNodatime.InstantError;

            if (string.IsNullOrEmpty(TzDbName) || (string.IsNullOrEmpty(CultureTag)))
                rc = $"{ProgramErrorMsg} 210. Please report this problem.";
            else
            {
                try
                {
                    DateTimeZone zone = DateTimeZoneProviders.Tzdb[TzDbName];
                    if (string.IsNullOrEmpty(startQuarantineLocal))
                        rc = $"This value is required";
                    else
                    {
                        if (startQuarantineLocal.ParseDateTime(zone, WithoutDaylightSavings, CultureTag, MxCultureInfo.FormatType.DateTime, false, out var isolation) == false)
                            rc = $"Please try again with a valid date/time like {SampleDateTime}";
                        else
                        {
                            var span = NowInstance - isolation;
                            if (span.TotalSeconds < 0)
                                rc = $"This value is after the current time. Please try again with value before {NowInstance.ToString(CultureTag, zone)}";
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

            StartSymptoms = ExtNodatime.InstantError;

            if (string.IsNullOrEmpty(TzDbName) || (string.IsNullOrEmpty(CultureTag)))
                rc = $"{ProgramErrorMsg} 220. Please report this problem.";
            else
            {
                try
                {
                    DateTimeZone zone = DateTimeZoneProviders.Tzdb[TzDbName];
                    if (string.IsNullOrEmpty(startSymptomsLocal))
                        StartSymptoms = null;  //Ok, as value is optional
                    else
                    {
                        if (startSymptomsLocal.ParseDateTime(zone, WithoutDaylightSavings, CultureTag, MxCultureInfo.FormatType.DateTime, false, out var symptoms) == false)
                            rc = $"Please try again with a valid date/time like {SampleDateTime}";
                        else
                        {
                            var span = NowInstance - symptoms;
                            if (span.TotalSeconds < 0)
                                rc = "This value is after the current time";
                            else
                            {
                                if (StartIsolation.IsError())
                                    rc = $"You must set the start of your self-isolation before giving the start of your symptoms";
                                else
                                {
                                    span = symptoms - StartIsolation;
                                    if (span.TotalSeconds < 0)
                                        rc = $"This value is before the start of your self-isolation. If your symptoms started before you entered self-isolation then enter {StartIsolation.ToString(CultureTag, zone)} as the start of your symptoms and try again";
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
