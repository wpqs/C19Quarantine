using System.Diagnostics.CodeAnalysis;
using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
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
            var timezones = new AppSupportedTimeZones();

            Assert.Equal("tz=GMT|dsa=yes", timezones.GetTimeZoneEncodedValue("GMT", true));
            Assert.Equal("tz=GMT|dsa=no", timezones.GetTimeZoneEncodedValue("GMT", false));
            Assert.Equal("tz=CET|dsa=yes", timezones.GetTimeZoneEncodedValue("CET", true));

            Assert.Equal("timezone=CET&ds-auto=yes", timezones.GetTimeZoneEncodedValue("CET", true, false));
            Assert.Equal("timezone=CET&ds-auto=no", timezones.GetTimeZoneEncodedValue("CET", false, false));
            Assert.Equal("timezone=GMT&ds-auto=yes", timezones.GetTimeZoneEncodedValue("GMT", true, false));
        }

        [Fact]
        public void GetTimeZoneValueToSetFailTest()
        {
            var timezones = new AppSupportedTimeZones();

            Assert.Equal("tz=GMT|dsa=yes", timezones.GetTimeZoneEncodedValue(null, true));
            Assert.Equal("tz=GMT|dsa=yes", timezones.GetTimeZoneEncodedValue(null, false));
        }

        [Fact]
        public void GetTimeZoneAcronymFromValueTest()
        {
            var timezones = new AppSupportedTimeZones();

            Assert.Equal("GMT", timezones.GetTimeZoneAcronym("tz=GMT|dsa=yes"));
            Assert.Equal("CET", timezones.GetTimeZoneAcronym("tz=CET|dsa=yes"));

        }

        [Fact]
        public void GetTimeZoneAcronymFromValueFailTest()
        {
            var timezones = new AppSupportedTimeZones();

            Assert.Equal("GMT", timezones.GetTimeZoneAcronym(null));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronym(""));

            Assert.Equal("GMT", timezones.GetTimeZoneAcronym("tz=CET"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronym("dsa=yes"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronym("tz=CET£dsa=yes"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronym("tz£CET|dsa=yes"));
            Assert.Equal("CET", timezones.GetTimeZoneAcronym("tz=CET|dsa£yes"));
            Assert.Equal("CET", timezones.GetTimeZoneAcronym("tz=CET|dsa=xxx"));
            Assert.Equal("CET", timezones.GetTimeZoneAcronym("tz=CET|dsx=yes"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronym("tz=CXX|dsa=yes"));
            Assert.Equal("GMT", timezones.GetTimeZoneAcronym("tx=CET|dsa=yes"));

        }

        [Fact]
        public void GetDaylightSavingAutoFromValueTest()
        {
            var timezones = new AppSupportedTimeZones();

            Assert.True(timezones.IsDaylightSavingAuto("tz=GMT|dsa=yes"));
            Assert.False(timezones.IsDaylightSavingAuto("tz=CET|dsa=no"));
        }

        [Fact]
        public void GetDaylightSavingAutoFromValueFailTest()
        {
            var timezones = new AppSupportedTimeZones();

            Assert.True(timezones.IsDaylightSavingAuto(null));
            Assert.True(timezones.IsDaylightSavingAuto(""));

            Assert.False(timezones.IsDaylightSavingAuto("tz=CET|dsa=no"));
            Assert.False(timezones.IsDaylightSavingAuto("tz£CET|dsa=no"));
            Assert.False(timezones.IsDaylightSavingAuto("tz=CXX|dsa=no"));

            Assert.True(timezones.IsDaylightSavingAuto("tz=CET"));
            Assert.True(timezones.IsDaylightSavingAuto("dsa=no"));
            Assert.True(timezones.IsDaylightSavingAuto("tz=CET£dsa=no"));
            Assert.True(timezones.IsDaylightSavingAuto("tz=CET|dsa£no"));
            Assert.True(timezones.IsDaylightSavingAuto("tz=CET|dsa=xxx"));
            Assert.True(timezones.IsDaylightSavingAuto("tz=CET|dsx=no"));
        }
    }
}