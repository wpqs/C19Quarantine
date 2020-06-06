using C19QCalcLib;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace C19QCalcLibTest
{
    public class ExtNodaTimeTest
    {
        private readonly IClock _clock;
        private readonly DateTimeZone _zoneGmt;

        public ExtNodaTimeTest()
        {
            _zoneGmt = DateTimeZoneProviders.Tzdb["Europe/London"];
            _clock = new FakeClock(new LocalDateTime(2020, 01, 30, 17, 45, 59).InZoneStrictly(_zoneGmt).ToInstant());
        }

        [Fact]
        public void InstantIsErrorTest()
        {
            Assert.True(Instant.MaxValue.IsError());
            Assert.False(_clock.GetCurrentInstant().IsError());
        }

        [Fact]
        public void DurationIsErrorTest()
        {
            Assert.True(Duration.MaxValue.IsError());
            Assert.False(Duration.FromDays(14).IsError());
        }

        [Fact]
        public void InstantToStringDateTimeTest()
        {
            var winterTime = new LocalDateTime(2020, 01, 30, 17, 45, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.Equal("30-01-2020 5:45 PM", winterTime.ToString("en-GB", _zoneGmt));
            Assert.Equal("30-01-2020 5:45 PM", winterTime.ToString("en-GB", _zoneGmt, true));   //not in daylight saving so no change

            var summerTime = new LocalDateTime(2020, 06, 30, 17, 45, 00).InZoneStrictly(_zoneGmt).ToInstant();
            Assert.Equal("30-06-2020 5:45 PM", summerTime.ToString("en-GB", _zoneGmt));
            Assert.Equal("30-06-2020 4:45 PM", summerTime.ToString("en-GB", _zoneGmt, true));   //daylight saving applies so display string as GMT value not BST
        }

        [Fact]
        public void InstantToStringDateTest()
        {
            Assert.Equal("30-01-2020", _clock.GetCurrentInstant().ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Date));
        }

        [Fact]
        public void InstantToStringTimeTest()
        {
            var winterTime = new LocalDateTime(2020, 01, 30, 17, 45, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.Equal("5:45 PM", winterTime.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Time));
            Assert.Equal("5:45 PM", winterTime.ToString("en-GB", _zoneGmt, true, MxCultureInfo.FormatType.Time)); //not in daylight saving so no change

            var summerTime = new LocalDateTime(2020, 06, 30, 17, 45, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.Equal("5:45 PM", summerTime.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Time));
            Assert.Equal("4:45 PM", summerTime.ToString("en-GB", _zoneGmt, true, MxCultureInfo.FormatType.Time)); //daylight saving applies so display string as GMT value not BST
        }

        [Fact]
        public void StringParseDateTimeToInstantShortTest()
        {
            var text = "30-01-2020 5:45 PM";
            var expected = new LocalDateTime(2020, 01, 30, 17, 45, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.True(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, false, out var result));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void StringParseDateTimeToInstantLongTest()
        {
            var textWinter = "30-01-2020 17:45:59";
            var winterTime = new LocalDateTime(2020, 01, 30, 17, 45, 59).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.True(textWinter.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out var result));
            Assert.Equal(winterTime, result);

            Assert.True(textWinter.ParseDateTime(_zoneGmt, true, "en-GB", MxCultureInfo.FormatType.DateTime, true, out result));
            Assert.Equal(winterTime, result);

            var textSummerBst = "30-06-2020 17:45:59";
            var summerTimeBst = new LocalDateTime(2020, 06, 30, 17, 45, 59).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.True(textSummerBst.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out result));
            Assert.Equal(summerTimeBst, result);

            var textSummerGmt = "30-06-2020 16:45:59";
            Assert.True(textSummerGmt.ParseDateTime(_zoneGmt, true, "en-GB", MxCultureInfo.FormatType.DateTime, true, out result));
            Assert.Equal(summerTimeBst, result);

        }

        [Fact]
        public void StringParseDateTimeToInstantVerboseShortTest()
        {
            var text = "Thursday, 30 January 2020 5:45 PM";
            var expected = new LocalDateTime(2020, 01, 30, 17, 45, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.True(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.Verbose, false, out var result));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void StringParseDateTimeToInstantVerboseLongTest()
        {
            var text = "Thursday, 30 January 2020 17:45:59";

            Assert.True(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.Verbose, true, out var result));
            Assert.Equal(_clock.GetCurrentInstant(), result);
        }

        [Fact]
        public void StringParseDateTimeToInstantMachineShortTest()
        {
            var text = "2020-01-30T17:45:59";

            Assert.True(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.Machine, false, out var result));
            Assert.Equal(_clock.GetCurrentInstant(), result);
        }

        [Fact]
        public void StringParseDateTimeToInstantMachineLongTest()
        {
            var text = "2020-01-30T17:45:59.000000000 (ISO)";

            Assert.True(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.Machine, true, out var result));
            Assert.Equal(_clock.GetCurrentInstant(), result);
        }

        [Fact]
        public void StringParseDateParseTimeToInstantShortTest()
        {
            var textDate = "30-01-2020";
            var textTime = "5:45 PM";
            var expected = new LocalDateTime(2020, 01, 30, 17, 45, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.True(textDate.ParseDate(_zoneGmt, "en-GB", false, out var resultDate));
            Assert.True(textTime.ParseTime(_zoneGmt, false, "en-GB", resultDate, false, out var result));

            Assert.Equal(expected, result);
        }

        [Fact]
        public void StringParseDateParseTimeToInstantLongTest()
        {
            var textDate = "Thursday, 30 January 2020";
            var textTime = "17:45:59";

            Assert.True(textDate.ParseDate(_zoneGmt, "en-GB", true, out var resultDate));
            Assert.True(textTime.ParseTime(_zoneGmt, false, "en-GB", resultDate, true, out var result));

            Assert.Equal(_clock.GetCurrentInstant(), result);
        }

        //  Spring 29-03-2020
        //  GMT clock           BST clock
        //  00:59:59            00:59:59
        //  01:00:00            02:00:00 - daylight saving starts @ 01:00:00 GMT; BST = GMT + 1 hour, so BST values from 1:00:00 to 01:59:59 do not exist - invalid
        //  01:00:01            02:00:01  

        [Fact]
        public void InstantToStringDsSpringTest()
        {
            var gmtEndingSpring = new LocalDateTime(2020, 03, 29, 00, 59, 59).InZoneStrictly(_zoneGmt).ToInstant();  //Instant for 0:59:59 GMT
            var bstStartsSpring = gmtEndingSpring.Plus(Duration.FromSeconds(1));                                                           //Instant for 1:00:00 GMT which is 2:00:00 BST - start of daylight saving

            Assert.Equal("29-03-2020 00:59:59", gmtEndingSpring.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.DateTime, true));
            //clocks move forward at 1:00 AM GMT so BST times between 01:00:00 GMT and 01:59:59 GMT are invalid   
            Assert.Equal("29-03-2020 02:00:00", bstStartsSpring.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.DateTime, true));
        }

        //  Autumn 25-10-2020
        //  GMT clock           BST clock
        //  23:59:59            00:59:59  - last unambiguous value on the BST clock
        //  00:00:00            01:00:00  - A
        //  00:00:01            01:00:01  - A
        //  ...
        //  00:59:59            01:59:59  - A
        //  01:00:00            01:00:00  - B - daylight saving ends @ 01:00:00 GMT (02:00:00 BST); BST = GMT, so BST values from 1:59:59 back to 01:00:00 have duplicates (A & B on timeline) - ambiguous
        //  01:00:01            01:00:01  - B
        //  ...
        //  01:59:59            01:59:59  - B
        //  02:00:00            02:00:00  - first unambiguous value on the BST clock 
        //  02:00:01            02:00:01


        [Fact]
        public void InstantToStringDsAutumnTest()
        {
            var bstLastValidTime = new LocalDateTime(2020, 10, 25, 0, 59, 59).InZoneStrictly(_zoneGmt).ToInstant();    //  Instant for 00:59:59 BST which is 24-10-20 23:59:59 GMT
            var bstFirstAmbiguousTime = bstLastValidTime.Plus(Duration.FromSeconds(1));                                                     //   Instance for 01:00:00 BST - ambiguous
            var bstLastAmbiguousTime = bstFirstAmbiguousTime.Plus(Duration.FromMinutes(59).Plus(Duration.FromSeconds(59)));                 //   Instance for 01:59:59 BST - ambiguous
            var bstFirstValidTime = bstLastValidTime.Plus(Duration.FromHours(2).Plus(Duration.FromSeconds(1)));                             //  Instant for 02:00:00 GMT - end of daylight saving

            //PASS        
            Assert.Equal("25-10-2020 00:59:59", bstLastValidTime.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.DateTime, true));
            //FAIL - clocks move back at 1:00:00 AM GMT (equiv 02:00:00 BST) so times between 01:59:59 BST and 01:00:00 BST are ambiguous
            Assert.Equal("[error: ambiguous time]", bstFirstAmbiguousTime.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.DateTime, true));
            Assert.Equal("[error: ambiguous time]", bstLastAmbiguousTime.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.DateTime, true));
            //PASS
            Assert.Equal("25-10-2020 02:00:00", bstFirstValidTime.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.DateTime, true));
        }

        [Fact]
        public void StringParseDateTimeToInstantDsSpringTest()
        {
            var gmtEndingSpring = new LocalDateTime(2020, 03, 29, 00, 59, 59).InZoneStrictly(_zoneGmt).ToInstant(); //Instant for 00:59:59 GMT 
            var bstStartsSpring = gmtEndingSpring.Plus(Duration.FromSeconds(1));                                                         //Instant for 01:00:00 GMT which is 02:00:00 BST - start of daylight saving
                                                                                                                                         //PASS
            var text = "29-03-2020 00:59:59";   //March 29 2020, 00:59:59 GMT - 1 sec before daylight saving starts
            Assert.True(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out var result));
            Assert.Equal(gmtEndingSpring, result);
            //FAIL
            text = "29-03-2020 01:00:00";       //clocks move forward at 1:00:00 GMT so BST times between 01:00:00 GMT and 01:59:59 GMT are invalid
            Assert.False(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out result));
            text = "29-03-2020 01:59:59";
            Assert.False(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out result));
            //PASS
            text = "29-03-2020 02:00:00";       //March 29 2020, 02:00:00 BST (01:00:00 GMT)  - daylight saving  starts
            Assert.True(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out result));
            Assert.Equal(bstStartsSpring, result);
        }

        [Fact]
        public void StringParseDateTimeToInstantDsAutumnTest()
        {
            var bstLastValidTime = new LocalDateTime(2020, 10, 25, 0, 59, 59).InZoneStrictly(_zoneGmt).ToInstant();    //  Instant for 00:59:59 BST which is 24-10-20 23:59:59 GMT
            var bstFirstValidTime = bstLastValidTime.Plus(Duration.FromHours(2).Plus(Duration.FromSeconds(1)));                             //  Instant for 02:00:00 GMT 
                                                                                                                                            //PASS
            var text = "25-10-2020 00:59:59";   //Oct 25 2020, 00:59:59 BST (24-10-20, 23:59:59 GMT) - 1 hour 1 sec before daylight saving ends 
            Assert.True(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out var result));
            Assert.Equal(bstLastValidTime, result);
            //FAIL
            text = "25-10-2020 01:00:00";       //clocks move back at 2:00:00 BST (01:00:00 GMT) so times between 01:00:00 BST and 01:59:59 BST are ambiguous
            Assert.False(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out result));
            text = "25-10-2020 01:59:59";
            Assert.False(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out result));
            //PASS
            text = "25-10-2020 02:00:00";       //Oct 25 2020, 2:00:00 GMT 
            Assert.True(text.ParseDateTime(_zoneGmt, false, "en-GB", MxCultureInfo.FormatType.DateTime, true, out result));
            Assert.Equal(bstFirstValidTime, result);
        }

        [Fact]
        public void IsDaylightSavingsTimeSpringTest()
        {
            var gmtEndingSpring = new LocalDateTime(2020, 03, 29, 00, 59, 59).InZoneStrictly(_zoneGmt).ToInstant(); //Instant for 00:59:59 GMT 
            var bstStartsSpring = gmtEndingSpring.Plus(Duration.FromSeconds(1));                                                         //Instant for 01:00:00 GMT which is 02:00:00 BST - start of daylight saving

            Assert.False(_zoneGmt.IsDaylightSavingsTime(gmtEndingSpring));
            Assert.True(_zoneGmt.IsDaylightSavingsTime(bstStartsSpring));
        }

        [Fact]
        public void IsDaylightSavingsTimeAutumnTest()
        {
            var bstLastValidTime = new LocalDateTime(2020, 10, 25, 0, 59, 59).InZoneStrictly(_zoneGmt).ToInstant();    //  Instant for 00:59:59 BST which is 24-10-20 23:59:59 GMT
            var bstFirstAmbiguousTime = bstLastValidTime.Plus(Duration.FromSeconds(1));                                                     //   Instance for 01:00:00 BST - ambiguous
            var bstLastAmbiguousTime = bstFirstAmbiguousTime.Plus(Duration.FromMinutes(59).Plus(Duration.FromSeconds(59)));                 //   Instance for 01:59:59 BST - ambiguous
            var bstFirstValidTime = bstLastValidTime.Plus(Duration.FromHours(2).Plus(Duration.FromSeconds(1)));                             //  Instant for 02:00:00 GMT 

            Assert.True(_zoneGmt.IsDaylightSavingsTime(bstLastValidTime));
            Assert.True(_zoneGmt.IsDaylightSavingsTime(bstFirstAmbiguousTime));
            Assert.True(_zoneGmt.IsDaylightSavingsTime(bstLastAmbiguousTime));
            Assert.False(_zoneGmt.IsDaylightSavingsTime(bstFirstValidTime));
        }

    }
}
