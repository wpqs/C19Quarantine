using C19QCalcLib;
using C19QCalcLibTest.Support;
using Xunit;

namespace C19QCalcLibTest
{
    public class MxSupportedTimeZonesTest
    {
        [Fact]
        public void GetSupportedCulturesTest()
        {
            var timezones = new AppSupportedTimeZones();

            Assert.Equal(2, timezones.GetSupportedTimeZones().Length);
            Assert.Equal("GMT", timezones.GetSupportedTimeZones()[0]);
            Assert.Equal("CET", timezones.GetSupportedTimeZones()[1]);
        }


        [Fact]
        public void IsSupportedTest()
        {
            var timezones = new AppSupportedTimeZones();

            Assert.True(timezones.IsSupported("CET"));
            Assert.False(timezones.IsSupported("XXX"));
        }

        [Fact]
        public void IsSupportedFailTest()
        {
            var timezones = new AppSupportedTimeZones();

            Assert.False(timezones.IsSupported(null));
            Assert.False(timezones.IsSupported(""));
        }

        [Fact]
        public void GetTimeZoneValueToSetTest()
        {
            var timezones = new AppSupportedTimeZonesTestSupport();

            Assert.Equal("tz=GMT|dsa=yes", timezones.GetTimeZoneValueToSetPublic("GMT", true));
            Assert.Equal("tz=GMT|dsa=no", timezones.GetTimeZoneValueToSetPublic("GMT", false));
            Assert.Equal("tz=CET|dsa=yes", timezones.GetTimeZoneValueToSetPublic("CET", true));

            Assert.Equal("timezone=CET&ds-auto=yes", timezones.GetTimeZoneValueToSetPublic("CET", true, false));
            Assert.Equal("timezone=CET&ds-auto=no", timezones.GetTimeZoneValueToSetPublic("CET", false, false));
            Assert.Equal("timezone=GMT&ds-auto=yes", timezones.GetTimeZoneValueToSetPublic("GMT", true, false));
        }

        [Fact]
        public void GetTimeZoneValueToSetFailTest()
        {
            var timezones = new AppSupportedTimeZonesTestSupport();

            Assert.Equal("tz=GMT|dsa=yes", timezones.GetTimeZoneValueToSetPublic(null, true));
            Assert.Equal("tz=GMT|dsa=yes", timezones.GetTimeZoneValueToSetPublic(null, false));
        }

        [Fact]
        public void GetTimeZoneAcronymFromValueTest()
        {
            var timezones = new AppSupportedTimeZonesTestSupport();

            Assert.Equal("GMT", timezones.GetTimeZoneAcronymFromValuePublic("tz=GMT|dsa=yes"));
            Assert.Equal("CET", timezones.GetTimeZoneAcronymFromValuePublic("tz=CET|dsa=yes"));

        }

        [Fact]
        public void GetTimeZoneAcronymFromValueFailTest()
        {
            var timezones = new AppSupportedTimeZonesTestSupport();

            Assert.Equal("GMT", timezones.GetTimeZoneAcronymFromValuePublic(null));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronymFromValuePublic(""));

            Assert.Equal("GMT", timezones.GetTimeZoneAcronymFromValuePublic("tz=CET"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronymFromValuePublic("dsa=yes"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronymFromValuePublic("tz=CET£dsa=yes"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronymFromValuePublic("tz£CET|dsa=yes"));
            Assert.Equal("CET", timezones.GetTimeZoneAcronymFromValuePublic("tz=CET|dsa£yes"));
            Assert.Equal("CET", timezones.GetTimeZoneAcronymFromValuePublic("tz=CET|dsa=xxx"));
            Assert.Equal("CET", timezones.GetTimeZoneAcronymFromValuePublic("tz=CET|dsx=yes"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronymFromValuePublic("tz=CXX|dsa=yes"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronymFromValuePublic("tx=CET|dsa=yes"));

        }

        [Fact]
        public void GetDaylightSavingAutoFromValueTest()
        {
            var timezones = new AppSupportedTimeZonesTestSupport();

            Assert.Equal("yes", timezones.GetDaylightSavingAutoFromValuePublic("tz=GMT|dsa=yes"));
            Assert.Equal("no", timezones.GetDaylightSavingAutoFromValuePublic("tz=CET|dsa=no"));
        }

        [Fact]
        public void GetDaylightSavingAutoFromValueFailTest()
        {
            var timezones = new AppSupportedTimeZonesTestSupport();

            Assert.Equal("yes", timezones.GetDaylightSavingAutoFromValuePublic(null));
            Assert.Equal("yes", timezones.GetDaylightSavingAutoFromValuePublic(""));

            Assert.Equal("yes", timezones.GetDaylightSavingAutoFromValuePublic("tz=CET"));
            Assert.Equal("yes", timezones.GetDaylightSavingAutoFromValuePublic("dsa=no"));
            Assert.Equal("yes", timezones.GetDaylightSavingAutoFromValuePublic("tz=CET£dsa=no"));
            Assert.Equal("no", timezones.GetDaylightSavingAutoFromValuePublic("tz£CET|dsa=no"));
            Assert.Equal("yes", timezones.GetDaylightSavingAutoFromValuePublic("tz=CET|dsa£no"));
            Assert.Equal("xxx", timezones.GetDaylightSavingAutoFromValuePublic("tz=CET|dsa=xxx"));
            Assert.Equal("yes", timezones.GetDaylightSavingAutoFromValuePublic("tz=CET|dsx=no"));
            Assert.Equal("no", timezones.GetDaylightSavingAutoFromValuePublic("tz=CXX|dsa=no"));
            Assert.Equal("no", timezones.GetDaylightSavingAutoFromValuePublic("tx=CET|dsa=no"));
        }
    }
}