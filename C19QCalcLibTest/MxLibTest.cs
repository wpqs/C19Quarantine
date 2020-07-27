using C19QCalcLibTest.Support;
using NodaTime;
using Xunit;

namespace C19QCalcLibTest
{
    public class MxLibTest
    {
        private readonly Page _page;
        private readonly DateTimeZone _zoneGmt;
        private readonly DateTimeZone _zoneCet;

        public MxLibTest()
        {
            _page = new Page(new HttpContextAccessorMock());
            _zoneGmt = DateTimeZoneProviders.Tzdb[id: "Europe/London"];
            _zoneCet = DateTimeZoneProviders.Tzdb[id: "Europe/Paris"];
        }


        [Fact]
        public void GetTextTest()
        {
            var mockAccessor = (HttpContextAccessorMock)_page.HttpContextAccessor;
            mockAccessor.AddCookie(".AspNetCore.Culture", "c=en|uic=en-GB");

            Assert.Equal("Welcome - en-GB", _page.GetText("Welcome"));

            mockAccessor.ClearCookies();
            mockAccessor.AddCookie(".AspNetCore.Culture", "c=en|uic=fr-CH");

            Assert.Equal("Bienvenue - fr-CH", _page.GetText("Welcome"));

        }

        [Fact]
        public void InstantToStringTest()
        {
            var mockAccessor = (HttpContextAccessorMock)_page.HttpContextAccessor;
            mockAccessor.AddCookie(".AspNetCore.Culture", "c=en-GB|uic=en-GB");
            mockAccessor.AddCookie(".AspNetCore.MxTimeZone", "tz=GMT|dsa=yes");

            var startIsolationLondon = new LocalDateTime(year: 2020, month: 03, day: 29, hour: 03, minute: 59, second: 59).InZoneLeniently(zone: _zoneGmt).ToInstant();

            Assert.Equal("29-03-2020 3:59 AM", _page.InstantToString(startIsolationLondon));
            Assert.Equal("29.03.2020 03:59", _page.InstantToString(startIsolationLondon, "fr-CH"));

            mockAccessor.ClearCookies();
            mockAccessor.AddCookie(".AspNetCore.Culture", "c=fr-CH|uic=en-GB");
            mockAccessor.AddCookie(".AspNetCore.MxTimeZone", "tz=CET|dsa=yes");

            var startIsolationGeneve = new LocalDateTime(year: 2020, month: 03, day: 29, hour: 03, minute: 59, second: 59).InZoneLeniently(zone: _zoneCet).ToInstant();

            Assert.Equal("29.03.2020 02:59", _page.InstantToString(startIsolationGeneve));
            Assert.Equal("29-03-2020 2:59 AM", _page.InstantToString(startIsolationGeneve, "en-GB"));
        }

        [Fact]
        public void ParseTimeDateTest()
        {
            var mockAccessor = (HttpContextAccessorMock)_page.HttpContextAccessor;
            mockAccessor.AddCookie(".AspNetCore.Culture", "c=en-GB|uic=en-GB");
            mockAccessor.AddCookie(".AspNetCore.MxTimeZone", "tz=GMT|dsa=yes");

            var startIsolationLondon = new LocalDateTime(year: 2020, month: 03, day: 29, hour: 03, minute: 59, second: 00).InZoneLeniently(zone: _zoneGmt).ToInstant();

            Assert.Equal(startIsolationLondon, _page.ParseTimeDate("29-03-2020 3:59 AM"));

        }
    }
}