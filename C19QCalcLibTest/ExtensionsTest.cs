using System;
using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class ExtensionsTest
    {
        private readonly DateTime _invalidDateTime;
        public ExtensionsTest()
        {
            _invalidDateTime = Extensions.DateTimeError;
        }

        [Fact]
        public void DateTimeInvalidIsErrorTest()
        {
            var tim = Extensions.DateTimeError;
            Assert.True(tim.IsError());
        }

        [Fact]
        public void DateTimeValidIsErrorTest()
        {
            var tim = DateTime.Now;
            Assert.False(tim.IsError());
        }

        [Fact]
        public void TimeSpanInvalidIsErrorTest()
        {
            var span = Extensions.TimeSpanError;
            Assert.True(span.IsError());
        }

        [Fact]
        public void TimeSpanValidIsErrorTest()
        {
            var span = new TimeSpan();
            Assert.False(span.IsError());
        }

        [Fact]
        public void ConvertUtcToLocalTimeCetWinterTest()
        {
            var now = new DateTime(2020, 1, 16, 13, 45, 0, DateTimeKind.Utc);
                //UTC 13:45 - CET is UTC+1 in wintertime
            Assert.Equal("16-01-2020 02:45 PM", now.ConvertUtcToLocalTime("dd-MM-yyyy hh:mm tt", "Central European Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeCetSummerTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 3, DateTimeKind.Utc);
            //UTC 13:45 - CET is UTC+2 in summertime
            Assert.Equal("15-04-20 03:45:03 PM", nowDayLight.ConvertUtcToLocalTime("dd-MM-yy hh:mm:ss tt", "Central European Standard Time")); //use hh for 12 hour or HH for 24 hour

        }

        [Fact]
        public void ConvertUtcToLocalTimeGmtWinterTest()
        {
            var now = new DateTime(2020, 1, 16, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC in wintertime
            Assert.Equal("16-01-20 01:45 PM", now.ConvertUtcToLocalTime("dd-MM-yy hh:mm tt", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeGmtSummerTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("15-04-20 02:45 PM", nowDayLight.ConvertUtcToLocalTime("dd-MM-yy hh:mm tt", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeLocalFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Local);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTime("dd-MM-yy hh:mm tt", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeLUnspecFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Unspecified);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTime("dd-MM-yy hh:mm tt", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeFormatNullFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTime(null, "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeFormatEmptyFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("15/04/2020 14:45:00 +01:00", nowDayLight.ConvertUtcToLocalTime("", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeFormatInvalidFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("xxx", nowDayLight.ConvertUtcToLocalTime("xxx", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }


        [Fact]
        public void ConvertUtcToLocalTimeTimeZoneNullFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTime("dd-MM-yy hh:mm tt", null)); //use hh for 12 hour or HH for 24 hour
        }


        [Fact]
        public void ConvertUtcToLocalTimeTimeZoneEmptyFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTime("dd-MM-yy hh:mm tt", "")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeTimeZoneInvalidFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTime("dd-MM-yy hh:mm tt", "xxx")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertLocalTimeToUtcMonthAbrInChars()
        {
            var tim = "16-Jan-20 02:45 PM";
            Assert.Equal(new DateTime(2020, 01, 16, 13, 45, 0), tim.ConvertLocalTimeToUtc("Central European Standard Time", "en-GB"));

        }

        [Fact]
        public void ConvertLocalTimeToUtcMonthFullInChars()
        {
            var tim = "16-January-20 02:45 PM";
            Assert.Equal(new DateTime(2020, 01, 16, 13, 45, 0), tim.ConvertLocalTimeToUtc("Central European Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcCetWinterTest()
        {
            var tim = "16-01-20 02:45 PM";
            Assert.Equal(new DateTime(2020, 01, 16, 13, 45, 0), tim.ConvertLocalTimeToUtc("Central European Standard Time", "en-GB"));
            Assert.Equal(new DateTime(2020, 01, 16, 13, 45, 0), tim.ConvertLocalTimeToUtc("Central European Standard Time", "en-GB", true));
        }

        [Fact]
        public void ConvertLocalTimeToUtcCetSummerTest()
        {
            var tim = "16-05-2020 02:45 PM";
            Assert.Equal(new DateTime(2020, 05, 16, 12, 45, 0), tim.ConvertLocalTimeToUtc("Central European Standard Time", "en-GB"));
            Assert.Equal(new DateTime(2020, 05, 16, 13, 45, 0), tim.ConvertLocalTimeToUtc("Central European Standard Time", "en-GB", true));
        }

        [Fact]

        public void ConvertLocalTimeToUtcGmtWinterTest()
        {
            var tim = "16-01-20 02:45PM";
            Assert.Equal(new DateTime(2020, 01, 16, 14, 45, 0), tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
            Assert.Equal(new DateTime(2020, 01, 16, 14, 45, 0), tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB", true));
        }

        [Fact]
        public void ConvertLocalTimeToUtcGmtSummerTest()
        {
           var tim = "16-05-20 02:45 PM";
            Assert.Equal(new DateTime(2020, 05, 16, 13, 45, 0), tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
            tim = "16-05-20 02:45 PM";
            Assert.Equal(new DateTime(2020, 05, 16, 14, 45, 0), tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB", true));
        }

        [Fact]
        public void ConvertLocalTimeToUtc24HourTest()
        {
            var tim = "16-05-2020 17:59 PM";
            Assert.Equal(new DateTime(2020, 05, 16, 16, 59, 0), tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtc24HourNoPmTest()
        {
            var tim = "16-05-20 17:59:05";
            Assert.Equal(new DateTime(2020, 05, 16, 16, 59, 5), tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtc24HourPmLowerCaseTest()
        {
            var tim = "16-05-20 17:59 pm";
            Assert.Equal(new DateTime(2020, 05, 16, 16, 59, 0), tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcNoonTest()
        {
            var tim = "16-05-20 12:00 noon";
            Assert.Equal(new DateTime(2020, 05, 16, 11, 0, 0), tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcMidnightTest()
        {
            var tim = "16-05-20 12:00 Midnight";
            Assert.Equal(new DateTime(2020, 05, 15, 23, 0, 0), tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcNotMidnightFailTest()
        {
            var tim = "16-05-20 12:01 Midnight";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcNotNoonFailTest()
        {
            var tim = "16-05-20 12:01 Noon";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtc24HourPmDotFailTest()
        {
            var tim = "16-05-20 17:59 P.M.";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcInvalidPmFailTest()
        {
            var tim = "16-05-20 15:59 AM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcInvalidSecFailTest()
        {
            var tim = "16-05-20 02:59:61 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcInvalidMinFailTest()
        {
            var tim = "16-05-20 02:61 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcInvalidHourFailTest()
        {
            var tim = "16-05-20 25:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcInvalidDateSepFailTest()
        {
            var tim = "30\\05\\20 15:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcInvalidTimeSepFailTest()
        {
            var tim = "30-05-20 15-59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcInvalidDayFailTest()
        {
            var tim = "32-05-20 15:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcInvalidMonthFailTest()
        {
            var tim = "30-13-20 15:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcInvalidYearFailTest()
        {
            var tim = "30-12-xx 15:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "en-GB"));
        }


        [Fact]
        public void ConvertLocalTimeToUtcTimeZoneNullFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc(null, "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcTimeZoneEmptyFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcTimeZoneInvalidFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("xxx", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcCultureNullFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", null));
        }

        [Fact]
        public void ConvertLocalTimeToUtcCultureEmptyFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", ""));
        }

        [Fact]
        public void ConvertLocalTimeToUtcCultureInvalidFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtc("GMT Standard Time", "xxx"));
        }

        [Fact]
        public void ToStringDaysHours3DayTest()
        {
            var span = new TimeSpan(3, 0, 0, 0);
            Assert.Equal("3 days", span.ToStringDaysHours());
        }


        [Fact]
        public void ToStringDaysHours2DayAdjUpTest()
        {
            var span = new TimeSpan(2, 12, 0, 0);
            Assert.Equal("3 days", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHours2DayAdjDownTest()
        {
            var span = new TimeSpan(2, 11, 29, 59);
            Assert.Equal("2 days 11 hours", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHours2DayTest()
        {
            var span = new TimeSpan(2, 0, 0, 0);
            Assert.Equal("2 days", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHours1DayAdjUpTest()
        {
            var span = new TimeSpan(1, 12, 0, 00);
            Assert.Equal("2 days", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHours1DayAdjDownTest()
        {
            var span = new TimeSpan(1, 11, 29, 59);
            Assert.Equal("1 day 11 hours", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHours1DayTest()
        {
            var span = new TimeSpan(1, 0, 0, 0);
            Assert.Equal("1 day", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHours3HourTest()
        {
            var span = new TimeSpan(0, 3, 0, 0);
            Assert.Equal("3 hours", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHours2HourAdjUpTest()
        {
            var span = new TimeSpan(0, 2, 30, 00);
            Assert.Equal("3 hours", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHours2HourAdjDownTest()
        {
            var span = new TimeSpan(0, 2, 29, 59);
            Assert.Equal("2 hours", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHours1HourTest()
        {
            var span = new TimeSpan(0, 1, 0, 0);
            Assert.Equal("1 hour", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHoursLessHourTest()
        {
            var span = new TimeSpan(0, 0, 59, 59);
            Assert.Equal("less than an hour", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHoursLessSecondTest()
        {
            var span = new TimeSpan(0, 0, 0, 1);
            Assert.Equal("less than an hour", span.ToStringDaysHours());
        }

        [Fact]
        public void ToStringDaysHoursZeroTest()
        {
            var span = new TimeSpan(0, 0, 0, 0);
            Assert.Equal("0 minutes", span.ToStringDaysHours());
        }
    }
}
