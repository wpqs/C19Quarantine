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
