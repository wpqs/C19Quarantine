using System;

namespace C19QCalcLib
{
    public class CalcUk
    {
        public readonly int QuarantinePeriodNoSymptoms = 14;
        public readonly int QuarantinePeriodWithSymptoms = 7;
        public readonly TimeSpan QuarantineSpanNoSymptoms = new TimeSpan(14,0,0,0);
        public readonly TimeSpan QuarantineSpanWithSymptoms = new TimeSpan(7, 0, 0, 0);
        public readonly double TemperatureFever = 38.0;

        private Person _person;

        public CalcUk(Person person)
        {
            _person = person;
        }
        public int GetDaysInQuarantine(DateTime nowUtc)
        {
            var rc = -1;

            if (_person != null)
            {
                var symptomsUtc = _person.FirstSymptomsUtc;
                if ((symptomsUtc == null) && (_person.Temperature >= TemperatureFever))
                    symptomsUtc = nowUtc;
                // ReSharper disable once ConstantNullCoalescingCondition
                var span =  (symptomsUtc == null) ? nowUtc - _person.QuarantineStartedUtc : nowUtc - (symptomsUtc ?? nowUtc);
                if (span.TotalSeconds >= 0)
                {
                    if (symptomsUtc == null) 
                        rc = (span.Days >= QuarantinePeriodNoSymptoms) ? 0 : QuarantinePeriodNoSymptoms - span.Days;
                    else
                    {
                        rc = (span.Days >= QuarantinePeriodWithSymptoms) ? ((_person.Temperature >= TemperatureFever) ? 1 : 0) : QuarantinePeriodWithSymptoms - span.Days;
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
                if ((symptomsUtc == null) && (_person.Temperature >= TemperatureFever))
                    symptomsUtc = nowUtc;
                // ReSharper disable once ConstantNullCoalescingCondition
                var span = (symptomsUtc == null) ? nowUtc - _person.QuarantineStartedUtc : nowUtc - (symptomsUtc ?? nowUtc);
                if (span.TotalSeconds >= 0)
                {
                    if (symptomsUtc == null)
                        rc = (span.Days >= QuarantinePeriodNoSymptoms) ? new TimeSpan(0) : QuarantineSpanNoSymptoms - span;
                    else
                    {
                        rc = (span.Days >= QuarantinePeriodWithSymptoms) ? ((_person.Temperature >= TemperatureFever) ? new TimeSpan(1,0,0, 0) : new TimeSpan(0)) : QuarantineSpanWithSymptoms - span;
                    }
                }
            }
            return rc;
        }
    }
}
