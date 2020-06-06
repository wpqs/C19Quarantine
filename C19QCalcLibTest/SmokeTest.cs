using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace C19QCalcLibTest
{
    public class SmokeTest
    {
        private readonly IClock _clock;
        private readonly DateTimeZone _zoneGmt;

        public SmokeTest()
        {
            _zoneGmt = DateTimeZoneProviders.Tzdb["Europe/London"];
            var tim = new LocalDateTime(2020, 5, 4, 14, 43, 0);  //04-05-2020 2:43 PM
            _clock = new FakeClock(tim.InZoneStrictly(_zoneGmt).ToInstant());
        }

        [Fact]
        public void Test() { }

        
    }
}