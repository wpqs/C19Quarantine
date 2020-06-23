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
            Assert.Equal("British Summer Time", AppSupportedTimeZones.GetDaylightSavingName(AppSupportedTimeZones.AcronymGmt));
            Assert.Equal("Central Europe Summer Time", AppSupportedTimeZones.GetDaylightSavingName(AppSupportedTimeZones.AcronymCet));
        }

        [Fact]
        public void GetDaylightSavingAcronymTest()
        {
            Assert.Equal("BST", AppSupportedTimeZones.GetDaylightSavingAcronym(AppSupportedTimeZones.AcronymGmt));
            Assert.Equal("CEST", AppSupportedTimeZones.GetDaylightSavingAcronym(AppSupportedTimeZones.AcronymCet));
        }

        [Fact]
        public void GetTzDbNameTest()
        {
            Assert.Equal("Europe/London", AppSupportedTimeZones.GetTzDbName(AppSupportedTimeZones.AcronymGmt));
            Assert.Equal("Europe/Paris", AppSupportedTimeZones.GetTzDbName(AppSupportedTimeZones.AcronymCet));
        }
    }
}
