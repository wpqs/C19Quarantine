using System;

namespace C19QCalcLib
{
    public class CalcUk : ICalc
    {
        private readonly double TemperatureFever = 38.0;
        private readonly int QuarantinePeriodNoSymptoms = 14;
        private readonly int QuarantinePeriodWithSymptoms = 7;

        private readonly TimeSpan _quarantineSpanNoSymptoms = new TimeSpan(14,0,0,0);
        private readonly TimeSpan _quarantineSpanWithSymptoms = new TimeSpan(7, 0, 0, 0);
        private readonly Person _person;

        public CalcUk(Person person)
        {
            _person = person;
        }

        public bool IsSymptomatic(double temperature) { return (temperature >= TemperatureFever); }
        public int GetDaysInQuarantine(DateTime nowUtc)
        {
            var rc = -1;

            if (_person != null)
            {
                var symptomsUtc = _person.FirstSymptomsUtc;
                if ((symptomsUtc == null) && (IsSymptomatic(_person.Temperature)))
                    symptomsUtc = nowUtc;
                // ReSharper disable once ConstantNullCoalescingCondition
                var span =  (symptomsUtc == null) ? nowUtc - _person.QuarantineStartedUtc : nowUtc - (symptomsUtc ?? nowUtc);
                if (span.TotalSeconds >= 0)
                {
                    if (symptomsUtc == null) 
                        rc = (span.Days >= QuarantinePeriodNoSymptoms) ? 0 : QuarantinePeriodNoSymptoms - span.Days;
                    else
                    {
                        rc = (span.Days >= QuarantinePeriodWithSymptoms) ? (IsSymptomatic(_person.Temperature) ? 1 : 0) : QuarantinePeriodWithSymptoms - span.Days;
                    }
                }
            }
            return rc;
        }

        public TimeSpan GetSpanInQuarantine(DateTime nowUtc)
        {
            var rc = Extensions.TimeSpanError; //error

            if (_person != null)
            {
                var symptomsUtc = _person.FirstSymptomsUtc;
                if ((symptomsUtc == null) && (IsSymptomatic(_person.Temperature)))
                    symptomsUtc = nowUtc;
                // ReSharper disable once ConstantNullCoalescingCondition
                var span = (symptomsUtc == null) ? nowUtc - _person.QuarantineStartedUtc : nowUtc - (symptomsUtc ?? nowUtc);
                if (span.TotalSeconds >= 0)
                {
                    if (symptomsUtc == null)
                        rc = (span.Days >= QuarantinePeriodNoSymptoms) ? new TimeSpan(0) : _quarantineSpanNoSymptoms - span;
                    else
                    {
                        rc = (span.Days >= QuarantinePeriodWithSymptoms) ? ((IsSymptomatic(_person.Temperature)) ? new TimeSpan(1,0,0, 0) : new TimeSpan(0)) : _quarantineSpanWithSymptoms - span;
                    }
                }
            }
            return rc;
        }
    }
}
