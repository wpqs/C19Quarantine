using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace C19QCalcLibTest
{
    public class IsolateRecordTest
    {
        private readonly IClock _clock;
        private readonly DateTimeZone _zoneGmt;
        private readonly LocalDateTime _clockTime;
        private readonly Instant _startIsolation;
        private readonly Instant _startSymptoms;

        public IsolateRecordTest()
        {
            _zoneGmt = DateTimeZoneProviders.Tzdb[id: "Europe/London"];
            
            _startIsolation = new LocalDateTime(year: 2020, month: 03, day: 29, hour: 00, minute: 59, second: 59).InZoneStrictly(zone: _zoneGmt).ToInstant();
            _startSymptoms = new LocalDateTime(year: 2020, month: 03, day: 30, hour: 00, minute: 59, second: 59).InZoneStrictly(zone: _zoneGmt).ToInstant();
            _clockTime = new LocalDateTime(year: 2020, month: 01, day: 30, hour: 17, minute: 45, second: 59);
            
            _clock = new FakeClock(initial: _clockTime.InZoneStrictly(zone: _zoneGmt).ToInstant());
        }

        [Fact]
        public void TestCtor()
        {
            Assert.Equal(_clockTime, _clock.GetCurrentInstant().InZone(_zoneGmt).LocalDateTime);

            var record = new C19QCalcLib.IsolateRecord("Test", _startIsolation, true, _startSymptoms);

            Assert.Equal("Test", record.Name);
            Assert.True(record.HasSymptoms);
            Assert.Equal(_startIsolation, record.QuarantineStarted);
            Assert.Equal(_startSymptoms, record.FirstSymptoms);
        }
    }
}