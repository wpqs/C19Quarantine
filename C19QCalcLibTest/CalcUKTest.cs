using System;
using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class CalcUkTest
    {

        [Fact]
        public void NoFeverNegTimeTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 23, 0);
            var now = new             DateTime(2020, 1, 1, 17, 22, 59);     //now is earlier than start

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(-1, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            Assert.Equal(Extensions.TimeSpanError, calc.GetSpanInQuarantine(now.ToUniversalTime()));

        }

        [Fact]
        public void NoFeverSameTimeTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 23, 0);
            var now = new             DateTime(2020, 1, 1, 17, 23, 0);     //now is same as start

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(14, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(14, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void NoFeverNowTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 1, 17, 30, 1);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(14, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(13, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);

        }

        [Fact]
        public void NoFever0DayHourTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 0, 0, 0);
            var now = new             DateTime(2020, 1, 1, 23, 59, 59);     

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);

            var calc = new CalcUk(fred);

            Assert.Equal(14, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(13, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(1, quarantine.Seconds);
        }

        [Fact]
        public void NoFever0DayLessThan24HourTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 2, 17, 29, 59);  

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(14, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(13, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(1, quarantine.Seconds);
        }

        [Fact]
        public void NoFever0DayTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 23, 0);
            var now =             new DateTime(2020, 1, 1, 17, 23, 1);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(14, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(13, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoFever1DayTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 2, 17, 30, 1);  

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(13, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(12, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoFever2DayTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 3, 17, 30, 1);  

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(12, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(11, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoFever12DayTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var now = new             DateTime(2020, 4, 13, 17, 30, 1);  

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(2, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoFever13DayTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 14, 17, 30, 1);  

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(1, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoFever13DayDiffMonthTest()
        {
            var startQuarantine = new DateTime(2020, 3, 30, 17, 30, 0);
            var now = new             DateTime(2020, 4, 12, 17, 30, 1);  

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(1, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoFever14DayTest()
        {
            var startQuarantine = new DateTime(2020, 1, 1, 17, 30, 0);
            var now = new             DateTime(2020, 1, 15, 17, 30, 1);  

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(0, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void NoFever15DayTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var now = new             DateTime(2020, 4, 16, 17, 30, 1); 

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0);
            var calc = new CalcUk(fred);

            Assert.Equal(0, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void FeverAtSameDayTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever = new      DateTime(2020, 4, 1, 17, 30, 0);
            var now = new             DateTime(2020, 4, 1, 17, 30, 0);  

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 39.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(7, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(7, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }


        [Fact]
        public void FeverAtNegDayTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever = new      DateTime(2020, 4, 2, 17, 30, 0);
            var now = new             DateTime(2020, 4, 2, 17, 29, 59);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 39.0, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(-1, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            Assert.Equal(Extensions.TimeSpanError, calc.GetSpanInQuarantine(now.ToUniversalTime()));

        }

        [Fact]
        public void FeverNowTest()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 1, 17, 30, 1);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 39.0);
            var calc = new CalcUk(fred);

            Assert.Equal(7, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(7, quarantine.Days);  //symptoms start at now (1/4/20 17:30:01)
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void FeverAtDay0Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 1, 17, 30, 1);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 39.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(7, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(6, quarantine.Days);  //symptoms start at 1/4/20 17:30:00
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void FeverAtDay1Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever = new      DateTime(2020, 4, 2, 17, 30, 0);
            var now = new             DateTime(2020, 4, 2, 17, 30, 0);  

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 39.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(7, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(7, quarantine.Days);  
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

        [Fact]
        public void FeverAtDay2Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 3, 17, 30, 0);
            var now =             new DateTime(2020, 4, 3, 17, 30, 1);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 39.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(7, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(6, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoFeverDay0Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 1, 17, 30, 1);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(7, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(6, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);
        }

        [Fact]
        public void NoFeverDay1Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 2, 17, 30, 1);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(6, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(5, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);

            var mary = new Person("Mary", startQuarantine.ToUniversalTime(), 39.0, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);

            Assert.Equal(6, calc2.GetDaysInQuarantine(now.ToUniversalTime()));
            quarantine = calc2.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(5, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);

        }

        [Fact]
        public void NoFeverDay5Test()
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 6, 17, 30, 1);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(2, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);


            var mary = new Person("Mary", startQuarantine.ToUniversalTime(), 39.0, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);

            Assert.Equal(2, calc2.GetDaysInQuarantine(now.ToUniversalTime()));

            quarantine = calc2.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(23, quarantine.Hours);
            Assert.Equal(59, quarantine.Minutes);
            Assert.Equal(59, quarantine.Seconds);

        }

        [Fact]
        public void NoFeverDay6Test()  //temperature normal on 6th day
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 7, 17, 30, 0);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(1, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);

            var mary = new Person("Mary", startQuarantine.ToUniversalTime(), 39.0, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);

            Assert.Equal(1, calc2.GetDaysInQuarantine(now.ToUniversalTime()));
            quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }


        [Fact]
        public void NoFeverDay7Test() //temperature normal on 7th day
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 8, 17, 30, 0);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(0, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);

            var mary = new Person("Mary", startQuarantine.ToUniversalTime(), 39.0, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);
            Assert.Equal(1, calc2.GetDaysInQuarantine(now.ToUniversalTime()));

            quarantine = calc2.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);


        }

        [Fact]
        public void NoFeverDay8Test() //temperature normal on 8th day
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 9, 17, 30, 0);

            var fred = new Person("Fred", startQuarantine.ToUniversalTime(), 37.0, startFever.ToUniversalTime());
            var calc = new CalcUk(fred);

            Assert.Equal(0, calc.GetDaysInQuarantine(now.ToUniversalTime()));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);

            var mary = new Person("Mary", startQuarantine.ToUniversalTime(), 39.0, startFever.ToUniversalTime());
            var calc2 = new CalcUk(mary);

            Assert.Equal(1, calc2.GetDaysInQuarantine(now.ToUniversalTime()));
            quarantine = calc2.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);

        }

        [Fact]
        public void NoFeverDay9Test() //temperature normal on 9th day
        {
            var startQuarantine = new DateTime(2020, 4, 1, 17, 30, 0);
            var startFever =      new DateTime(2020, 4, 1, 17, 30, 0);
            var now =             new DateTime(2020, 4, 10, 17, 30, 1);

            var fred = new Person("Fred", startQuarantine, 37.0, startFever);
            var calc = new CalcUk(fred);

            Assert.Equal(0, calc.GetDaysInQuarantine(now));
            var quarantine = calc.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(0, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);


            var mary = new Person("Mary", startQuarantine, 39.0, startFever);
            var calc2 = new CalcUk(mary);

            Assert.Equal(1, calc2.GetDaysInQuarantine(now.ToUniversalTime()));
            quarantine = calc2.GetSpanInQuarantine(now.ToUniversalTime());

            Assert.Equal(1, quarantine.Days);
            Assert.Equal(0, quarantine.Hours);
            Assert.Equal(0, quarantine.Minutes);
            Assert.Equal(0, quarantine.Seconds);
        }

    }
}
