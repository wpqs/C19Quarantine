using System;

namespace C19QCalcLib
{
    public class CalcUk
    {
        public readonly int QuarantinePeriodNoSymptoms = 14;
        public readonly int QuarantinePeriodWithSymptoms = 7;
        public readonly double TemperatureFever = 38.0;

        private Person _person;

        public CalcUk(Person person)
        {
            _person = person;
        }
        public int GetDaysInQuarantine(DateTime now)
        {
            var rc = -1;

            if (_person != null)
            {
                var symptoms = _person.FirstSymptoms;
                if ((symptoms == null) && (_person.Temperature >= TemperatureFever))
                    symptoms = now;
                var span =  (symptoms == null) ? now - _person.QuarantineStarted : now - (symptoms ?? now);
                if (span.TotalSeconds >= 0)
                {
                    if (symptoms == null) 
                        rc = (span.Days >= QuarantinePeriodNoSymptoms) ? 0 : QuarantinePeriodNoSymptoms - span.Days;
                    else
                    {
                        rc = (span.Days >= QuarantinePeriodWithSymptoms) ? ((_person.Temperature >= TemperatureFever) ? 1 : 0) : QuarantinePeriodWithSymptoms - span.Days;
                    }
                }
            }
            return rc;
        }
    }
}
