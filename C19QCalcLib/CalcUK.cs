using NodaTime;

namespace C19QCalcLib
{
    public class CalcUk : ICalc
    {
        private const int QuarantinePeriodNoSymptoms = 14;
        private const int QuarantinePeriodWithSymptoms = 7;

        private readonly Duration _quarantineSpanNoSymptoms = Duration.FromDays(QuarantinePeriodNoSymptoms); 
        private readonly Duration _quarantineSpanWithSymptoms = Duration.FromDays(QuarantinePeriodWithSymptoms);
        private readonly IsolateRecord _isolateRecord;

        public bool IsSymptomatic() { return _isolateRecord?.HasSymptoms ?? false; }

        public CalcUk(IsolateRecord isolateRecord)
        {
            _isolateRecord = isolateRecord;
        }

        public int GetIsolationPeriodMax() { return QuarantinePeriodNoSymptoms; }

        public int GetIsolationDaysRemaining(Instant nowInstance, out string resultColor, out string comment)
        {
            int rc = -1;

            comment = "[error]";
            resultColor = "red";

            var span = GetIsolationRemaining(nowInstance);
            if (span.IsError() == false)
            {
                if (span.TotalMinutes > 0)
                {
                    comment = $"The time remaining for your self-isolation is {span.ToStringRemainingTime()}";
                    resultColor = "orange";
                    rc = ((int) span.TotalDays) + ((span.Hours > 12) ? 1 : 0);
                }
                else
                {
                    comment = $"Your self-isolation is now COMPLETE unless you have been advised otherwise";
                    resultColor = "green";
                    rc = 0;
                }
            }
            return rc;
        }

        public Duration GetIsolationRemaining(Instant nowInstance)
        {
            var rc = ExtNodatime.DurationError; //error

            if ((_isolateRecord != null) && (nowInstance >= _isolateRecord.QuarantineStarted) 
                                  && (nowInstance >= (_isolateRecord.FirstSymptoms ?? nowInstance)) 
                                  && ((_isolateRecord.FirstSymptoms ?? nowInstance) >= _isolateRecord.QuarantineStarted))
            {
                if (_isolateRecord.HasSymptoms && (_isolateRecord.FirstSymptoms == null))
                    _isolateRecord.SetFirstSymptoms(nowInstance);

                var span = nowInstance - (_isolateRecord.FirstSymptoms ?? _isolateRecord.QuarantineStarted);
                if (span.TotalSeconds >= 0)
                {
                    if (_isolateRecord.FirstSymptoms == null)
                        rc = (span.Days >= _quarantineSpanNoSymptoms.Days) ? Duration.Zero : _quarantineSpanNoSymptoms - span;
                    else
                    {
                        if (span.Days >= QuarantinePeriodWithSymptoms)
                            rc =  (_isolateRecord.HasSymptoms) ? Duration.FromDays(1) : Duration.Zero;
                        else
                        {
                            var remain = _quarantineSpanWithSymptoms - span;
                            if (remain.Days < 1)
                                rc = (_isolateRecord.HasSymptoms) ? Duration.FromDays(1) : remain;
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
