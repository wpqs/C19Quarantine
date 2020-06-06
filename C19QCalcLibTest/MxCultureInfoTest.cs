using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using C19QCalcLib;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace C19QCalcLibTest
{
    public class MxCultureInfoTest
    {
        private readonly IClock _clock;
        private readonly DateTimeZone _zoneGmt;
        private readonly LocalDateTime _clockTime;
        private readonly Instant _startIsolation;

        public MxCultureInfoTest()
        {
            _zoneGmt = DateTimeZoneProviders.Tzdb[id: "Europe/London"];

            _startIsolation = new LocalDateTime(year: 2020, month: 03, day: 29, hour: 00, minute: 59, second: 59).InZoneStrictly(zone: _zoneGmt).ToInstant();
            _clockTime = new LocalDateTime(year: 2020, month: 01, day: 30, hour: 17, minute: 45, second: 59);

            _clock = new FakeClock(initial: _clockTime.InZoneStrictly(zone: _zoneGmt).ToInstant());

        }

        [Fact]
        public void InstanceTest()
        {
            Assert.Equal("en-GB", MxCultureInfo.Instance.GetCultureInfo("en-GB").Name);
            Assert.Equal("en-GB", MxCultureInfo.Instance.GetCultureInfo("en-GB").Name);
        }

        [Fact]
        public void GetFormatSpecifierTest()
        {
            Assert.Equal("g", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.DateTime, false));
            Assert.Equal("G", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.DateTime, true));
            Assert.Equal("d", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Date, false));
            Assert.Equal("D", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Date, true));
            Assert.Equal("t", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Time, false));
            Assert.Equal("T", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Time, true));
            Assert.Equal("s", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Machine, false));
            Assert.Equal("r", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Machine, true));
            Assert.Equal("f", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Verbose, false));
            Assert.Equal("F", MxCultureInfo.GetFormatSpecifier(MxCultureInfo.FormatType.Verbose, true));
        }

        [Fact]
        public void ShortDateTimeTest()
        {
            Assert.Equal(_clockTime, _clock.GetCurrentInstant().InZone(_zoneGmt).LocalDateTime);

            Assert.Equal("en-GB", MxCultureInfo.Instance.GetCultureInfo("en-GB").Name);
            Assert.Equal("fr-CH", MxCultureInfo.Instance.GetCultureInfo("fr-CH").Name);

            Assert.Equal("29-03-2020 12:59 AM", _startIsolation.ToString("en-GB", _zoneGmt)); //12:59 AM is 00:59 in 24 hour clock
            Assert.Equal("29.03.2020 00:59", _startIsolation.ToString("fr-CH", _zoneGmt));
        }

        [Fact]
        public void GetCultureInfoNameTest()
        {
            Assert.Equal("en-GB", MxCultureInfo.Instance.GetCultureInfo("en-GB").Name);
            Assert.Equal("fr-CH", MxCultureInfo.Instance.GetCultureInfo("fr-CH").Name);
            Assert.Equal("en-US", MxCultureInfo.Instance.GetCultureInfo("en-US").Name);
        }

        [Fact]
        public void DateTimeSupportedFormatSpecifiersTest()
        {
            //Documentation for Format Strings: https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tostring?view=netcore-3.1

            DateTime timA = new DateTime(2020, 03, 29, 00, 59, 59, DateTimeKind.Utc);
            DateTime timB = new DateTime(2020, 04, 2, 00, 59, 59, DateTimeKind.Utc);


            //formatSpecifier d
            Assert.Equal("29-03-2020", timA.ToString("d", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("3/29/2020", timA.ToString("d", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("29.03.2020", timA.ToString("d", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            Assert.Equal("Sunday, 29 March 2020", timA.ToString("D", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("29 March 2020", timA.ToString("D", CultureInfo.GetCultureInfo("en-GB")));
            Assert.Equal("Sunday, March 29, 2020", timA.ToString("D", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("dimanche, 29 mars 2020", timA.ToString("D", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            Assert.Equal("02-04-2020", timB.ToString("d", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("Thursday, 2 April 2020", timB.ToString("D", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("Thursday, April 2, 2020", timB.ToString("D", MxCultureInfo.Instance.GetCultureInfo("en-US")));

            //formatSpecifier f
            Assert.Equal("Sunday, 29 March 2020 12:59 AM", timA.ToString("f", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("Sunday, March 29, 2020 12:59 AM", timA.ToString("f", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("dimanche, 29 mars 2020 00:59", timA.ToString("f", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            Assert.Equal("Sunday, 29 March 2020 00:59:59", timA.ToString("F", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("Sunday, March 29, 2020 12:59:59 AM", timA.ToString("F", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("dimanche, 29 mars 2020 00:59:59", timA.ToString("F", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            //formatSpecifier g
            Assert.Equal("29-03-2020 12:59 AM", timA.ToString("g", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("3/29/2020 12:59 AM", timA.ToString("g", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("29.03.2020 00:59", timA.ToString("g", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            Assert.Equal("29-03-2020 00:59:59", timA.ToString("G", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("3/29/2020 12:59:59 AM", timA.ToString("G", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("29.03.2020 00:59:59", timA.ToString("G", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            //formatSpecifier m
            Assert.Equal("29 March", timA.ToString("m", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("March 29", timA.ToString("m", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("29 mars", timA.ToString("m", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            //formatSpecifier 0
            Assert.Equal("2020-03-29T00:59:59.0000000Z", timA.ToString("o", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("2020-03-29T00:59:59.0000000Z", timA.ToString("o", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("2020-03-29T00:59:59.0000000Z", timA.ToString("o", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            //formatSpecifier r
            Assert.Equal("Sun, 29 Mar 2020 00:59:59 GMT", timA.ToString("r", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("Sun, 29 Mar 2020 00:59:59 GMT", timA.ToString("r", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("Sun, 29 Mar 2020 00:59:59 GMT", timA.ToString("r", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            //formatSpecifier s
            Assert.Equal("2020-03-29T00:59:59", timA.ToString("s", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("2020-03-29T00:59:59", timA.ToString("s", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("2020-03-29T00:59:59", timA.ToString("s", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            //formatSpecifier t
            Assert.Equal("12:59 AM", timA.ToString("t", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("12:59 AM", timA.ToString("t", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("00:59", timA.ToString("t", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            Assert.Equal("00:59:59", timA.ToString("T", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("12:59:59 AM", timA.ToString("T", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("00:59:59", timA.ToString("T", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            //formatSpecifier u
            Assert.Equal("2020-03-29 00:59:59Z", timA.ToString("u", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("2020-03-29 00:59:59Z", timA.ToString("u", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("2020-03-29 00:59:59Z", timA.ToString("u", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));
                //WARNING: The "U" gives invalid date unless DateTime is DateTimeKind.UTC - see https://stackoverflow.com/questions/62009624/why-does-format-specifier-u-alter-the-value-of-datetime
            Assert.Equal("Sunday, 29 March 2020 00:59:59", timA.ToString("U", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("Sunday, March 29, 2020 12:59:59 AM", timA.ToString("U", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("dimanche, 29 mars 2020 00:59:59", timA.ToString("U", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));

            //formatSpecifier Y
            Assert.Equal("March 2020", timA.ToString("Y", MxCultureInfo.Instance.GetCultureInfo("en-GB")));
            Assert.Equal("March 2020", timA.ToString("Y", MxCultureInfo.Instance.GetCultureInfo("en-US")));
            Assert.Equal("mars 2020", timA.ToString("Y", MxCultureInfo.Instance.GetCultureInfo("fr-CH")));
        }

        [Fact]
        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
        public void NodaSupportedFormatSpecifiersTest()
        {
            //formatSpecifier d - date
            Assert.Equal("29-03-2020", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Date));
            Assert.Equal("29.03.2020", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.Date));

            Assert.Equal("Sunday, 29 March 2020", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Date, true));
            Assert.Equal("dimanche, 29 mars 2020", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.Date, true));

            //formatSpecifier t - time
            Assert.Equal("12:59 AM", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Time));
            Assert.Equal("00:59", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.Time));

            Assert.Equal("00:59:59", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Time, true));
            Assert.Equal("00:59:59", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.Time, true));

            //formatSpecifier f - verbose
            Assert.Equal("Sunday, 29 March 2020 12:59 AM", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Verbose));
            Assert.Equal("dimanche, 29 mars 2020 00:59", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.Verbose));
            
            Assert.Equal("Sunday, 29 March 2020 00:59:59", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Verbose, true));
            Assert.Equal("dimanche, 29 mars 2020 00:59:59", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.Verbose, true));

            //formatSpecifier g - datetime
            Assert.Equal("29-03-2020 12:59 AM", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.DateTime));    //date uses ShortDatePattern not LongDatePattern; originally set as yyyy, but for en-GB now set as yy
            Assert.Equal("29.03.2020 00:59", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.DateTime));

            Assert.Equal("29-03-2020 00:59:59", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.DateTime, true));    //date uses ShortDatePattern not LongDatePattern; originally set as yyyy, but for en-GB now set as yy
            Assert.Equal("29.03.2020 00:59:59", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.DateTime, true));

            //formatSpecifier s - machine proc short
            Assert.Equal("2020-03-29T00:59:59", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Machine));
            Assert.Equal("2020-03-29T00:59:59", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.Machine));

            //formatSpecifier r - machine proc full ISO
            Assert.Equal("2020-03-29T00:59:59.000000000 (ISO)", _startIsolation.ToString("en-GB", _zoneGmt, false, MxCultureInfo.FormatType.Machine, true));
            Assert.Equal("2020-03-29T00:59:59.000000000 (ISO)", _startIsolation.ToString("fr-CH", _zoneGmt, false, MxCultureInfo.FormatType.Machine, true));

        }
    }
}