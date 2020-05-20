using System;

namespace C19QCalcLib
{
    public class CalcUk : ICalc
    {
        private readonly int QuarantinePeriodNoSymptoms = 14;
        private readonly int QuarantinePeriodWithSymptoms = 7;

        private readonly TimeSpan _quarantineSpanNoSymptoms = new TimeSpan(14,0,0,0);
        private readonly TimeSpan _quarantineSpanWithSymptoms = new TimeSpan(7, 0, 0, 0);
        private readonly Record _record;

        public bool IsSymptomatic() { return _record?.HasSymptoms ?? false; }

        public CalcUk(Record record)
        {
            _record = record;
        }

        public int GetIsolationPeriodMax() { return QuarantinePeriodNoSymptoms; }

        public TimeSpan GetTimeSpanInIsolation(DateTime nowUtc)
        {
            var rc = Extensions.TimeSpanError; //error

            if ((_record != null) && (nowUtc >= _record.QuarantineStartedUtc) && (nowUtc >= (_record.FirstSymptomsUtc ?? nowUtc)) && ((_record.FirstSymptomsUtc ?? nowUtc) >= _record.QuarantineStartedUtc))
            {
                if (_record.HasSymptoms && (_record.FirstSymptomsUtc == null))
                    _record.SetFirstSymptoms(nowUtc);

                var span = nowUtc - (_record.FirstSymptomsUtc ?? _record.QuarantineStartedUtc);
                if (span.TotalSeconds >= 0)
                {
                    if (_record.FirstSymptomsUtc == null)
                        rc = (span.Days >= QuarantinePeriodNoSymptoms) ? new TimeSpan(0) : _quarantineSpanNoSymptoms - span;
                    else
                    {
                        if (span.Days >= QuarantinePeriodWithSymptoms)
                            rc =  (_record.HasSymptoms) ? new TimeSpan(1,0,0, 0) : new TimeSpan(0);
                        else
                        {
                            var remain = _quarantineSpanWithSymptoms - span;
                            if (remain.Days < 1)
                                rc = (_record.HasSymptoms) ? new TimeSpan(1, 0, 0, 0) : remain;
                            else
                                rc = remain;
                        }
                    }
                }
            }
            return rc;
        }
    }
}
