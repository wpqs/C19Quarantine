using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class AppTimeZonesTest
    {
        [Fact]
        public void DefaultSelectedTest()
        {
            var zones = new AppTimeZones();

            Assert.Equal(AppTimeZones.DefaultAcronym, zones.Selected);
        }

        [Fact]
        public void GetDaylightSavingNameTest()
        {
            Assert.Equal("British Summer Time", AppTimeZones.GetDaylightSavingName(AppTimeZones.AcronymGmt));
            Assert.Equal("Central Europe Summer Time", AppTimeZones.GetDaylightSavingName(AppTimeZones.AcronymCet));
        }

        [Fact]
        public void GetDaylightSavingAcronymTest()
        {
            Assert.Equal("BST", AppTimeZones.GetDaylightSavingAcronym(AppTimeZones.AcronymGmt));
            Assert.Equal("CEST", AppTimeZones.GetDaylightSavingAcronym(AppTimeZones.AcronymCet));
        }

        [Fact]
        public void GetTzDbNameTest()
        {
            Assert.Equal("Europe/London", AppTimeZones.GetTzDbName(AppTimeZones.AcronymGmt));
            Assert.Equal("Europe/Paris", AppTimeZones.GetTzDbName(AppTimeZones.AcronymCet));
        }
    }
}
