using System;
using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class CalcUkTest
    {
        [Fact]
        public void NowEarlierStartIsolationFailTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 23, 0);
            var now = new             DateTime(2020, 1, 1, 17, 22, 59);     //now is earlier than start

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            Assert.Equal(Extensions.TimeSpanError, calc.GetTimeSpanInIsolation(now.ToUniversalTime()));
        }

        [Fact]
        public void NowEarlierStartFeverFailTest()
        {
            var startFever = new DateTime(2020, 1, 1, 17, 23, 0);
            var now =        new DateTime(2020, 1, 1, 17, 22, 59);     //now is earlier than start
            var startQuarantine = now;
            
            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(Extensions.TimeSpanError, calc.GetTimeSpanInIsolation(now.ToUniversalTime()));
        }

        [Fact]
        public void StartFeverEarlierStartIsolationFailTest()
        {
            var now = new DateTime(2020, 1, 1, 17, 23, 0);     //now is earlier than start
            var startQuarantine = now;
            var startFever = new DateTime(2020, 1, 1, 17, 22, 59);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(Extensions.TimeSpanError, calc.GetTimeSpanInIsolation(now.ToUniversalTime()));
        }


        [Fact]
        public void SymptomsAtNegDayFailTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever = new DateTime(2020, 4, 2, 17, 30, 0);
            var now = new DateTime(2020, 4, 2, 17, 29, 59);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), true, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(Extensions.TimeSpanError, calc.GetTimeSpanInIsolation(now.ToUniversalTime()));
        }

        [Fact]
        public void SymptomsNowTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 1, 17, 30, 1);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), true);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(7, quarantine.Days);  //symptoms start at now (1/4/20 17:30:01)
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void SymptomsAtSameDayTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever = new DateTime(2020, 4, 1, 17, 30, 0);
            var now = new DateTime(2020, 4, 1, 17, 30, 0);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(7, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void SymptomsAtDay0Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever = new DateTime(2020, 4, 1, 17, 30, 0);
            var now = new DateTime(2020, 4, 1, 17, 30, 1);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(6, quarantine.Days);  //symptoms start at 1/4/20 17:30:00
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void SymptomsAtDay1Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever = new DateTime(2020, 4, 2, 17, 30, 0);
            var now = new DateTime(2020, 4, 2, 17, 30, 0);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(7, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void SymptomsAtDay2Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever = new DateTime(2020, 4, 3, 17, 30, 0);
            var now = new DateTime(2020, 4, 3, 17, 30, 1);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(6, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void Symptoms13DaysAfterStartIsolationTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 23, 0);
            var startFever = new DateTime(2020, 1, 14, 17, 23, 0);
            var now = new DateTime(2020, 1, 14, 17, 23, 0);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(7, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void Symptoms14DaysAfterStartIsolationTest()    //It could be argued that the self-isolation is complete so fever is irrelevant, but side with caution
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 23, 0);
            var startFever =      new DateTime(2020, 1, 15, 17, 23, 0);
            var now =             new DateTime(2020, 1, 15, 17, 23, 0);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(7, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }


        [Fact]
        public void NoSymptomsSameTimeTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 23, 0);
            var now = new             DateTime(2020, 1, 1, 17, 23, 0);     //now is same as start

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(14, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptomsNowTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 1, 17, 30, 1);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(13, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptoms0DayHourTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 0, 0, 0);
            var now = new             DateTime(2020, 1, 1, 23, 59, 59);     

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);

            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(13, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(1, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptoms0DayLessThan24HourTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 2, 17, 29, 59);  

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(13, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(1, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptomsAfterDay0Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 1, 17, 30, 1);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(6, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptomsAfterDay1Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 2, 17, 30, 1);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(5, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);

            var mary = new C19QCalcLib.Record("Mary", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);

            quarantine = calc2.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(5, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptomsAfterDay5Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 6, 17, 30, 1);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);

            var mary = new C19QCalcLib.Record("Mary", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);

            quarantine = calc2.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptomsAfterDay6Test()  //temperature normal on 6th day
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 7, 17, 30, 0);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);

            var mary = new C19QCalcLib.Record("Mary", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);

            quarantine = calc2.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }


        [Fact]
        public void NoSymptomsAfterDay7Test() //temperature normal on 7th day
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 8, 17, 30, 0);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);

            var mary = new C19QCalcLib.Record("Mary", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);

            quarantine = calc2.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptomsAfterDay8Test() //temperature normal on 8th day
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 9, 17, 30, 0);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);

            var mary = new C19QCalcLib.Record("Mary", startQuarantine.ToUniversalTime(), true, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);

            quarantine = calc2.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptomsAfterDay9Test() //no symptoms on 9th day
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 10, 17, 30, 1);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine, false, startFever);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);

            var mary = new C19QCalcLib.Record("Mary", startQuarantine, true, startFever);
            var calc2 = new CalcUk(mary);

            quarantine = calc2.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }


        [Fact]
        public void NoSymptomsDay0Test()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 23, 0);
            var now = new DateTime(2020, 1, 1, 17, 23, 1);

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(13, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptoms1DayTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 2, 17, 30, 1);  

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(12, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptoms2DayTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 3, 17, 30, 1);  

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(11, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptoms12DayTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var now = new             DateTime(2020, 4, 13, 17, 30, 1);  

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptoms13DayTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 14, 17, 30, 1);  

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptoms13DayDiffMonthTest()
        {
            var startQuarantine = new DateTime(2020, 3, 30, 17, 30, 0);
            var now = new             DateTime(2020, 4, 12, 17, 30, 1);  

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptoms14DayTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 15, 17, 30, 1);  

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void NoSymptoms15DayTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var now = new             DateTime(2020, 4, 16, 17, 30, 1); 

            var fred = new C19QCalcLib.Record("Fred", startQuarantine.ToUniversalTime(), false);
            var calc = new CalcUk(fred);

            var quarantine = calc.GetTimeSpanInIsolation(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }
    }
}
