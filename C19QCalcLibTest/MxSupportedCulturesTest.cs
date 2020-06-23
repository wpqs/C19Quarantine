using C19QCalcLib;
using C19QCalcLibTest.Support;
using Xunit;

namespace C19QCalcLibTest
{
    public class MxSupportedCulturesTest
    {
        [Fact]
        public void GetCultureInfoTest()
        {
            var cultures = new AppSupportedCultures();
            Assert.Equal(6, cultures.GetSupportedCultures().Length);

            Assert.Equal("en", cultures.GetCultureInfo("en").Name);
            Assert.Equal("en-GB", cultures.GetCultureInfo("en-GB").Name);
            Assert.Equal("de-CH", cultures.GetCultureInfo("de-CH").Name);

            Assert.Null(cultures.GetCultureInfo("xxx"));

        }

        [Fact]
        public void GetSupportedCulturesTest()
        {
            var cultures = new AppSupportedCultures();

            Assert.Equal(6, cultures.GetSupportedCultures().Length);
            Assert.Equal("en", cultures.GetSupportedCultures()[0]);
            Assert.Equal("en-GB", cultures.GetSupportedCultures()[1]);
            Assert.Equal("de-CH", cultures.GetSupportedCultures()[5]);
        }

        [Fact]
        public void GetSupportedCulturesInfoTest()
        {
            var cultures = new AppSupportedCultures();

            Assert.Equal(6, cultures.GetSupportedCulturesInfo().Length);
            Assert.Equal("en", cultures.GetSupportedCulturesInfo()[0].Name);
            Assert.Equal("en-GB", cultures.GetSupportedCulturesInfo()[1].Name);
            Assert.Equal("de-CH", cultures.GetSupportedCulturesInfo()[5].Name);
        }

        [Fact]
        public void IsSupportedTest()
        {
            var cultures = new AppSupportedCultures();

            Assert.True(cultures.IsSupported("en-GB"));
            Assert.False(cultures.IsSupported("xx-GB"));
        }

        [Fact]
        public void IsSupportedFailTest()
        {
            var cultures = new AppSupportedCultures();

            Assert.False(cultures.IsSupported(null));
            Assert.False(cultures.IsSupported(""));
        }

        [Fact]
        public void GetCulturesValueToSetTest()
        {
            var cultures = new AppSupportedCulturesTestSupport();

            Assert.Equal("c=fr-CH|uic=en-GB", cultures.GetCulturesValueToSetPublic("fr-CH", "en-GB"));
            Assert.Equal("c=en-GB|uic=fr-CH", cultures.GetCulturesValueToSetPublic("en-GB", "fr-CH"));
            Assert.Equal("c=fr-CH|uic=en-GB", cultures.GetCulturesValueToSetPublic("fr-CH", "en-GB"));
            Assert.Equal("c=en-GB|uic=fr-CH", cultures.GetCulturesValueToSetPublic("en-GB", "fr-CH"));

            Assert.Equal("culture=fr-CH&ui-culture=en-GB", cultures.GetCulturesValueToSetPublic("fr-CH", "en-GB", false));
            Assert.Equal("culture=en-GB&ui-culture=fr-CH", cultures.GetCulturesValueToSetPublic("en-GB", "fr-CH", false));

        }

        [Fact]
        public void GetCulturesValueToSetFailTest()
        {
            var cultures = new AppSupportedCulturesTestSupport();

            Assert.Equal("c=en|uic=en", cultures.GetCulturesValueToSetPublic(null, null));
            Assert.Equal("c=en|uic=en", cultures.GetCulturesValueToSetPublic(null, "fr-CH"));   //both parameters must be valid
            Assert.Equal("c=en|uic=en", cultures.GetCulturesValueToSetPublic("fr-CH", null));   //both parameters must be valid
        }

        [Fact]
        public void GetCultureTabFromValueTest()
        {
            var cultures = new AppSupportedCulturesTestSupport();

            Assert.Equal("fr-CH", cultures.GetCultureTabFromValuePublic("c=fr-CH|uic=en-GB"));
            Assert.Equal("fr-CH", cultures.GetCultureTabFromValuePublic("culture=fr-CH&ui-culture=en-GB", false));
        }

        [Fact]
        public void GetCultureTabFromValueFailTest()
        {
            var cultures = new AppSupportedCulturesTestSupport();

            Assert.Equal("en", cultures.GetCultureTabFromValuePublic(""));
            Assert.Equal("en", cultures.GetCultureTabFromValuePublic(null));

            Assert.Equal("en", cultures.GetCultureTabFromValuePublic("cx=fr-CH£uic=en-GB"));
            Assert.Equal("en", cultures.GetCultureTabFromValuePublic("cx=fr-CH|uic=en-GB"));
            Assert.Equal("en", cultures.GetCultureTabFromValuePublic("c="));
            Assert.Equal("en", cultures.GetCultureTabFromValuePublic("c=fr-CH x|uic=en-GB"));
            Assert.Equal("en", cultures.GetCultureTabFromValuePublic("cx=fr-CH|uic=en-GB"));
            Assert.Equal("en", cultures.GetCultureTabFromValuePublic("uic=en-GB"));
        }


        [Fact]
        public void GetUiCultureTabFromValueTest()
        {
            var cultures = new AppSupportedCulturesTestSupport();

            Assert.Equal("en-GB", cultures.GetUiCultureTabFromValuePublic("c=fr-CH|uic=en-GB"));
            Assert.Equal("en-GB", cultures.GetUiCultureTabFromValuePublic("culture=fr-CH&ui-culture=en-GB", false));
        }

        [Fact]
        public void GetUiCultureTabFromValueFailTest()
        {
            var cultures = new AppSupportedCulturesTestSupport();

            Assert.Equal("en", cultures.GetUiCultureTabFromValuePublic(""));
            Assert.Equal("en", cultures.GetUiCultureTabFromValuePublic(null));

            Assert.Equal("en", cultures.GetUiCultureTabFromValuePublic("c=fr-CH£uic=en-GB"));
            Assert.Equal("en", cultures.GetUiCultureTabFromValuePublic("c=fr-CH|uic="));
            Assert.Equal("en", cultures.GetUiCultureTabFromValuePublic("c=fr-CH|uicx="));
            Assert.Equal("en", cultures.GetUiCultureTabFromValuePublic("c=fr-CH"));
            Assert.Equal("en", cultures.GetUiCultureTabFromValuePublic("c=en-GB"));
        }
    }
}