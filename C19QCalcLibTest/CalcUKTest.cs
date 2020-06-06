using System;
using C19QCalcLib;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace C19QCalcLibTest
{
    public class CalcUkTest
    {
        private readonly IClock _clock;
        private readonly DateTimeZone _zoneGmt;
        private readonly LocalDateTime _localClock;

        public CalcUkTest()
        {
            _zoneGmt = DateTimeZoneProviders.Tzdb["Europe/London"];
            _localClock = new LocalDateTime(2020, 01, 30, 17, 45, 59);
            _clock = new FakeClock(_localClock.InZoneStrictly(_zoneGmt).ToInstant());
        }
        [Fact]
        public void GetIsolationPeriodMaxTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant();

            var fred = new IsolateRecord("Fred", startQuarantine, false);
            var calc = new CalcUk(fred);

            Assert.Equal(14, calc.GetIsolationPeriodMax()); 
        }

        [Fact]
        public void NoSymptomsTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant();

            var fred = new IsolateRecord("Fred", startQuarantine, false);
            var calc = new CalcUk(fred);

            Assert.Equal(Duration.FromDays(14), calc.GetIsolationRemaining(_clock.GetCurrentInstant())); //now same as startQuarantine
        }

        [Fact]
        public void SymptomsTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant();

            var fred = new IsolateRecord("Fred", startQuarantine, true);
            var calc = new CalcUk(fred);

            Assert.Equal(Duration.FromDays(7), calc.GetIsolationRemaining(_clock.GetCurrentInstant())); //now same as startQuarantine
        }

        [Fact]
        public void NoSymptomsFeverAfterStartTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromHours(3));
            var startFever = startQuarantine.Plus(Duration.FromHours(1));

            var fred = new IsolateRecord("Fred", startQuarantine, false, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(Duration.FromHours((6 * 24) + 22), calc.GetIsolationRemaining(_clock.GetCurrentInstant())); //now 3 hours after startQuarantine and 2 hours after startFever
        }

        [Fact]
        public void SymptomsFeverAfterStartTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromHours(3));
            var startFever = startQuarantine.Plus(Duration.FromHours(1));

            var fred = new IsolateRecord("Fred", startQuarantine, true, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(Duration.FromHours((6 * 24) + 22), calc.GetIsolationRemaining(_clock.GetCurrentInstant())); //now 3 hours after startQuarantine and 2 hours after startFever
        }

        [Fact]
        public void NowEarlierStartIsolationFailTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Plus(Duration.FromHours(3));

            var fred = new IsolateRecord("Fred", startQuarantine, false);
            var calc = new CalcUk(fred);

            Assert.Equal(ExtNodatime.DurationError, calc.GetIsolationRemaining(_clock.GetCurrentInstant())); //now is earlier than start
        }

        [Fact]
        public void NowEarlierStartFeverFailTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromHours(3));   //3 hours before _clock (now)
            var startFever = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Plus(Duration.FromHours(3));         //3 hours after _clock (now)

            var fred = new IsolateRecord("Fred", startQuarantine, false, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(ExtNodatime.DurationError, calc.GetIsolationRemaining(_clock.GetCurrentInstant())); //now is earlier than startFever
        }

        [Fact]
        public void StartFeverEarlierStartIsolationFailTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromHours(3));   //3 hours before _clock (now)
            var startFever = startQuarantine.Minus(Duration.FromHours(1));                                          //1 hour before startQuarantine

            var fred = new IsolateRecord("Fred", startQuarantine, false, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(ExtNodatime.DurationError, calc.GetIsolationRemaining(_clock.GetCurrentInstant())); //startFever is earlier than startQuarantine
        }

        [Fact]
        public void SymptomsAtLessThan24HourTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromTimeSpan(new TimeSpan(0, 0, 0, 1)));
            var startFever = startQuarantine;
            var expected = Duration.FromTimeSpan(new TimeSpan(6, 23, 59, 59));

            var fred = new IsolateRecord("Fred", startQuarantine, true, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));

        }

        [Fact]
        public void SymptomsAtDay0Test()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant();
            var startFever = startQuarantine;
            var expected = Duration.FromTimeSpan(new TimeSpan(7, 0, 0, 0));

            var fred = new IsolateRecord("Fred", startQuarantine, true, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));
        }

        [Fact]
         public void SymptomsAtDay1Test()
         {
             var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromDays(1));
             var startFever = startQuarantine.Plus(Duration.FromDays(1));
             var expected = Duration.FromTimeSpan(new TimeSpan(7, 0, 0, 0));

             var fred = new IsolateRecord("Fred", startQuarantine, true, startFever);
             var calc = new CalcUk(fred);

             Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));
        }

        [Fact]
        public void SymptomsAtDay2Test()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromDays(2));
            var startFever = startQuarantine.Plus(Duration.FromDays(2));
            var expected = Duration.FromTimeSpan(new TimeSpan(7, 0, 0, 0));

            var fred = new IsolateRecord("Fred", startQuarantine, true, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));
        }

        [Fact]
        public void SymptomsAtDay13Test()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromDays(13));
            var startFever = startQuarantine.Plus(Duration.FromDays(13));
            var expected = Duration.FromTimeSpan(new TimeSpan(7, 0, 0, 0));

            var fred = new IsolateRecord("Fred", startQuarantine, true, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));
        }

        [Fact]
        public void SymptomsAtDay14Test() //It could be argued that the self-isolation is complete so fever is irrelevant, but side with caution
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromDays(14));
            var startFever = startQuarantine.Plus(Duration.FromDays(14));
            var expected = Duration.FromTimeSpan(new TimeSpan(7, 0, 0, 0));

            var fred = new IsolateRecord("Fred", startQuarantine, true, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));
        }

        [Fact]
        public void NoSymptomsAtLessThan24HourTest()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromTimeSpan(new TimeSpan(0, 0, 0, 1)));
            var expected = Duration.FromTimeSpan(new TimeSpan(13, 23, 59, 59));

            var fred = new IsolateRecord("Fred", startQuarantine, false);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));

        }

        [Fact]
        public void NoSymptomsAtDay0Test()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant();
            var expected = Duration.FromTimeSpan(new TimeSpan(14, 0, 0, 0));

            var fred = new IsolateRecord("Fred", startQuarantine, false);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));
        }

        [Fact]
        public void NoSymptomsAtDay1Test()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromDays(1));
            var expected = Duration.FromTimeSpan(new TimeSpan(13, 0, 0, 0));

            var fred = new IsolateRecord("Fred", startQuarantine, false);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));
        }

        [Fact]
        public void NoSymptomsAtDay13Test()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromDays(13));
            var expected = Duration.FromTimeSpan(new TimeSpan(1, 0, 0, 0));

            var fred = new IsolateRecord("Fred", startQuarantine, false);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));
        }

        [Fact]
        public void NoSymptomsAtDay14Test()
        {
            var startQuarantine = _localClock.InZoneStrictly(_zoneGmt).ToInstant().Minus(Duration.FromDays(14));
            var expected = Duration.FromTimeSpan(new TimeSpan(0, 0, 0, 0));

            var fred = new IsolateRecord("Fred", startQuarantine, false);
            var calc = new CalcUk(fred);

            Assert.Equal(expected, calc.GetIsolationRemaining(_clock.GetCurrentInstant()));
        }
    }
}
