using System;
using System.Collections.Generic;
using System.Linq;

namespace C19QCalcLib
{
    public class IndexForm : MxFormProc
    {
        public const string SampleDateTime = "14-04-20 10:45 AM";
        public const string DateTimeFormat = "dd-MM-yy h:mm tt";

        public const string HasSymptomsKey = "HasSymptoms";
        public const string StartIsolationKey = "StartIsolation";
        public const string StartSymptomsKey = "StartSymptoms";

        public bool HasSymptoms { get; private set; }
        public DateTime StartIsolation { get; private set; }
        public DateTime? StartSymptoms { get; private set; }
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

            HasSymptoms = false;
            StartIsolation = Extensions.DateTimeError;
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
            return ((StartIsolation.CompareTo(Extensions.DateTimeError) != 0) && (StartSymptoms?.CompareTo(Extensions.DateTimeError) ?? 1) != 0); 
        }

        public string ValidateHasSymptoms(string hasSymptoms)
        {
            string rc = null;

            HasSymptoms = false;

            if (string.IsNullOrEmpty(CultureName))
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
                        var isolation = startQuarantineLocal.ConvertLocalTimeToUtcDateTime(TimeZoneId, CultureName);
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
                        var symptoms = startSymptomsLocal.ConvertLocalTimeToUtcDateTime(TimeZoneId, CultureName);
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
