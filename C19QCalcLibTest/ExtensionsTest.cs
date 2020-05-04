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

        // Test to demonstrate DateTimeOffset unexpected functionality

        [Fact]
        public void DateTimeOffsetLocalTimeIsSystemTimeZone()
        {
            var testTime = new DateTime(2020, 1, 1, 13, 45, 0);
            var noOffset = new TimeSpan(0);

            var utc = new DateTimeOffset(testTime, noOffset);

            Assert.Equal(noOffset, utc.Offset);                              //as anticipated
            Assert.Equal(testTime, utc.DateTime);                            //as anticipated - utc.DateTime is not affected by Offset
            Assert.Equal(DateTimeKind.Unspecified, utc.DateTime.Kind);       //as anticipated
            Assert.Equal(testTime, utc.UtcDateTime);                         //as anticipated
            Assert.Equal(DateTimeKind.Utc, utc.UtcDateTime.Kind);            //as anticipated
            Assert.Equal(DateTimeKind.Local, utc.LocalDateTime.Kind);        //as anticipated

            Assert.Equal(testTime + new TimeSpan(0,1,0,0), utc.LocalDateTime);    //the potential gotcha - passes only when test computer is operating in CET Summer Time
        }

        // IsInvalidTimeSafe tests

        [Fact]
        public void TimeZoneIsInvalidTimeSafeDateTime()
        {  
            //see https://stackoverflow.com/questions/61574644/why-does-timezoneinfo-isvalidtime-give-unexpected-result-for-datetimekind-loca

            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            var testTimeUnspec = new DateTime(2020, 3, 29, 02, 01, 0, DateTimeKind.Unspecified);
            var testTimeLocal = new DateTime(2020, 3, 29, 02, 01, 0, DateTimeKind.Local);
            var testTimeUtc = new DateTime(2020, 3, 29, 02, 01, 0, DateTimeKind.Utc);

            Assert.False(timeZone.IsInvalidTime(testTimeUtc));      //as anticipated - it's UTC so cannot be invalid
            Assert.True(timeZone.IsInvalidTime(testTimeUnspec));    //strange - it might be UTC so might be valid
            Assert.False(timeZone.IsInvalidTime(testTimeLocal));    //totally unexpected - Converts dateTime to the time of the TimeZoneInfo object and returns false - https://docs.microsoft.com/en-us/dotnet/api/system.timezoneinfo.isinvalidtime?view=netcore-3.1
            //FIX
            Assert.True(timeZone.IsInvalidTimeSafe(testTimeLocal));  //as expected
            Assert.True(timeZone.IsInvalidTimeSafe(testTimeUnspec)); //as expected
            Assert.False(timeZone.IsInvalidTimeSafe(testTimeUtc));   //as expected
        }


        //IsValid Tests

        [Fact]
        public void DateTimeOffsetIsErrorTrueTest()
        {
            var tim = Extensions.DateTimeOffsetError;
            Assert.True(tim.IsError());
        }

        [Fact]
        public void DateTimeIsErrorTrueTest()
        {
            var tim = Extensions.DateTimeError;
            Assert.True(tim.IsError());
        }

        [Fact]
        public void DateTimeOffsetIsErrorFalseTest()
        {
            var tim = DateTimeOffset.Now;
            Assert.False(tim.IsError());
        }

        [Fact]
        public void DateTimeIsErrorFalseTest()
        {
            var tim = DateTime.Now;
            Assert.False(tim.IsError());
        }

        [Fact]
        public void TimeSpanIsErrorTrueTest()
        {
            var span = Extensions.TimeSpanError;
            Assert.True(span.IsError());
        }

        [Fact]
        public void TimeSpanIsErrorFalseTest()
        {
            var span = new TimeSpan();
            Assert.False(span.IsError());
        }

        //ConvertUtcToLocalTimeString tests

        [Fact]
        public void ConvertUtcToLocalTimeStringCetPmWinterTest()
        {
            var now = new DateTime(2020, 1, 16, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - CET is UTC+1 in wintertime  = 14:45
            Assert.Equal("16-01-2020 02:45 PM", now.ConvertUtcToLocalTimeString("dd-MM-yyyy hh:mm tt", "Central European Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringCetPmSummerTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 3, DateTimeKind.Utc);
            //UTC 13:45:03 - CET is UTC+2 in summertime = 14:45:03
            Assert.Equal("15-04-20 03:45:03 PM", nowDayLight.ConvertUtcToLocalTimeString("dd-MM-yy hh:mm:ss tt", "Central European Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringGmtPmWinterTest()
        {
            var now = new DateTime(2020, 1, 16, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC in wintertime
            Assert.Equal("16-01-20 01:45 PM", now.ConvertUtcToLocalTimeString("dd-MM-yy hh:mm tt", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringGmtSummerTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("15-04-20 02:45 PM", nowDayLight.ConvertUtcToLocalTimeString("dd-MM-yy hh:mm tt", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringTimeLocalFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Local);
            //Fail as nowDayLight is local not utc
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTimeString("dd-MM-yy hh:mm tt", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringTimeUnspecFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Unspecified);
            //Fail as nowDayLight is unspec not utc
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTimeString("dd-MM-yy hh:mm tt", "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringTimeFormatNullFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTimeString(null, "GMT Standard Time")); //use hh for 12 hour or HH for 24 hour
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringTimeFormatEmptyFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTimeString("", "GMT Standard Time")); 
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringTimeFormatInvalidFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("xxx", nowDayLight.ConvertUtcToLocalTimeString("xxx", "GMT Standard Time")); 
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringTimeZoneNullFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTimeString("dd-MM-yy hh:mm tt", null)); 
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringTimeZoneEmptyFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTimeString("dd-MM-yy hh:mm tt", "")); 
        }

        [Fact]
        public void ConvertUtcToLocalTimeStringTimeZoneInvalidFailTest()
        {
            var nowDayLight = new DateTime(2020, 4, 15, 13, 45, 0, DateTimeKind.Utc);
            //UTC 13:45 - GMT is UTC+1 in summertime
            Assert.Equal("[Error]", nowDayLight.ConvertUtcToLocalTimeString("dd-MM-yy hh:mm tt", "xxx")); 
        }


        [Fact]
        public void ConvertLocalTimeToUtcWinterCet()
        {
            var utc = new DateTime(2020, 1, 1, 12, 45, 0);
            var time = new DateTime(2020, 1, 1, 13, 45, 0);
            //CET Winter 13:45 - UTC 12:45
            Assert.Equal(utc, time.ConvertLocalTimeToUtc("Central European Standard Time"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcSummerCet()
        {
            var utc = new DateTime(2020, 5, 1, 11, 45, 0);
            var time = new DateTime(2020, 5, 1, 13, 45, 0);
            //CET Summer 13:45 - UTC 11:45
            Assert.Equal(utc, time.ConvertLocalTimeToUtc("Central European Standard Time"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcWinterGmt()
        {
            var utc = new DateTime(2020, 1, 1, 12, 45, 0);
            var time = new DateTime(2020, 1, 1, 12, 45, 0);
            //GMT Winter 12:45 - UTC 12:45
            Assert.Equal(utc, time.ConvertLocalTimeToUtc("GMT Standard Time"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcSummerGmt()
        {
            var utc = new DateTime(2020, 5, 1, 11, 45, 0);
            var time = new DateTime(2020, 5, 1, 12, 45, 0);
            //GMT Summer 12:45 - UTC 11:45
            Assert.Equal(utc, time.ConvertLocalTimeToUtc("GMT Standard Time"));
        }

        //ConvertLocalTimeToDateTimeOffset tests

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeMonthAbrInChars()
        {
            var tim = "16-Jan-20 02:45 PM"; //CET Winter UTC+1
            var expected = new DateTime(2020, 01, 16, 13, 45, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("Central European Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeMonthFullInChars()
        {
            var tim = "16-January-20 02:45 PM"; //CET Winter UTC+1
            var expected = new DateTime(2020, 01, 16, 13, 45, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("Central European Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeCetWinterTest()
        {
            var tim = "16-01-20 02:45 PM"; //CET Winter UTC+1
            var expected = new DateTime(2020, 01, 16, 13, 45, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("Central European Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeCetSummerTest()
        {
            var tim = "16-05-2020 02:45 PM"; //CET Summer UTC+2
            var expected = new DateTime(2020, 05, 16, 12, 45, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("Central European Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeGmtWinterTest()
        {
            var tim = "16-01-20 02:45PM"; //CET Winter UTC+0
            var expected = new DateTime(2020, 01, 16, 14, 45, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeGmtSummerTest()
        {
           var tim = "16-05-20 02:45 PM"; //GMT Summer UTC+1
           var expected = new DateTime(2020, 05, 16, 13, 45, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTime24HourTimePmTest()
        {
            var tim = "16-05-2020 17:59 PM"; //GMT Summer UTC+1
            var expected = new DateTime(2020, 05, 16, 16, 59, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTime24HourTimeNoPmTest()
        {
            var tim = "16-05-20 17:59:05"; //GMT Summer UTC+1
            var expected = new DateTime(2020, 05, 16, 16, 59, 5);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB" ));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTime24HourTimePmLowerCaseTest()
        {
            var tim = "16-05-20 17:59 pm"; //GMT Summer UTC+1
            var expected = new DateTime(2020, 05, 16, 16, 59, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeNoonTest()
        {
            var tim = "16-05-20 12:00 noon"; //GMT Summer UTC+1
            var expected = new DateTime(2020, 05, 16, 11, 0, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeMidnightTest()
        {
            var tim = "16-05-20 12:00 Midnight"; //GMT Summer UTC+1
            var expected = new DateTime(2020, 05, 15, 23, 0, 0);
            Assert.Equal(expected, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeNotMidnightFailTest()
        {
            var tim = "16-05-20 12:01 Midnight";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeNotNoonFailTest()
        {
            var tim = "16-05-20 12:01 Noon";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTime24HourTimePmDotFailTest()
        {
            var tim = "16-05-20 17:59 P.M.";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeInvalidPmFailTest()
        {
            var tim = "16-05-20 15:59 AM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeInvalidSecFailTest()
        {
            var tim = "16-05-20 02:59:61 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeInvalidMinFailTest()
        {
            var tim = "16-05-20 02:61 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeInvalidHourFailTest()
        {
            var tim = "16-05-20 25:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeInvalidDateSepFailTest()
        {
            var tim = "30\\05\\20 15:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeInvalidTimeSepFailTest()
        {
            var tim = "30-05-20 15-59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeInvalidDayFailTest()
        {
            var tim = "32-05-20 15:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeInvalidMonthFailTest()
        {
            var tim = "30-13-20 15:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcDateTimeInvalidYearFailTest()
        {
            var tim = "30-12-xx 15:59 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "en-GB"));
        }


        [Fact]
        public void ConvertLocalTimeToUtcTimeZoneNullFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime(null, "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcTimeZoneEmptyFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcTimeZoneInvalidFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("xxx", "en-GB"));
        }

        [Fact]
        public void ConvertLocalTimeToUtcCultureNullFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", null));
        }

        [Fact]
        public void ConvertLocalTimeToUtcCultureEmptyFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", ""));
        }

        [Fact]
        public void ConvertLocalTimeToUtcCultureInvalidFailTest()
        {
            var tim = "16-05-20 02:45 PM";
            Assert.Equal(_invalidDateTime, tim.ConvertLocalTimeToUtcDateTime("GMT Standard Time", "xxx"));
        }

        [Fact]
        public void ToStringDaysHours3DayTest()
        {
            var span = new TimeSpan(3, 0, 0, 0);
            Assert.Equal("3 days", span.ToStringRemainingDaysHours());
        }


        [Fact]
        public void ToStringDaysHours2DayAdjUpTest()
        {
            var span = new TimeSpan(2, 12, 0, 0);
            Assert.Equal("3 days", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHours2DayAdjDownTest()
        {
            var span = new TimeSpan(2, 11, 29, 59);
            Assert.Equal("2 days 11 hours", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHours2DayTest()
        {
            var span = new TimeSpan(2, 0, 0, 0);
            Assert.Equal("2 days", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHours1DayAdjUpTest()
        {
            var span = new TimeSpan(1, 12, 0, 00);
            Assert.Equal("2 days", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHours1DayAdjDownTest()
        {
            var span = new TimeSpan(1, 11, 29, 59);
            Assert.Equal("1 day 11 hours", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHours1DayTest()
        {
            var span = new TimeSpan(1, 0, 0, 0);
            Assert.Equal("1 day", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHours3HourTest()
        {
            var span = new TimeSpan(0, 3, 0, 0);
            Assert.Equal("3 hours", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHours2HourAdjUpTest()
        {
            var span = new TimeSpan(0, 2, 30, 00);
            Assert.Equal("3 hours", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHours2HourAdjDownTest()
        {
            var span = new TimeSpan(0, 2, 29, 59);
            Assert.Equal("2 hours", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHours1HourTest()
        {
            var span = new TimeSpan(0, 1, 0, 0);
            Assert.Equal("1 hour", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHoursLessHourTest()
        {
            var span = new TimeSpan(0, 0, 59, 59);
            Assert.Equal("less than an hour", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHoursLessSecondTest()
        {
            var span = new TimeSpan(0, 0, 0, 1);
            Assert.Equal("less than an hour", span.ToStringRemainingDaysHours());
        }

        [Fact]
        public void ToStringDaysHoursZeroTest()
        {
            var span = new TimeSpan(0, 0, 0, 0);
            Assert.Equal("0", span.ToStringRemainingDaysHours());
        }
    }
}
