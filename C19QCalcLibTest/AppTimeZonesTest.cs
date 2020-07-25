using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class AppTimeZonesTest
    {
        [Fact]
        public void DefaultSelectedTest()
        {
            var zones = new AppSupportedTimeZones();

            Assert.Equal(AppSupportedTimeZones.DefaultAcronym, zones.Selected);
        }

        [Fact]
        public void GetDaylightSavingNameTest()
        {
            var zones = new AppSupportedTimeZones();

            Assert.Equal("British Summer Time", zones.GetDaylightSavingName(AppSupportedTimeZones.AcronymGmt));
            Assert.Equal("Central Europe Summer Time", zones.GetDaylightSavingName(AppSupportedTimeZones.AcronymCet));
        }

        [Fact]
        public void GetDaylightSavingAcronymTest()
        {
            var zones = new AppSupportedTimeZones();

            Assert.Equal("BST", zones.GetDaylightSavingAcronym(AppSupportedTimeZones.AcronymGmt));
            Assert.Equal("CEST", zones.GetDaylightSavingAcronym(AppSupportedTimeZones.AcronymCet));
        }

        [Fact]
        public void GetTzDbNameTest()
        {
            var zones = new AppSupportedTimeZones();

            Assert.Equal("Europe/London", zones.GetTzDbName(AppSupportedTimeZones.AcronymGmt));
            Assert.Equal("Europe/Paris", zones.GetTzDbName(AppSupportedTimeZones.AcronymCet));
        }
    }
}
